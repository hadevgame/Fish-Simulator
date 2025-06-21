using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UCExtension.Audio;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPanel : MonoBehaviour
{
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform desPosition;
    [SerializeField] private Button btnOK, btnUndo, btnClear;

    [SerializeField] private CashDesk cashDesk;

    [SerializeField] private List<Mesh> moneyMeshs;
    private int[] moneyValues = { 1, 5, 10, 20, 50 };

    [SerializeField] private List<Mesh> coinMeshs;
    private float[] coinValues = { 0.01f, 0.05f, 0.10f, 0.20f, 0.50f };

    private float yOffset = 0f;
    private float yOffsetStep = 0.005f;

    private Stack<GameObject> moneyStack = new Stack<GameObject>();

    // TÁCH 2 pool riêng
    private Queue<GameObject> moneyPool = new Queue<GameObject>();
    private Queue<GameObject> coinPool = new Queue<GameObject>();

    [SerializeField] private int poolSize = 40;
    public static Func<bool> OnCheckPayment = null;
    private void Start()
    {
        btnOK.onClick.AddListener(Payment);
        btnClear.onClick.AddListener(ClearAllMoney);
        btnUndo.onClick.AddListener(UndoLastSpawn);
        InitCoin();
        InitMoney();
        
    }

    void InitMoney() 
    {
        for (int i = 0; i < poolSize / 2; i++)
        {
            GameObject money = Instantiate(moneyPrefab);
            money.SetActive(false);
            moneyPool.Enqueue(money);
            money.transform.SetParent(this.transform);
        }
    }
    void InitCoin() 
    {
        for (int i = 0; i < poolSize / 2; i++)
        {
            GameObject coin = Instantiate(coinPrefab);
            coin.SetActive(false);
            coinPool.Enqueue(coin);
            coin.transform.SetParent(this.transform);
        }
    }
    public void Payment()
    {
        if (/*PayMentUI.OnCheckPayment?.Invoke() == true*/ OnCheckPayment?.Invoke() == true)
        {
            Vibrator.SoftVibrate();
            CustomerStateMachine payCus = cashDesk.GetFirstCustomerInQueue() as CustomerStateMachine;
            if (payCus != null)
            {
                payCus.ChangeState<TakingState>();
                Clear();
                ShopLevelManager.OnAddExp?.Invoke(5);
                
            }
        }
    }

    public void SpawnMoney(int index)
    {
        if (index < 0 || index >= moneyValues.Length) return;
        Vibrator.SoftVibrate();
        GameObject newMoney = GetFromMoneyPoolOrInstantiate();
        SetupAndFly(newMoney, moneyValues[index], moneyMeshs[index]);
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("MONEY"));
    }

    public void SpawnCoin(int index)
    {
        if (index < 0 || index >= coinValues.Length) return;
        Vibrator.SoftVibrate();
        GameObject newCoin = GetFromCoinPoolOrInstantiate();
        newCoin.transform.localScale = new Vector3(30f, 30f, 30f);
        SetupAndFly(newCoin, coinValues[index], coinMeshs[index]);
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("COIN"));
    }

    private GameObject GetFromMoneyPoolOrInstantiate()
    {
        GameObject obj = (moneyPool.Count > 0) ? moneyPool.Dequeue() : Instantiate(moneyPrefab);
        obj.SetActive(true);
        obj.transform.position = spawnPosition.position;
        obj.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        return obj;
    }

    private GameObject GetFromCoinPoolOrInstantiate()
    {
        GameObject obj = (coinPool.Count > 0) ? coinPool.Dequeue() : Instantiate(coinPrefab);
        obj.SetActive(true);
        obj.transform.position = spawnPosition.position;
        obj.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        return obj;
    }

    private void SetupAndFly(GameObject money, float value, Mesh mesh)
    {
        Money moneyScript = money.GetComponent<Money>();
        if (moneyScript != null)
        {
            moneyScript.SetMoney(value, mesh);
        }

        PayMentUI.OnUpdateGiving?.Invoke(value);
        moneyStack.Push(money);
        FlyToDestination(money);
    }

    private void FlyToDestination(GameObject money)
    {
        Vector3 startPos = spawnPosition.position;
        Vector3 endPos = desPosition.position + Vector3.up * yOffset;
        Vector3 midPos = (startPos + endPos) / 2 + Vector3.up * 0.5f;

        money.transform.DOPath(new Vector3[] { startPos, midPos, endPos }, 0.5f, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad);

        yOffset += yOffsetStep;
    }

    public void UndoLastSpawn()
    {
        if (moneyStack.Count == 0) return;
        Vibrator.SoftVibrate();
        GameObject lastMoney = moneyStack.Pop();
        yOffset -= yOffsetStep;

        float value = lastMoney.GetComponent<Money>().value;

        lastMoney.transform.DOPath(new Vector3[] { lastMoney.transform.position, spawnPosition.position }, 0.5f, PathType.Linear)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => ReturnToCorrectPool(lastMoney));

        PayMentUI.OnUpdateGiving?.Invoke(-value);
    }

    public void ClearAllMoney()
    {
        Vibrator.SoftVibrate();
        StartCoroutine(ClearMoneyRoutine());
    }

    private IEnumerator ClearMoneyRoutine()
    {
        while (moneyStack.Count > 0)
        {
            GameObject lastMoney = moneyStack.Pop();
            yOffset -= yOffsetStep;

            float value = lastMoney.GetComponent<Money>().value;

            lastMoney.transform.DOPath(new Vector3[] { lastMoney.transform.position, spawnPosition.position }, 0.3f, PathType.Linear)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => ReturnToCorrectPool(lastMoney));

            PayMentUI.OnUpdateGiving?.Invoke(-value);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Clear()
    {
        while (moneyStack.Count > 0)
        {
            GameObject lastMoney = moneyStack.Pop();
            yOffset -= yOffsetStep;
            ReturnToCorrectPool(lastMoney);
        }
    }

    private void ReturnToCorrectPool(GameObject obj)
    {
        obj.SetActive(false);
        Money moneyScript = obj.GetComponent<Money>();
        if (moneyScript != null)
        {
            if (moneyScript.isCoin)
                coinPool.Enqueue(obj);
            else
                moneyPool.Enqueue(obj);
        }
    }

    // Các hàm gán vào nút bấm
    public void OnClick_SpawnCoin001() => SpawnCoin(0);
    public void OnClick_SpawnCoin005() => SpawnCoin(1);
    public void OnClick_SpawnCoin010() => SpawnCoin(2);
    public void OnClick_SpawnCoin020() => SpawnCoin(3);
    public void OnClick_SpawnCoin050() => SpawnCoin(4);

    public void OnClick_Spawn1() => SpawnMoney(0);
    public void OnClick_Spawn5() => SpawnMoney(1);
    public void OnClick_Spawn10() => SpawnMoney(2);
    public void OnClick_Spawn20() => SpawnMoney(3);
    public void OnClick_Spawn50() => SpawnMoney(4);
}

