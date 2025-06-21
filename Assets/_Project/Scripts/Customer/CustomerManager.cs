using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UCExtension.Audio;
using UCExtension.GUI;
using UnityEngine;

public class CustomerManager : Singleton<CustomerManager>
{
    [SerializeField] private List<GameObject> customerList = new List<GameObject>();
    [SerializeField] private TankManager tankManager;
    public List<GameObject> customerPrefabs;
    [SerializeField] private int poolSize = 8;
    private Queue<GameObject> customerPool = new Queue<GameObject>();

    [Header("Spawn Point Groups")]
    [SerializeField] private List<Transform> entryPoints = new List<Transform>();
    [SerializeField] private List<Transform> roamingPoints = new List<Transform>();

    private int entryIndex = 0;
    private int roamingIndex = 0;

    public static Action<GameObject> OnReleaseCustomerEvent = null;
    public static Func<Transform> OnExitShop;

    private float spawnTime;
    private enum SpawnMode { Entry, Roaming }
    private SpawnMode currentMode = SpawnMode.Entry;
    private int spawnCounter = 0;
    private const int ENTRY_LIMIT = 5;
    private const int ROAMING_LIMIT = 3;
    private void Start()
    {
        InitializePool();
        OnReleaseCustomerEvent = ReturnCustomerToPool;
        StartCoroutine(SpawnCustomersRoutine());
        OnExitShop = () => roamingPoints.RandomElement();
    }
    private void OnDisable()
    {
        OnReleaseCustomerEvent = null;
    }
    private void InitializePool()
    {
        if (customerPrefabs.Count == 0) return;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject randomPrefab = customerPrefabs[UnityEngine.Random.Range(0, customerPrefabs.Count)];
            GameObject customer = Instantiate(randomPrefab);
            customer.SetActive(false);
            customerPool.Enqueue(customer);
            customer.transform.SetParent(gameObject.transform);
        }
    }
    private GameObject GetCustomerFromPool()
    {
        if (customerPool.Count > 0)
        {
            GameObject customer = customerPool.Dequeue();
            customer.SetActive(true);
            return customer;
        }
        else
        {
            GameObject randomPrefab = customerPrefabs[UnityEngine.Random.Range(0, customerPrefabs.Count)];
            return Instantiate(randomPrefab);
        }
    }
    private IEnumerator SpawnCustomersRoutine()
    {
        while (true)
        {
            if (ShopLevelManager.Ins.CurrentLevel <= 3) spawnTime = 4f;
            else if (ShopLevelManager.Ins.CurrentLevel <= 7) spawnTime = 3.5f;
            else spawnTime = 2.5f;

            yield return new WaitForSeconds(spawnTime);
            SpawnCustomer();
        }
    }
    private void SpawnCustomer()
    {
        if (customerList.Count >= 15) return;

        List<Transform> pointList = currentMode == SpawnMode.Entry ? entryPoints : roamingPoints;
        if (pointList.Count < 2) return;

        GameObject newCustomer = GetCustomerFromPool();

        Transform spawnPoint = null;
        if (currentMode == SpawnMode.Entry)
        {
            spawnPoint = entryPoints[entryIndex];
            entryIndex = (entryIndex + 1) % entryPoints.Count;
        }
        else
        {
            spawnPoint = roamingPoints[roamingIndex];
            roamingIndex = (roamingIndex + 1) % roamingPoints.Count;
        }

        newCustomer.transform.position = spawnPoint.position;
        newCustomer.transform.rotation = Quaternion.identity;
        
        CustomerStateMachine customerStateMachine = newCustomer.GetComponent<CustomerStateMachine>();

        if (customerStateMachine != null)
        {
            List<Transform> validDestinations = new List<Transform>(pointList);
            validDestinations.Remove(spawnPoint);

            Transform destination = validDestinations[UnityEngine.Random.Range(0, validDestinations.Count)];
            customerStateMachine.SetPath(new List<Transform> { destination });
            customerStateMachine.isValid = false;

            customerList.Add(newCustomer);

            if (currentMode == SpawnMode.Entry)
            {
                StartCoroutine(CheckStoreStatus(customerStateMachine));
            }
        }

        spawnCounter++;
        if (currentMode == SpawnMode.Entry && spawnCounter >= ENTRY_LIMIT)
        {
            currentMode = SpawnMode.Roaming;
            spawnCounter = 0;
        }
        else if (currentMode == SpawnMode.Roaming && spawnCounter >= ROAMING_LIMIT)
        {
            currentMode = SpawnMode.Entry;
            spawnCounter = 0;
        }
    }
    private IEnumerator CheckStoreStatus(CustomerStateMachine customer)
    {
        while (customer != null)
        {
            yield return new WaitForSeconds(0.5f);

            if (GameManager.Instance.GetState())
            {
                if (customer.IsNearStore() && customer.currentState is RoamingState)
                {
                    //Transform validTankPosition = tankManager.HasValidSlot();
                    BaseTank tank = tankManager.HasValidSlot();
                    if (tank != null)
                    {
                        customer.SetPath(new List<Transform> { tank.GetWaitPos() },tank.gameObject);
                        customer.ChangeState<WalkingState>();
                        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("ES"));
                        yield break;
                    }
                }
            }
        }
    }
    public void ReturnCustomerToPool(GameObject customer)
    {
        ResetDefault(customer);
        customerPool.Enqueue(customer);
        customerList.Remove(customer);
        customer.SetActive(false);
    }
    void ResetDefault(GameObject customer)
    {
        CustomerStateMachine customerStateMachine = customer.GetComponent<CustomerStateMachine>();
        if (customerStateMachine == null) return;

        customerStateMachine.ResetHand();
        customerStateMachine.isValid = true;
        customerStateMachine.isOrderCompleted = false;
        customerStateMachine.collectedFish = 0;
        customerStateMachine.path?.Clear();
        customerStateMachine.currentWaypointIndex = 0;
        customerStateMachine.interactTank = null;
        customerStateMachine.SetCollectedFish(0);
        customerStateMachine.animationCustomer = null;

        FishTankBox ftb = customerStateMachine.tank;
        if (ftb != null)
        {
            ftb.gameObject.layer = LayerMask.NameToLayer("Default");
            ftb.fishList.Clear();
            ftb.gameObject.SetActive(false);
        }
    }
}



