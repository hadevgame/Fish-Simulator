using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UCExtension;
using UCExtension.GUI;
using System.Collections.Generic;

public class CrossHairInteraction : MonoBehaviour
{
    [SerializeField] private Image crosshair;
    [SerializeField] private float rayDistance = 3f;

    [SerializeField] private Button buttonInteract;
    [SerializeField] private Button buttonDrop;
    [SerializeField] private Button buttonPlacing;
    [SerializeField] private Image holdLoad;
    
    private GameObject lastHitObject;
    private InteractionController interactionController;
    private UnityAction currentAction;

    private float holdStartTime;
    private bool isHolding;
    private bool isMessageShown = false;

    List<Button> listButton;
    private void Start()
    {
        interactionController = GetComponent<InteractionController>();
        listButton = GUIController.Ins.Open<PlayGUI>().GetButton();
        buttonInteract = listButton[0];
        buttonDrop = listButton[1];
        buttonPlacing = listButton[2];
        AddHoldEventsToButton(buttonInteract);
    }

    void Update()
    {
        
        if (RaycastHitsFishTank(out GameObject fishTank) && interactionController.heldObject == null)
        {
            if (!isMessageShown) 
            {
                NotifiGUI.Instance.ShowPopup("Hold Interact to placing");
                isMessageShown = true; 
            }
        }
        else
        {
            isMessageShown = false;
            isHolding = false;
            holdLoad.gameObject.SetActive(false);
            holdLoad.fillAmount = 0;
        }

        if (isHolding)
        {
            float progress = (Time.time - holdStartTime) / 2f;
            holdLoad.fillAmount = Mathf.Clamp01(progress);
            if (!fishTank.GetComponent<BaseTank>().isValidSlot) 
            {
                isHolding = false;
                holdLoad.gameObject.SetActive(false);
                holdLoad.fillAmount = 0;
            }
        }
        if (isHolding && Time.time - holdStartTime >= 2f)
        {
            holdLoad.gameObject.SetActive(false);
        }

        if (interactionController.heldObject == null && !interactionController.isHeld)
        {
            ToggleButton(buttonDrop, false);
            ToggleButton(buttonPlacing, false);
            HandleRaycast();
        }
        else if (interactionController.heldObject != null && interactionController.isHeld && !interactionController.isPlacing)
        {
            if (interactionController.heldObject.CompareTag(GameConstants.BOX)) 
            {
                ToggleButton(buttonPlacing, true);
                buttonPlacing.onClick.RemoveAllListeners();
                buttonPlacing.onClick.AddListener(OnPlacing);
            }
            HandleHeldObject();
            
        }
        if (interactionController.isPlacing)
        {
            ToggleButton(buttonDrop, false);
            ToggleButton(buttonPlacing, false);

            buttonPlacing.onClick.RemoveAllListeners();
            buttonPlacing.onClick.AddListener(StopPlacing);

            buttonInteract.onClick.RemoveAllListeners();
            buttonInteract.onClick.AddListener(StopPlacing);

            buttonDrop.onClick.RemoveAllListeners();
        }
    }
    private void ToggleButton(Button button, bool isActive)
    {
        button.gameObject.SetActive(isActive);
        button.interactable = isActive;
    }
    public void OnPlacing()
    {
        if (interactionController.heldObject != null)
        {
            interactionController.StartPlacingMode();
        }
    }
    public void StopPlacing()
    {
        if (interactionController.heldObject != null)
        {
            Rigidbody rb = interactionController.heldObject.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;

            interactionController.ConfirmPlacingMode();
        }
        interactionController.ConfirmPlacingMode();
    }
    public void StartHold()
    {
        if (interactionController.heldObject != null) return;
        if (!RaycastHitsFishTank(out GameObject fishTank)) return;
        if (!fishTank.GetComponent<BaseTank>().isValidSlot) return;

        holdStartTime = Time.time;
        isHolding = true;

        NotifiGUI.Instance.ShowPopup("Hold to placing");
        holdLoad.gameObject.SetActive(true);
        holdLoad.fillAmount = 0;
    }

