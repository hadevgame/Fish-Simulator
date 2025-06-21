using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UCExtension;
using UCExtension.Audio;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.UI.GridLayoutGroup;

public class CashDesk : MonoBehaviour
{
    [SerializeField] private List<Transform> queuePositions = new List<Transform>(); // Danh sách vị trí trong hàng đợi
    private Queue<CustomerStateMachine> waitingCustomers = new Queue<CustomerStateMachine>(); // Danh sách khách đang chờ
    private List<CustomerStateMachine> waitingList = new List<CustomerStateMachine>();
    private List<CustomerStateMachine> buyingList = new List<CustomerStateMachine> ();

    public static Action<CustomerStateMachine> OnCustomerReadyToQueue = null; // Event khi khách hàng mua xong
    public static Action<CustomerStateMachine> OnRemoveCustomerFromQueue = null;
    public static Func<CustomerStateMachine, bool> OnCheckFirstCustomer = null;
    public static Func<bool> OnCheckQueue = null;
    public static Action OnSetTankLeaving = null;
    public static Action<CustomerStateMachine> OnAddBuyList = null;
    public static Action<CustomerStateMachine> OnRemoveBuyList = null;

    [SerializeField] private GameObject cashItemSlot;
    [SerializeField] private GameObject endCashSlot;
    [SerializeField] private RecyclableObject minitankPrefab;
    [SerializeField] private RecyclableObject currentTank;
    [SerializeField] private Transform tankPoint;

