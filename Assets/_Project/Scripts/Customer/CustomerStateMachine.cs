using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using pooling;
using UCExtension;

public class CustomerStateMachine : MonoBehaviour
{
    private StateMachine<CustomerStateMachine> stateMachine;
    public string customerID;
    [Header("Core Components")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform startPos;
    public GameObject moneyObj;
    public GameObject cardObj;

    [Header("Tank")]
    public FishTankBox tank;
    public BaseTank interactTank;
    private Transform tankPos;

    [Header("Movement")]
    public List<Transform> path;
    public int currentWaypointIndex;
    public bool isMoving;
    public float waitingTime;
    public float maxWaitTime = 10f;
    public bool isValid = true;
    private bool isNearStore = false;

    [Header("Order")]
    public CustomerOrder order;
    public int collectedFish = 0;
    public bool isOrderCompleted = false;

    public AnimationCustomer animationCustomer;
    public BaseState<CustomerStateMachine> previousState;
    public BaseState<CustomerStateMachine> currentState { get; private set; }

    [SerializeField] private GameObject desTank;
    public Canvas Emo;

    private void OnEnable()
    {
        //animationCustomer = new AnimationCustomer(this.animator);
        animationCustomer  = GetComponent<AnimationCustomer>();
        animationCustomer.SetAnimator(animator);

        stateMachine = new StateMachine<CustomerStateMachine>(this, 1);
        ChangeState<IdleState>();
        order = new CustomerOrder();
        order.GenerateRandomOrder();

        collectedFish = 0;
        isOrderCompleted = false;
        desTank = null;

        if (tank != null)
            tankPos = tank.transform.parent;
    }

    private void OnDisable()
    {
        collectedFish = 0;
        isOrderCompleted = false;
    }

    private void Update()
    {
        stateMachine?.FrameUpdate();
    }

    public void ResetHand()
    {
        if (tankPos != null)
        {
            if (tankPos.childCount > 1)
            {
                Transform child = tankPos.GetChild(1);
                RecyclableObject recycle = child.GetComponent<RecyclableObject>();
                if (child != null)
                {
                    recycle.GetComponent<FishTankBox>().ClearAll();
                    PoolObjects.Ins.Destroy(recycle);
                }

            }
        }
    }
    public void SetCollectedFish(int count)
    {
        collectedFish = count;
    }

    public void ChangeState<T>() where T : BaseState<CustomerStateMachine>, new()
    {
        //previousState = stateMachine.GetCurrentState();
        previousState = currentState;
        stateMachine.ChangeState<T>();
        currentState = stateMachine.CurrentState;
    }

   
    public void SetPath(List<Transform> path, GameObject obj = null)
    {
        this.path = path;
        currentWaypointIndex = 0;

        if (obj != null) desTank = obj;

        if (path != null && path.Count > 0)
            ChangeState<RoamingState>();
    }

    public void SetQueuePosition(Transform queueSpot)
    {
        agent.SetDestination(queueSpot.position);
        isMoving = true;
    }

    public void SetStartPos(Transform spawnPoint)
    {
        startPos = spawnPoint;
       // transform.position = spawnPoint.position;
    }

    public void MoveToNextWaypoint()
    {
        if (path == null || path.Count == 0) return;
        agent.SetDestination(path[currentWaypointIndex].position);
        isMoving = true;
    }

    public void MoveToExit()
    {
        path.Clear();
        path.Add(startPos);
        currentWaypointIndex = 0;

        if (currentState is WaitingForProductState)
        {
            animationCustomer.SetMovement(AnimationCustomer.MovementType.Walking);
        }
        else if (currentState is TakingState)
        {
            animationCustomer.SetAction(AnimationCustomer.ActionType.Carrying);
        }

        MoveToNextWaypoint();
    }

    public bool IsProductAvailable()
    {
        return interactTank != null && interactTank.IsHasFish();
    }

    public void SetStateSlot()
    {
        interactTank.isValidSlot = true;
    }

    public bool IsNearStore() => isNearStore;

    //public void HandleTouchMoney()
    //{
    //    GameObject target = order.isCard ? cardObj : moneyObj;
    //    if (target == null || !target.activeSelf) return;

    //    if (Input.touchCount > 0)
    //    {
    //        Touch touch = Input.GetTouch(0);
    //        if (touch.phase == TouchPhase.Began)
    //        {
    //            Ray ray = Camera.main.ScreenPointToRay(touch.position);
    //            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == target)
    //            {
    //                Debug.Log("Player collected money!");
    //                //moneyObj.SetActive(false);
    //                target.SetActive(false);
    //                animationCustomer.Deaction(AnimationCustomer.ActionType.Giving);
    //                ChangeState<IdleState>();
    //                CheckOutController.Ins.PayMent(order.payMoney, order.fishCount, order.isCard);
    //            }
    //        }
    //    }
    //}
    public void HandleTouchMoney()
    {
        GameObject target = order.isCard ? cardObj : moneyObj;
        if (target == null || !target.activeSelf) return;

        Vector2 inputPosition = Vector2.zero;
        bool isInput = false;

#if UNITY_EDITOR 
        if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Input.mousePosition;
            isInput = true;
        }
#else
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            inputPosition = touch.position;
            isInput = true;
        }
    }