    public void StopHold()
    {
        if (!isHolding) return;

        if (Time.time - holdStartTime >= 2f)
        {
            if (interactionController.heldObject == null && RaycastHitsFishTank(out GameObject fishTank))
            {
                fishTank.transform.SetParent(interactionController.handPosition);
                interactionController.heldObject = fishTank;
                OnPlacing();
                holdLoad.gameObject.SetActive(false);
                holdLoad.fillAmount = 0;
            }
        }
        else 
        {
            holdStartTime = 0;
            holdLoad.gameObject.SetActive(false);
            holdLoad.fillAmount = 0;
        }

        isHolding = false;
    }
    private void AddHoldEventsToButton(Button button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        // PointerDown → StartHold
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((eventData) => { StartHold(); });
        trigger.triggers.Add(entryDown);

        // PointerUp → StopHold
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((eventData) => { StopHold(); });
        trigger.triggers.Add(entryUp);
    }
    private void HandleRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag(GameConstants.FISH_TANK_BOX) || hitObject.CompareTag(GameConstants.BOX))
            {
                HandleInteraction(hitObject, () => interactionController.PickupObject(hitObject));
                if (hitObject.CompareTag(GameConstants.FISH_TANK_BOX))
                    GUIController.Ins.Open<HoverGUI>().SetData(hitObject);
                return;
            }
            if (hitObject.CompareTag(GameConstants.SIGN_BOARD))
            {
                HandleInteraction(hitObject, () => StoreManager.Instance.ToggleStore());
                return;
            }
            if (hitObject.CompareTag(GameConstants.PAY_COMPUTER))
            {
                HandleInteraction(hitObject, () => interactionController.SwitchCamera( 1));
                return;
            }
            if (hitObject.CompareTag(GameConstants.SHOP_COMPUTER))
            {
                HandleInteraction(hitObject, () => interactionController.SwitchCamera( 2));
                return;
            }
            if (hitObject.CompareTag(GameConstants.FISH_TANK) )
            {
                return;
            }
            
        }

        ResetInteraction();
    }


    private void HandleHeldObject()
    {
        if (interactionController.isPlacing) return;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

        bool isCollidingWithWall = false;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            if ((hitObject.CompareTag(GameConstants.FISH_TANK) || hitObject.CompareTag(GameConstants.INVERTEBRATE_TANK) || hitObject.CompareTag(GameConstants.Amphibian_Reptile_Tank)) &&
            interactionController.heldObject.CompareTag(GameConstants.FISH_TANK_BOX))
            {
                if (currentAction == null)
                {
                    HandleInteraction(hitObject, () => interactionController.AddFishToTank(hitObject));
                }
                return;
            }

            if (hitObject.CompareTag(GameConstants.TRASH_BIN) &&
                interactionController.heldObject.CompareTag(GameConstants.FISH_TANK_BOX))
            {
                if(currentAction == null) 
                {
                    HandleInteraction(hitObject, () =>
                        interactionController.ThrowTrash(interactionController.heldObject, hitObject.transform.position));
                }
                
                return;
            }

            if (hitObject.CompareTag(GameConstants.WALL) || hitObject.CompareTag(GameConstants.FISH_TANK) ||
                hitObject.CompareTag(GameConstants.PAY_COMPUTER) || hitObject.CompareTag(GameConstants.SHOP_COMPUTER) )
            {
                isCollidingWithWall = true;
            }
        }

        if (isCollidingWithWall)
        {
            buttonDrop.onClick.RemoveAllListeners();
        }
        else
        {
            buttonDrop.onClick.RemoveAllListeners();
            buttonDrop.onClick.AddListener(() => interactionController.DropObject());
        }

        ResetInteraction();
    }

    private void InteractAction()
    {
        if (currentAction != null)
        {
            currentAction?.Invoke();
            currentAction = null; 
        }
    }

    private void HandleInteraction(GameObject hitObject, UnityAction action)
    {
        if (hitObject != lastHitObject || currentAction != action)
        {
            currentAction = action;
            buttonInteract.onClick.AddListener(() => currentAction.Invoke());

            SetOutline(hitObject);
        }
    }

    private void ResetInteraction()
    {
        buttonInteract.onClick.RemoveAllListeners();

        if (lastHitObject!=null && lastHitObject.CompareTag(GameConstants.FISH_TANK_BOX)) 
        {
            interactionController.ResetLastObject(lastHitObject);
            lastHitObject = null;
            GUIController.Ins.Close<HoverGUI>();
        }
        if (lastHitObject != null)
        {
            interactionController.ResetLastObject(lastHitObject);
            lastHitObject = null;
        }
        
        currentAction = null;
    }

    private void EnableOutline(GameObject obj)
    {
        QOutline outline = obj.GetComponent<QOutline>();
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green;
            outline.OutlineWidth = 10;
        }
    }
    private void SetOutline(GameObject obj)
    {
        interactionController.ResetLastObject(lastHitObject);
        lastHitObject = obj;
        EnableOutline(lastHitObject);
    }
    private bool RaycastHitsFishTank(out GameObject fishTank)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance) && (hit.collider.CompareTag(GameConstants.FISH_TANK) 
            || hit.collider.CompareTag(GameConstants.INVERTEBRATE_TANK) || hit.collider.CompareTag(GameConstants.Amphibian_Reptile_Tank)))
        {
            fishTank = hit.collider.gameObject;
            return true;
        }

        fishTank = null;
        return false;
    }

    public GameObject GetObjectUnderCrosshair()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            return hit.collider.gameObject;
        }
        return null;
    }
}