    private bool canclick = false;
    private int clickedFishCount = 0;
    public static CashDesk Instance { get; private set; }
    private int sastisfiedcus ;
    public int SastisfiedCus => sastisfiedcus;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnTank();
        OnCustomerReadyToQueue = HandleCustomerBuying;
        OnRemoveCustomerFromQueue = RemoveCustomerFromQueue;
        OnCheckFirstCustomer = IsCustomerFirstInQueue;
        OnCheckQueue = IsQueueEmpty;
        OnSetTankLeaving = HandleCusLeaving;
        OnAddBuyList = AddBuyList; 
        OnRemoveBuyList = RemoveBuyList;
    }

    private void OnDisable()
    {
        OnCustomerReadyToQueue = null;
        OnRemoveCustomerFromQueue = null;
        OnCheckFirstCustomer = null;
        OnCheckQueue = null;
        OnSetTankLeaving= null;
        OnAddBuyList = null;
        OnRemoveBuyList = null;
    }

    public void AddBuyList(CustomerStateMachine cus) 
    {
        buyingList.Add(cus);   
    }

    public void RemoveBuyList(CustomerStateMachine cus) 
    {
        buyingList.Remove(cus);
    }
    public void SetSastisfied() 
    {
        sastisfiedcus = 0;
    }
    private void Update()
    {
        UpdateCanClick();
        if (canclick)
        {
            HandleTouchFish();
        }
    }

    void SpawnTank() 
    {
        currentTank = PoolObjects.Ins.Spawn(minitankPrefab);
        currentTank.transform.position = tankPoint.transform.position;
    }
    private void UpdateCanClick()
    {
        if (CheckOutController.Ins == null)
        {
            canclick = false;
            return;
        }

        canclick = CheckOutController.Ins.IsActive;
    }
    public void Cash(FishTankBox tank)
    {
        tank.transform.SetParent(null);
        float moveDuration = 0.5f;
        tank.transform.DOMove(cashItemSlot.transform.position, moveDuration).SetEase(Ease.InOutQuad)
             .OnComplete(() =>
             {
                 tank.transform.rotation = Quaternion.identity;
                 tank.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                 HighlightFishInTank(tank);
             });
    }

    private void HighlightFishInTank(FishTankBox tank)
    {
        foreach (var fish in tank.GetAllFish()) 
        {
            var highlight = fish.GetComponent<QOutline>(); 
            if (highlight != null)
            {
                highlight.enabled = true; 
            }
        }
    }
    
    public void HandleTouchFish()
    {
        if (!canclick) return;

        Vector2 inputPosition = Vector2.zero;
        bool isInput = false;

#if UNITY_EDITOR || UNITY_STANDALONE
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
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                var highlight = clickedObject.GetComponent<QOutline>();
                if (highlight != null && highlight.enabled)
                {
                    FishTankBox tank = clickedObject.GetComponentInParent<FishTankBox>();
                    if (tank != null)
                    {
                        CheckOutController.Ins.ToggleButton(false);
                        MoveFishToCash(clickedObject, tank);
                        TotalUI.OnUpdateScene?.Invoke(clickedObject.GetComponent<Fish>());
                    }
                }
            }
        }
    }
    private void MoveFishToCash(GameObject fish, FishTankBox tank)
    {
        Vector3 end = endCashSlot.transform.position;
        CustomerStateMachine cus = GetFirstCustomerInQueue();
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("SCAN"));
        //fish.GetComponent<FishSwim>().enabled = false;
        if (fish.TryGetComponent<FishSwim>(out var swim))
        {
            swim.enabled = false;
        }
        fish.transform.DOJump(end, 0.5f, 1, 0.6f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                var highlight = fish.GetComponent<QOutline>();
                if (highlight != null)
                {
                    highlight.enabled = false;
                    currentTank.GetComponent<FishTankBox>().fishList.Add(fish);
                    fish.transform.SetParent(currentTank.transform);
                    //fish.gameObject.SetActive(false);
                }
                clickedFishCount++;
                if (clickedFishCount >= cus.collectedFish)
                {
                    cus.ChangeState<GivingState>();
                    clickedFishCount = 0;
                    
                }
            });
        Debug.Log("Fish clicked and moved to cashier.");
    }
    public bool HasSpace()
    {
        return waitingCustomers.Count < queuePositions.Count;
    }

    public bool IsCustomerFirstInQueue(CustomerStateMachine customer)
    {
        return waitingCustomers.Count > 0 && waitingCustomers.Peek() == customer;
    }

    public void AddCustomerToQueue(CustomerStateMachine customer)
    {
        if (HasSpace())
        {
            Transform queueSpot = queuePositions[waitingCustomers.Count]; 
            waitingCustomers.Enqueue(customer); // Thêm khách vào danh sách hàng đợi
            customer.SetQueuePosition(queueSpot); // Di chuyển khách hàng đến vị trí hàng đợi
            //customer.ChangeState<CashingState>(); // Đổi trạng thái sang CashingState
        }
        else
        {
            Debug.Log("Hàng đợi đã đầy!");
        }
    }

    public void RemoveCustomerFromQueue(CustomerStateMachine customer)
    {
        if (waitingCustomers.Count > 0 && waitingCustomers.Peek() == customer)
        {
            waitingCustomers.Dequeue(); // Xóa khách đầu khỏi hàng đợi
        }
        sastisfiedcus++;
        
        UpdateQueuePositions();
    }

    private void UpdateQueuePositions()
    {
        CustomerStateMachine[] customers = waitingCustomers.ToArray();

        waitingCustomers.Clear();
        for (int i = 0; i < customers.Length; i++)
        {
            customers[i].SetQueuePosition(queuePositions[i]); // Cập nhật vị trí mới cho khách
            waitingCustomers.Enqueue(customers[i]); // Thêm lại vào hàng đợi
            customers[i].ChangeState<GoToCashingState>(); 
        }

        while (waitingCustomers.Count < queuePositions.Count && waitingList.Count > 0)
        {
            CustomerStateMachine nextCustomer = waitingList[0];
            waitingList.RemoveAt(0);
            AddCustomerToQueue(nextCustomer);
            nextCustomer.ChangeState<GoToCashingState>(); 
            nextCustomer.SetStateSlot(); // Cập nhật lại slot của khách
        }
    }

    public CustomerStateMachine GetFirstCustomerInQueue()
    {
        return waitingCustomers.Count > 0 ? waitingCustomers.Peek() : null;
    }

    public void HandleCustomerBuying(CustomerStateMachine customer)
    {
        if (HasSpace())
        {
            AddCustomerToQueue(customer);
            customer.ChangeState<CashingState>(); // Đổi trạng thái sang CashingState
            customer.SetStateSlot();

        }
        else
        {
            waitingList.Add(customer); 
        }
    }

    public void HandleCusLeaving() 
    {
        CustomerStateMachine cus = GetFirstCustomerInQueue();
        cus.SetTankLeaving(currentTank.gameObject);
        currentTank = null;
        SpawnTank();
    }
    public bool IsQueueEmpty()
    {
        return waitingCustomers.Count == 0;
    }
    public void SaveCustomerQueueData()
    {
        PlayerPrefs.SetInt("Sastisfies", sastisfiedcus);
        int index = 0;
        foreach (var customer in waitingCustomers)
        {
            SaveCustomerData(index++, customer, 0); // 0 là chỉ số cho waitingCustomers
        }
        foreach (var customer in waitingList)
        {
            SaveCustomerData(index++, customer, 1); // 1 là chỉ số cho waitingList
        }
        foreach (var customer in buyingList)
        {
            SaveCustomerData(index++, customer, 2); 
        }
        PlayerPrefs.SetInt("CustomerQueueCount", index);
        PlayerPrefs.Save(); 
    }

    private void SaveCustomerData(int index, CustomerStateMachine customer, int queueType)
    {
        string id = customer.customerID;
        Vector3 position = customer.transform.position;
        float rotationY = customer.transform.eulerAngles.y;
        string state = customer.currentState?.GetType().Name ?? "Unknown";

        List<int> fishIDs = new List<int>();
        if (customer.tank != null)
        {
            foreach (var fish in customer.tank.fishList)
            {
                int fishID = fish.GetComponent<Fish>().data.fishID; // Lấy ID cá
                fishIDs.Add(fishID);
            }
        }
        string fishData = string.Join(",", fishIDs); // Lưu danh sách ID cá dưới dạng chuỗi

        // Lưu dữ liệu khách hàng bao gồm cả thông tin cá
        string data = $"{id}|{position.x}|{position.y}|{position.z}|{rotationY}|{state}|{fishData}";
        PlayerPrefs.SetString($"Customer_{queueType}_{index}", data);
    }

    public void LoadCustomerQueueData()
    {
        int sas = PlayerPrefs.GetInt("Sastisfies", 0);
        sastisfiedcus = sas;
        int customerCount = PlayerPrefs.GetInt("CustomerQueueCount", 0);
        waitingCustomers.Clear();
        waitingList.Clear();

        for (int i = 0; i < customerCount; i++)
        {
            string waitingData = PlayerPrefs.GetString($"Customer_0_{i}");
            string waitingListData = PlayerPrefs.GetString($"Customer_1_{i}");
            string buyingListData = PlayerPrefs.GetString($"Customer_2_{i}");

            if (!string.IsNullOrEmpty(waitingData))
            {
                LoadCustomerFromData(waitingData, true,0); // Thêm vào waitingCustomers
            }
            if (!string.IsNullOrEmpty(waitingListData))
            {
                LoadCustomerFromData(waitingListData, false, 1); // Thêm vào waitingList
            }
            if (!string.IsNullOrEmpty(buyingListData))
            {
                LoadCustomerFromData(buyingListData, false,2); // Thêm vào waitingList
            }
            else Debug.Log("null");
        }
    }

    private void LoadCustomerFromData(string customerData, bool isInQueue, int index)
    {
        string[] data = customerData.Split('|');
        if (data.Length < 7) return;

        string customerID = data[0];
        Vector3 position = new Vector3(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));
        float rotationY = float.Parse(data[4]);
        string state = data[5];

        List<int> fishIDs = new List<int>();
        if (data.Length > 6)
        {
            string[] fishDataArray = data[6].Split(','); // Phân tách chuỗi ID cá
            foreach (string fishIDStr in fishDataArray)
            {
                if (int.TryParse(fishIDStr, out int fishID))
                {
                    fishIDs.Add(fishID);
                }
            }
        }

        GameObject customerPrefab = CustomerManager.Ins.customerPrefabs.Find(prefab =>
            prefab.GetComponent<CustomerStateMachine>().customerID == customerID);

        if (customerPrefab != null)
        {
            GameObject customerObject = Instantiate(customerPrefab, position, Quaternion.Euler(0, rotationY, 0));
            CustomerStateMachine customerStateMachine = customerObject.GetComponent<CustomerStateMachine>();

            switch (state)
            {
                case "CashingState":
                    customerStateMachine.ChangeState<CashingState>();
                    customerStateMachine.tank.gameObject.SetActive(true);
                    Cash(customerStateMachine.tank);
                    break;
                case "WaitingForCashingState":
                    customerStateMachine.ChangeState<WaitingForCashingState>();
                    break;
                case "GivingState":
                    customerStateMachine.ChangeState<CashingState>();
                    customerStateMachine.tank.gameObject.SetActive(true);
                    Cash(customerStateMachine.tank);
                    break;
                case "BuyingState":
                    customerStateMachine.ChangeState<BuyingState>();
                    break;
                case "GoToCashingState":
                    AddCustomerToQueue(customerStateMachine);
                    customerStateMachine.ChangeState<GoToCashingState>();
                    customerStateMachine.tank.gameObject.SetActive(true);
                    break;
                case "IdleState":
                    customerStateMachine.ChangeState<CashingState>();
                    customerStateMachine.tank.gameObject.SetActive(true);
                    Cash(customerStateMachine.tank);
                    break;

            }

            if (customerStateMachine.tank != null && fishIDs.Count > 0)
            {
                StartCoroutine(DelayAddFish(customerStateMachine, fishIDs, 2f));
            }

            if (isInQueue)
            {
                if(!(customerStateMachine.currentState is GoToCashingState))
                    waitingCustomers.Enqueue(customerStateMachine);
            }
            if(!isInQueue && index == 1)
            {
                waitingList.Add(customerStateMachine);
                StartCoroutine(DelaySetSlot(customerStateMachine));
            }
            if (!isInQueue && index == 2)
            {
                buyingList.Add(customerStateMachine);
                StartCoroutine(DelaySetSlot(customerStateMachine));
            }
        }
        else Debug.Log("null");
    }

    private IEnumerator DelaySetSlot(CustomerStateMachine cus) 
    {
        yield return new WaitForSeconds(1f);
        if (cus.interactTank != null)
        {
            cus.interactTank.isValidSlot = false;
        }
    }
    private IEnumerator DelayAddFish(CustomerStateMachine customerStateMachine, List<int> fishIDs, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        foreach (int fishID in fishIDs)
        {
            FishData fishData = FishDatabase.Instance.GetFishByID(fishID);
            if (fishData != null)
            {
                GameObject fishObject = Instantiate(fishData.fishPrefab, customerStateMachine.tank.GetAddPos().position, Quaternion.identity);
                fishObject.transform.localScale = fishObject.transform.localScale * 2;
                customerStateMachine.tank.AddFish(fishObject);
            }
        }
        if (IsCustomerFirstInQueue(customerStateMachine)) 
            HighlightFishInTank(customerStateMachine.tank);

        customerStateMachine.order.SetFishCount(fishIDs.Count);
        customerStateMachine.SetCollectedFish(fishIDs.Count);
    }


}



