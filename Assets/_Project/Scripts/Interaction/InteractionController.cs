using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;
using UCExtension.GUI;
using UCExtension;
using System;
using System.Collections;
using UCExtension.Audio;
using System.Data;

public class InteractionController : Singleton<InteractionController>
{
    public GameObject heldObject;
    public Transform handPosition;
    [SerializeField] private Transform dropPosition;
    public bool isHeld, isPlacing, isValidPlacing = true, candrop = false;

    private bool isTrashing = false;
    private Vector3 lastValidPosition;
    private Quaternion placingRotation;
    private Vector3 pos;
    private Quaternion rot;
    private Material[] originalMaterials;
    
    public static Action<bool> UpdatePlacingState;
    public static Action<bool> UpdateInteractState;

    //[SerializeField] private PlayGUI playUIManager;
    [SerializeField] private Material placingMaterial, invalidPlacingMaterial;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject checkOutCanvas/*, shopCanvas*/;
    [SerializeField] private Transform checkoutPoint;
    [SerializeField] private Transform shopPoint;
    private void Update()
    {
        if (isPlacing && heldObject != null)
        {
            UpdatePlacingPosition();
        }
        if (heldObject == null) candrop = false;
    }
    public void StartPlacingMode()
    {
        if (heldObject == null) return;
        GameManager.OnPlacingMode?.Invoke(true);

        lastValidPosition = heldObject.transform.position;
        pos = heldObject.transform.position;
        rot = heldObject.transform.rotation;
        isPlacing = true;
        GUIController.Ins.Open<PlacingGUI>();

        if (heldObject.TryGetComponent(out Box box))
        {
            box.StartPlacing();
            heldObject = box.itemInside;
        }

        if (heldObject.TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;

        heldObject.GetComponent<Collider>().isTrigger = true;
        heldObject.transform.rotation = Quaternion.identity;
        placingRotation = heldObject.transform.rotation;

       

        if (heldObject.CompareTag(GameConstants.FISH_TANK) || heldObject.CompareTag(GameConstants.INVERTEBRATE_TANK) 
            || heldObject.CompareTag(GameConstants.Amphibian_Reptile_Tank))
        {
            TankManager.OnTankRemoved?.Invoke(heldObject);
            BaseTank baseTank = heldObject.GetComponent<BaseTank>();
            Renderer renderer = baseTank.highlightRenderer;
            originalMaterials = renderer.materials;
            SetPlacingMaterial(heldObject, placingMaterial);
        }
    }

    public void CancelPlacing()
    {
        if (heldObject == null) return;
        GameManager.OnPlacingMode?.Invoke(false);
        Vibrator.SoftVibrate();
        isPlacing = false;
        UpdatePlacingState?.Invoke(true);
        heldObject.GetComponent<Collider>().isTrigger = false;

        if (handPosition.childCount == 0) return;
        GameObject child = handPosition.GetChild(0).gameObject;
        if (child.TryGetComponent(out Box box))
        {
            if (heldObject.CompareTag(GameConstants.FISH_TANK) || heldObject.CompareTag(GameConstants.INVERTEBRATE_TANK) 
                || heldObject.CompareTag(GameConstants.Amphibian_Reptile_Tank))
            {
                box.CancelPlacing();
                RestoreOriginalMaterial();
                heldObject = child;
                return;
            }
        }

        if (heldObject.CompareTag(GameConstants.FISH_TANK) || heldObject.CompareTag(GameConstants.INVERTEBRATE_TANK) 
            || heldObject.CompareTag(GameConstants.Amphibian_Reptile_Tank))
        {
            TankManager.OnTankAdded?.Invoke(heldObject);
            heldObject.transform.SetParent(null);
            
            heldObject.transform.position = pos;
            heldObject.transform.rotation = rot;
            RestoreOriginalMaterial();
            heldObject = null;
            return;
        }
        RestoreOriginalMaterial();
    }

    public void ConfirmPlacingMode()
    {
        if (heldObject == null) return;
        GameManager.OnPlacingMode?.Invoke(false);

        isPlacing = false;
        GUIController.Ins.Close<PlacingGUI>();
        RestoreOriginalMaterial();

        heldObject.transform.SetParent(null);
        heldObject.GetComponent<Collider>().isTrigger = false;

        if (heldObject.CompareTag(GameConstants.FISH_TANK) || heldObject.CompareTag(GameConstants.INVERTEBRATE_TANK) 
            || heldObject.CompareTag(GameConstants.Amphibian_Reptile_Tank))
        {
            TankManager.OnTankAdded?.Invoke(heldObject);
        }

        heldObject = null;
        isHeld = false;
        GameObject child = handPosition.GetChild(0).gameObject;
        if (child != null)
        {
            child.SetActive(true);
            Destroy(child);
        }
    }
    private void UpdatePlacingPosition()
    {
        heldObject.transform.rotation = Quaternion.Euler(0f, heldObject.transform.rotation.eulerAngles.y, 0f);
        Vector3 newTargetPosition = lastValidPosition;
        bool foundValidGround = false;

        Ray forwardRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(forwardRay, 5f);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag(GameConstants.SHOP_COMPUTER)
                || hit.collider.CompareTag(GameConstants.PAY_COMPUTER)) continue;

            if (hit.collider.CompareTag(GameConstants.GROUND))
            {
                float objectHeight = heldObject.GetComponent<Collider>().bounds.extents.y;
                newTargetPosition = hit.point + Vector3.up * (objectHeight * 0.1f);

                Vector3 clampedPos;
                isValidPlacing = ClampPositionForShopArea(newTargetPosition, out clampedPos);
                newTargetPosition = clampedPos;
                foundValidGround = true;
                break;
            }
        }

