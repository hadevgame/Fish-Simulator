//using System;
//using System.Collections.Generic;
//using System.Linq;
//using API.RemoteConfig;
//using JsonFx.Json;
//using Sirenix.OdinInspector;
//using UnityEditor;
//using UnityEngine;

//public class AlarmManager : MonoBehaviour
//{
//    public static AlarmManager Ins;

//    public ListNotificationSetting NotificationSettings;

//    private void Awake()
//    {
//        if (Ins == null)
//        {
//            Ins = this;
//            DontDestroyOnLoad(transform.root.gameObject);
//        }
//        else if (Ins != this)
//        {
//            Destroy(transform.root.gameObject);
//        }
//    }

//    private void Start()
//    {
//        RegisterAlarm();
//        RemoteConfigManager.OnFetchComplete += OnFirebaseInitComplete;
//    }

//    private void OnFirebaseInitComplete()
//    {
//        string data = ApiRemoteData.Ins.NotifySettings;
//        data = data.CheckCorrect();
//        if (string.IsNullOrEmpty(data))
//        {
//            NotificationSettings = JsonUtility.FromJson<ListNotificationSetting>(data);
//        }
//        RegisterAlarm();
//    }

//    private void RegisterAlarm()
//    {
//        AllUnRegister();
//        RegisterDailyAlarm();
//    }

//    private void RegisterDailyAlarm()
//    {
//        string title = Application.productName;
//        List<string> noti = new List<string>();
//        for (int i = 0; i < NotificationSettings.settings.Count; i++)
//        {
//            if (i % 2 == 0)
//            {
//                noti.Add("⭐ " + NotificationSettings.settings[i].NotificationString + " ⭐");
//            }
//            else
//            {
//                noti.Add("⏰ " + NotificationSettings.settings[i].NotificationString + " ⏰");
//            }
//        }
//        for (int i = 0; i < 31; i++)
//        {
//            for (int j = 0; j < NotificationSettings.settings.Count; j++)
//            {
//                if (PlayerPrefs.HasKey("DailyAlarm_" + i + "_" + NotificationSettings.settings[j].Hour + "_" + NotificationSettings.settings[j].Minute))
//                {
//                    if (UnityEngine.Random.Range(0, 2) == 1)
//                    {
//                        Alarm.Unregister("DailyAlarm_" + i + "_" + NotificationSettings.settings[j].Hour + "_" + NotificationSettings.settings[j].Minute);
//                        Alarm.Register("DailyAlarm_" + i + "_" + NotificationSettings.settings[j].Hour + "_" + NotificationSettings.settings[j].Minute, title, noti[j], GetFireTickAfterDays(i, NotificationSettings.settings[j].Hour, NotificationSettings.settings[j].Minute));
//                    }
//                }
//                else
//                {
//                    Alarm.Register("DailyAlarm_" + i + "_" + NotificationSettings.settings[j].Hour + "_" + NotificationSettings.settings[j].Minute, title, noti[j], GetFireTickAfterDays(i, NotificationSettings.settings[j].Hour, NotificationSettings.settings[j].Minute));
//                }

//            }

//        }
//    }

//    private static DateTime GetFireTickAfterDays(int days, int hour, int minute)
//    {
//        DateTime dateTime = DateTime.Now.AddDays(days);
//        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, minute, 0);
//    }

//    private void AllUnRegister()
//    {
//        Alarm.AllUnregister();
//        for (int i = 0; i < 31; i++)
//        {
//            for (int j = 0; j < NotificationSettings.settings.Count; j++)
//                if (PlayerPrefs.HasKey("DailyAlarm_" + i + "_" + j))
//                {
//                    PlayerPrefs.DeleteKey("DailyAlarm_" + i + "_" + j);
//                }
//        }

//    }

//#if UNITY_EDITOR
//    [Button]
//    void GetConfigJsonString()
//    {
//        string json = JsonUtility.ToJson(NotificationSettings);
//        EditorGUIUtility.systemCopyBuffer = json;
//    }
//#endif
//}

//[Serializable]
//public class ListNotificationSetting
//{
//    public List<NotificationSetting> settings;
//}


//[Serializable]
//public class NotificationSetting
//{
//    public string NotificationString;
//    public int Hour;
//    public int Minute;
//}

//public static class Alarm
//{
//    public class Entry
//    {
//        public int index;

//        public string title = string.Empty;

//        public string msg = string.Empty;

