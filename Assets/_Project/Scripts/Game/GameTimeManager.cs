using API.LogEvent;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeManager : MonoBehaviour
{
    public static GameTimeManager Instance;

    [Header("Thời gian bắt đầu trong game")]
    [SerializeField] private int startHour = 9; 

    [Header("Tổng thời gian 1 ngày (phút thực tế)")]
    [SerializeField] private float realMinutesPerDay = 10f;

    [Header("Text hiển thị thời gian")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;

    private float timer = 0f;
    private bool isRunning = false;
    private DateTime gameTime;
    private int currentDay = 1;
    public int CurrentDay => currentDay;
    private bool isEndday = false;
     
    private const string TimeKey = "GameTime";
    private const string TimerKey = "GameTimer";
    private const string DayKey = "CurrentDay";
    private const string IsEndDayKey = "IsEndDay";

    [SerializeField] private Button buttonEnd;
    [SerializeField] private GameObject panelDailyStats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            buttonEnd.onClick.AddListener(TurnOnStatistical);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime;

            float percentOfDay = timer / (realMinutesPerDay * 60f);
            int totalGameMinutes = (int)(percentOfDay * 12f * 60f);

            gameTime = new DateTime(1, 1, 1, startHour, 0, 0).AddMinutes(totalGameMinutes);

            UpdateUI();

            if (percentOfDay >= 1f)
            {
                EndDay(); 
            }
        }

        if (isEndday)
        {
            bool validall = CheckSlotAll();
            if (CashDesk.OnCheckQueue?.Invoke() == true && validall)
            {
                buttonEnd.gameObject.SetActive(true);
            }

        }
    }
    void InitTime()
    {
        timer = 0f;
        gameTime = new DateTime(1, 1, 1, startHour, 0, 0);
        UpdateUI();
    }
    void UpdateUI()
    {
        if (timeText != null)
        {
            timeText.text = gameTime.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            dayText.text = currentDay.ToString();
        }
    }
    public void StartTime()
    {
        isRunning = true;
    }
    public void PauseTime()
    {
        isRunning = false;
    }
    void EndDay()
    {
        isRunning = false;  
        isEndday = true;
        StoreManager.OnCloseStore?.Invoke();
        Debug.Log("Kết thúc ngày!");
    }
    private bool CheckSlotAll()
    {
        foreach (GameObject tank in TankManager.Instance.tanks)
        {
            BaseTank baseTank = tank.GetComponent<BaseTank>();
            if (baseTank != null && !baseTank.isValidSlot)
            {
                return false;
            }
        }
        return true;
    }
    public void TurnOnStatistical()
    {
        buttonEnd.gameObject.SetActive(false);
        panelDailyStats.SetActive(true);
    }
    public DateTime GetGameTime()
    {
        return gameTime;
    }
    public float GetTimer() 
    {
        return timer;
    }
    public void SaveGameTime()
    {
        PlayerPrefs.SetString(TimeKey, gameTime.ToString());
        PlayerPrefs.SetFloat(TimerKey, timer);
        PlayerPrefs.SetInt(DayKey,currentDay);
        PlayerPrefs.SetInt(IsEndDayKey, isEndday ? 1 : 0);
        PlayerPrefs.Save();
    }
    public void LoadGameTime()
    {
        if (PlayerPrefs.HasKey(TimeKey))
        {
            string savedTime = PlayerPrefs.GetString(TimeKey);
            gameTime = DateTime.Parse(savedTime);

            if (PlayerPrefs.HasKey(TimerKey))
            {
                timer = PlayerPrefs.GetFloat(TimerKey); 
                currentDay = PlayerPrefs.GetInt(DayKey);
                isEndday = PlayerPrefs.GetInt(IsEndDayKey, 0) == 1;
            }
            else
            {
                timer = 0f; 
            }
            UpdateUI();
            if (StoreManager.Instance.GetState())
            {
                isRunning = true;
            }

        }
        else
        {
            InitTime();  
        }
    }
    public void StartNewDay()
    {
        StoreManager.Instance.Setstate();
        currentDay++;
        timer = 0f;
        gameTime = new DateTime(1, 1, 1, startHour, 0, 0);
        isRunning = false;
        isEndday = false;
        
        UpdateUI();
        buttonEnd.gameObject.SetActive(false);
    }
}