        // Nếu ray thẳng không thấy ground, bắn thêm ray xuống
        if (!foundValidGround)
        {
            Vector3 rayEndPoint = Camera.main.transform.position + Camera.main.transform.forward * 5f;
            Ray downRay = new Ray(rayEndPoint, Vector3.down);

            if (Physics.Raycast(downRay, out RaycastHit downHit, 10f))
            {
                if (downHit.collider.CompareTag(GameConstants.GROUND))
                {
                    float objectHeight = heldObject.GetComponent<Collider>().bounds.extents.y;
                    newTargetPosition = downHit.point + Vector3.up * (objectHeight * 0.1f);

                    Vector3 clampedPos;
                    isValidPlacing = ClampPositionForShopArea(newTargetPosition, out clampedPos);
                    newTargetPosition = clampedPos;
                    foundValidGround = true;
                }
            }
        }

        if (foundValidGround)
        {
            lastValidPosition = newTargetPosition;
        }

        heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, lastValidPosition, Time.deltaTime * 15f);
        heldObject.transform.rotation = Quaternion.Lerp(heldObject.transform.rotation, placingRotation, Time.deltaTime * 5f);

        CheckPlacingValidity();
    }


    //duyet tat ca Collider va kiem tra va cham
    private void CheckPlacingValidity()
    {
        bool isValid = !heldObject.GetComponents<Collider>().Any(heldCollider => Physics.OverlapBox(heldCollider.bounds.center,heldCollider.bounds.extents,
                                                  Quaternion.identity).Any(other => other != heldCollider && !other.transform.IsChildOf(heldObject.transform)));

        SetPlacingMaterial(heldObject, isValid ? placingMaterial : invalidPlacingMaterial);
        UpdatePlacingState?.Invoke(isValid);
        UpdateInteractState?.Invoke(isValid);
    }
    private bool ClampPositionForShopArea(Vector3 targetPosition, out Vector3 clampedPosition)
    {
        clampedPosition = targetPosition;
        if (!(heldObject.CompareTag(GameConstants.FISH_TANK) ||
       heldObject.CompareTag(GameConstants.INVERTEBRATE_TANK) ||
       heldObject.CompareTag(GameConstants.Amphibian_Reptile_Tank)))
            return true;
        clampedPosition.x = Mathf.Clamp(targetPosition.x, 7f ,15f );
        clampedPosition.z = Mathf.Clamp(targetPosition.z, 5.5f, 13f);
        bool isInsideShop = (clampedPosition.x == targetPosition.x) && (clampedPosition.z == targetPosition.z) && (clampedPosition.y == targetPosition.y);

        return isInsideShop;
    }

    public void RotateLeft() => RotateObject(90);
    public void RotateRight() => RotateObject(-90);

    private void RotateObject(float angle)
    {
        if (heldObject != null && isPlacing)
        {
            Vibrator.SoftVibrate();
            placingRotation *= Quaternion.Euler(0, angle, 0);
        }
    }

    public void PickupObject(GameObject obj)
    {
        if (heldObject != null || isTrashing) return;
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("PU"));
        Vibrator.SoftVibrate();
        obj.transform.SetParent(null);
        heldObject = obj;
        heldObject.GetComponent<Rigidbody>().useGravity = false;
        heldObject.GetComponent<Collider>().enabled = false;
        isHeld = true;
        MoveObjectToHand(heldObject);
        ResetLastObject(obj);
    }

    public void DropObject()
    {
        if (heldObject == null) return;
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("DROP"));
        Vibrator.SoftVibrate();
        heldObject.transform.SetParent(null);
        if (heldObject.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            Vector3 throwDirection = (dropPosition.position - heldObject.transform.position).normalized;
            rb.AddForce(throwDirection * 3f + Vector3.up * 5f, ForceMode.Impulse);
        }

        heldObject = null;
        isHeld = false;
    }
    public void SwitchCamera( int index)
    {
        ToggleCanvasVisibility(index);
        Vibrator.SoftVibrate();
    }
    public void ToggleCanvasVisibility(int canvasIndex)
    {
        GUIController.Ins.Close<PlayGUI>();

        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.enabled = false;

        if (canvasIndex == 1)
        {
            CameraManager.Ins.ChangeCheckOutCam();

            StartCoroutine(SwitchCamera(false, checkoutPoint, 0.8f));
        }
        else if (canvasIndex == 2)
        {
            CameraManager.Ins.ChangeShopCam();
            StartCoroutine(SwitchCamera(true, shopPoint, 0.8f));
        }
    }

    private IEnumerator SwitchCamera(bool isShop, Transform targetPosition, float delay)
    {
        yield return new WaitForSeconds(delay);

        player.transform.position = targetPosition.position;
        player.transform.rotation = targetPosition.rotation;

        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.enabled = true;

        //canvas.SetActive(true);
        if (isShop) GUIController.Ins.Open<ShopGUI>().TogglePanel(true);
        else checkOutCanvas.SetActive(true);
    }
    public void AddFishToTank(GameObject obj)
    {
        if (heldObject == null || !heldObject.CompareTag(GameConstants.FISH_TANK_BOX)) return;
        BaseTank fishTank = obj.GetComponent<BaseTank>();
        Vibrator.SoftVibrate();
        heldObject.GetComponent<FishTankBox>().TakeFishFromBoxToTank(fishTank.gameObject);
    }
    public void SetPlacingMaterial(GameObject heldObject, Material newMaterial)
    {
        if (heldObject.TryGetComponent(out BaseTank baseTank) && baseTank.highlightRenderer != null)
        {
            Renderer renderer = baseTank.highlightRenderer;

            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = newMaterial;
            }

            renderer.materials = newMaterials;
        }
        else if (heldObject.TryGetComponent(out Renderer renderer))
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++) newMaterials[i] = newMaterial;
            renderer.materials = newMaterials;
        }
    }
    private void RestoreOriginalMaterial()
    {
        if (heldObject == null || originalMaterials == null) return;

        if (heldObject.TryGetComponent(out BaseTank baseTank) && baseTank.highlightRenderer != null)
        {
            baseTank.highlightRenderer.materials = originalMaterials;
        }
        else heldObject.GetComponent<Renderer>().materials = originalMaterials;
    }
    public void ThrowTrash(GameObject obj, Vector3 trashPos)
    {
        if (isTrashing) return;
        if (obj == null || !obj.TryGetComponent(out FishTankBox ftb) || ftb.fishList.Count > 0)
        {
            NotifiGUI.Instance.ShowPopup("The tank still has fish, can't throw!",Color.red);
            return;
        }
        Vibrator.SoftVibrate();
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("TRASH"));
        isTrashing = true;
        heldObject = null;
        isHeld = false;
        PurchaseManager.OnRemoveFromList?.Invoke(obj);

        float duration = 1f;
        Vector3 midPos = (obj.transform.position + trashPos) / 2 + Vector3.up * 1f;
        Vector3 endPos = new Vector3(trashPos.x, trashPos.y + 0.3f, trashPos.z);

        obj.transform.DOPath(new Vector3[] { obj.transform.position, midPos, endPos }, duration, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad);
        obj.transform.DOScale(Vector3.zero, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => {
                Destroy(obj);
                isTrashing = false;
            }); 
    }

    private void MoveObjectToHand(GameObject obj)
    {
        obj.transform.DOMove(handPosition.position, 0.5f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                obj.transform.SetParent(handPosition);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.GetComponent<Collider>().enabled = true;
                obj.GetComponent<Rigidbody>().isKinematic = true;
                candrop = true;
            });
    }

    public void ResetLastObject(GameObject obj)
    {
        if (obj == null) return;

        QOutline outline = obj.GetComponent<QOutline>();
        if (outline != null)
        {
            //outline.enabled = false;
            //playUIManager.ResetUIForNewObject();
            //GUIController.Ins.Open<PlayGUI>().ResetUIForNewObject();
            Color c = outline.OutlineColor;
            c.a = 0f; // tắt
            outline.OutlineColor = c;
        }
    }

    public void OpenSetPrice(GameObject obj) 
    {
        var priceItem = obj.transform.parent.GetComponent<PriceItemUI>();
        if (priceItem != null)
        {
            SetPriceUI.Ins.Open(priceItem.GetData());
        }
    }
}