#endif

        if (isInput)
        {
            Ray ray = Camera.main.ScreenPointToRay(inputPosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == target)
            {
                Debug.Log("Player collected money!");
                target.SetActive(false);
                animationCustomer.Deaction(AnimationCustomer.ActionType.Giving);
                ChangeState<IdleState>();
                CheckOutController.Ins.PayMent(order.payMoney, order.fishCount, order.isCard);
            }
        }
    }
    public void DeCash()
    {
        // Set lại parent và vị trí ban đầu
        tank.transform.SetParent(tankPos);
        tank.transform.localPosition = new Vector3(-0.0170000009f, 0.178000003f, 0.0140000004f);
        tank.transform.localRotation = Quaternion.Euler(33.1f,18.11f,-30.16f);

        tank.gameObject.SetActive(false);
        StartCoroutine(WaitBeforeLeaving());
    }

    public void SetTankLeaving(GameObject obj) 
    {
        obj.transform.SetParent(tankPos);
        obj.transform.localPosition = new Vector3(0.0529999994f, 0.0419999994f, -0.0869999975f);
        obj.transform.localRotation = Quaternion.Euler(33.1f, 18.11f, -30.16f);
    }
    private IEnumerator WaitBeforeLeaving()
    {
        yield return new WaitForSeconds(0.5f);
        CashDesk.OnSetTankLeaving?.Invoke();
        ChangeState<LeavingState>();
        CashDesk.OnRemoveCustomerFromQueue?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (desTank == null || other.gameObject != desTank) return;

        //if (other.CompareTag(GameConstants.FISH_TANK) && other.gameObject == desTank)
        //    interactTank = other.GetComponent<FishTank>();
        //else if (other.CompareTag(GameConstants.INVERTEBRATE_TANK) && other.gameObject == desTank)
        //    interactTank = other.GetComponent<InvertebrateTank>();
        //else if (other.CompareTag(GameConstants.Amphibian_Reptile_Tank) && other.gameObject == desTank)
        //    interactTank = other.GetComponent<Amphibian_ReptileTank>();
        //else if (other.CompareTag(GameConstants.STORE)) 
        //{
        //    isNearStore = true;
        //    Debug.Log("near");
        //}
        if (other.CompareTag(GameConstants.STORE))
        {
            isNearStore = true;
            return;
        }

        // Nếu không phải bể đích thì bỏ qua
        if (desTank == null || other.gameObject != desTank) return;

        // Đúng là bể đích, gán chính xác loại bể
        if (other.CompareTag(GameConstants.FISH_TANK))
            interactTank = other.GetComponent<FishTank>();
        else if (other.CompareTag(GameConstants.INVERTEBRATE_TANK))
            interactTank = other.GetComponent<InvertebrateTank>();
        else if (other.CompareTag(GameConstants.Amphibian_Reptile_Tank))
            interactTank = other.GetComponent<Amphibian_ReptileTank>();
    }

    private void OnTriggerExit(Collider other)
    {
        //if (other.CompareTag(GameConstants.FISH_TANK) ||
        //    other.CompareTag(GameConstants.INVERTEBRATE_TANK) ||
        //    other.CompareTag(GameConstants.Amphibian_Reptile_Tank))
        //{
        //    interactTank = null;
        //}
        if (interactTank != null && other.gameObject == interactTank.gameObject)
        {
            interactTank = null;
        }
        if (other.CompareTag(GameConstants.STORE))
        {
            isNearStore = false;
        }
    }
}