//        public DateTime date = DateTime.Now;
//    }

//    public static List<Entry> entrys;

//    private static string PREFIX;

//    public static int AlarmCount => entrys.Count;

//    static Alarm()
//    {
//        entrys = new List<Entry>();
//        PREFIX = "ALARM_";
//        Init();
//    }

//    public static void Init()
//    {
//        AlarmAndroid.Init();
//        if (PlayerPrefs.HasKey("Alarm"))
//        {
//            string @string = PlayerPrefs.GetString("Alarm");
//            entrys = JsonReader.Deserialize<Entry[]>(@string).ToList();
//        }
//    }

//    public static void Register(string key, string title, string message, DateTime alarmTime, bool isSoundEnabled = false)
//    {
//        TimeSpan timeSpan = alarmTime - DateTime.Now;
//        if (timeSpan.TotalHours > 0.0)
//        {
//            Register(key, title, message, timeSpan.TotalHours, isSoundEnabled);
//        }
//    }

//    public static void Register(string key, string title, string message, double intervalHours, bool isSoundEnabled = false)
//    {
//        Unregister(key);
//        key = PREFIX + key;
//        DateTime date = DateTime.Now.AddHours(intervalHours);
//        int value = RegisterInternal(title, message, date, isSoundEnabled);
//        PlayerPrefs.SetInt(key, value);
//        PlayerPrefs.Save();
//    }

//    private static int RegisterInternal(string title, string message, DateTime date, bool isSoundEnabled = false)
//    {
//        entrys.Sort((Entry a, Entry b) => a.index.CompareTo(b.index));
//        int num = 0;
//        int i;
//        for (i = 0; entrys.Count > i && entrys[i].index == i; i++)
//        {
//        }
//        num = i;
//        Entry entry = new Entry();
//        entry.index = num;
//        entry.title = title;
//        entry.msg = message;
//        entry.date = date;
//        entrys.Add(entry);
//        AlarmAndroid.Register(num, title, message, date, isSoundEnabled);
//        PlayerPrefs.SetString("Alarm", JsonWriter.Serialize(entrys));
//        return num;
//    }

//    public static void Unregister(string key)
//    {
//        key = PREFIX + key;
//        if (PlayerPrefs.HasKey(key))
//        {
//            int @int = PlayerPrefs.GetInt(key);
//            UnregisterInternal(@int);
//            PlayerPrefs.DeleteKey(key);
//            PlayerPrefs.Save();
//        }
//    }

//    private static void UnregisterInternal(int index)
//    {
//        List<Entry> unregisterEntrys = GetUnregisterEntrys(index);
//        if (unregisterEntrys != null)
//        {
//            AlarmAndroid.Unregister(unregisterEntrys, index);
//            foreach (Entry item in unregisterEntrys)
//            {
//                entrys.Remove(item);
//            }
//            PlayerPrefs.SetString("Alarm", JsonWriter.Serialize(entrys));
//        }
//    }

//    public static void AllUnregister()
//    {
//        AlarmAndroid.AllUnregister(entrys);
//        entrys.RemoveRange(0, entrys.Count);
//        PlayerPrefs.SetString("Alarm", JsonWriter.Serialize(entrys));
//    }

//    private static List<Entry> GetUnregisterEntrys(int index)
//    {
//        List<Entry> list = new List<Entry>();
//        foreach (Entry entry in entrys)
//        {
//            if (entry != null && entry.index == index)
//            {
//                list.Add(entry);
//            }
//        }
//        return list;
//    }
//}

//public static class AlarmAndroid
//{
//    public static void Init()
//    {
//    }

//    public static void Register(int index, string title, string message, DateTime date, bool isSoundEnabled = false)
//    {
//        LocalNotification.SendNotification(index, (date - DateTime.Now), title, message, Color.white);
//    }

//    public static void Unregister(List<Alarm.Entry> unregisterEntrys, int index)
//    {
//        int num = 0;
//        LocalNotification.CancelNotification(index);
//        if (num > 0)
//        {
//            UnityEngine.Debug.Log("AlarmUnregister failCnt : " + num);
//        }
//    }

//    public static void AllUnregister(List<Alarm.Entry> entrys)
//    {
//        int num = 0;
//        LocalNotification.ClearNotifications();
//        if (num > 0)
//        {
//            UnityEngine.Debug.Log("AllUnregister failCnt : " + num);
//        }
//    }
//}
