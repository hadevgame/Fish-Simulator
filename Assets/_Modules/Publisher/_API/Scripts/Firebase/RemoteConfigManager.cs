using API.Ads;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UCExtension;
using UnityEngine;

namespace API.RemoteConfig
{
    public class RemoteConfigManager : Singleton<RemoteConfigManager>
    {
        [SerializeField] bool showLog = true;

        [SerializeField] bool autoInit = true;

        public static Action OnFetchComplete;
        public bool IsFetchedDatas { get; private set; }

        // Start is called before the first frame update
        Dictionary<string, object> defaults = new Dictionary<string, object>();

        BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        List<object> bindingTargets = new List<object>();

        private void Start()
        {
            if (autoInit)
            {
                Init();
            }
        }

        public void Init()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    InitDefaultValues();
                }
                else
                {
                    Debug.LogError("FirebaseInitFalse");
                }
            });
        }

        public void AddDefault(string key, object data)
        {
            if (!defaults.ContainsKey(key))
            {
                object value = TryGetSaveValue(key, data);
                Log($"Add default - saved value: {key} -- {value}");
                defaults.Add(key, value);
            }
        }

        private void InitDefaultValues()
        {
            string adsSetting = PlayerPrefs.GetString(AdPlayerPrefsKeys.Settings, "");
            defaults.Add(AdManager.Ins.Configs.RemoteConfigKey, adsSetting);
            foreach (var item in defaults)
            {
                Log($"Init default: {item.Key} -- {item.Value}");
            }
            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
            {
                FetchData();
            });
        }


        private void FetchData()
        {
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
            fetchTask.ContinueWithOnMainThread(FetchComplete);
        }

        private void FetchComplete(Task fetchTask)
        {
            var info = FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case LastFetchStatus.Success:
                    FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread((Task t) => SetData());
                    return;
            }
            SetData();
        }

        void SetData()
        {
            Log("===========> Firebase Fetch Complete <===========");
            foreach (var item in FirebaseRemoteConfig.DefaultInstance.AllValues)
            {
                Log($"{item.Key}: {item.Value.StringValue}");
                TrySaveValue(item.Value, item.Key);
            }
            Log("<===============================================>");
            foreach (var item in bindingTargets)
            {
                SetRemoteDatas(item);
            }
            OnFetchComplete?.Invoke();
            IsFetchedDatas = true;
        }

        void Log(string content)
        {
            if (!showLog) return;
            UCLogger.Log($"[RemoteConfigs] {content}", Color.yellow);
        }

        /// <summary>
        /// Bind before mono start function
        /// </summary>
        /// <param name="target"></param>
        public void Bind(object target)
        {
            bindingTargets.Add(target);
            List<RemoteValueAttribute> allAttributes = new List<RemoteValueAttribute>();
            foreach (var item in target.GetType().GetFields(flag))
            {
                foreach (var attr in item.GetCustomAttributes(false))
                {
                    if (attr is RemoteValueAttribute)
                    {
                        var remoteValue = attr as RemoteValueAttribute;
                        object value = TryGetSaveValue(remoteValue.Key, item.GetValue(target));
                        item.SetValue(target, value);
                        AddDefault(remoteValue.Key, item.GetValue(target));
                    }
                }
            }
            foreach (var item in target.GetType().GetProperties(flag))
            {
                foreach (var attr in item.GetCustomAttributes(false))
                {
                    if (attr is RemoteValueAttribute)
                    {
                        var remoteValue = attr as RemoteValueAttribute;
                        object value = TryGetSaveValue(remoteValue.Key, item.GetValue(target));
                        item.SetValue(target, value);
                        AddDefault(remoteValue.Key, item.GetValue(target));
                    }
                }
            }
            Init();
        }

        void SetRemoteDatas(object target)
        {
            if (target == null) return;
            foreach (var item in target.GetType().GetFields(flag))
            {
                foreach (var attr in item.GetCustomAttributes(false))
                {
                    if (attr is RemoteValueAttribute)
                    {
                        var remoteValue = attr as RemoteValueAttribute;
                        item.SetValue(target, GetValue(item.FieldType, remoteValue.Key));
                    }
                }
            }
            foreach (var item in target.GetType().GetProperties(flag))
            {
                foreach (var attr in item.GetCustomAttributes(false))
                {
                    if (attr is RemoteValueAttribute)
                    {
                        var remoteValue = attr as RemoteValueAttribute;
                        item.SetValue(target, GetValue(item.PropertyType, remoteValue.Key));
                    }
                }
            }
        }


        object GetValue(Type type, string key)
        {
            if (type.Equals(typeof(bool)))
            {
                return RemoteConfigHelper.GetBool(key);
            }
            if (type.Equals(typeof(int)))
            {
                return RemoteConfigHelper.GetInt(key);
            }
            if (type.Equals(typeof(float)))
            {
                return RemoteConfigHelper.GetFloat(key);
            }
            if (type.Equals(typeof(string)))
            {
                return RemoteConfigHelper.GetString(key);
            }
            return null;
        }

        string SaveStringKey(string key) => key + "_remote_string";

        string SaveFloatKey(string key) => key + "_remote_float";

        string SaveIntKey(string key) => key + "_remote_int";

        string SaveBoolKey(string key) => key + "_remote_bool";

        object TryGetSaveValue(string key, object defaultValue)
        {
            var type = defaultValue.GetType();
            if (type.Equals(typeof(bool)))
            {
                return PlayerPrefsExtension.GetBool(SaveBoolKey(key), (bool)defaultValue);
            }
            if (type.Equals(typeof(int)))
            {
                return PlayerPrefs.GetInt(SaveIntKey(key), (int)defaultValue);
            }
            if (type.Equals(typeof(float)))
            {
                return PlayerPrefs.GetFloat(SaveFloatKey(key), (float)defaultValue);
            }
            if (type.Equals(typeof(string)))
            {
                return PlayerPrefs.GetString(SaveStringKey(key), "");
            }
            return defaultValue;
        }
        void TrySaveValue(ConfigValue value, string key)
        {
            TrySaveString(value, key);
            TrySaveFloat(value, key);
            TrySaveInt(value, key);
            TrySaveBool(value, key);
        }

        void TrySaveString(ConfigValue value, string key)
        {
            try
            {
                PlayerPrefs.SetString(SaveStringKey(key), value.StringValue);
            }
            catch { }
        }

        void TrySaveFloat(ConfigValue value, string key)
        {
            try
            {
                PlayerPrefs.SetFloat(SaveFloatKey(key), (float)value.DoubleValue);
            }
            catch { }
        }

        void TrySaveInt(ConfigValue value, string key)
        {
            try
            {
                PlayerPrefs.SetInt(SaveIntKey(key), (int)value.LongValue);
            }
            catch { }
        }

        void TrySaveBool(ConfigValue value, string key)
        {
            try
            {
                PlayerPrefsExtension.SetBool(SaveBoolKey(key), value.BooleanValue);
            }
            catch { }
        }
    }

    public static class RemoteConfigHelper
    {
        public static float GetFloat(string key)
        {
            return (float)FirebaseRemoteConfig.DefaultInstance.GetValue(key).DoubleValue;
        }
        public static int GetInt(string key)
        {
            return (int)FirebaseRemoteConfig.DefaultInstance.GetValue(key).LongValue;
        }
        public static string GetString(string key)
        {
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue;
        }
        public static bool GetBool(string key)
        {
            return FirebaseRemoteConfig.DefaultInstance.GetValue(key).BooleanValue;
        }
    }

    /// <summary>
    /// Support type of bool, string, int, float.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RemoteValueAttribute : Attribute
    {
        public string Key { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Remote key</param>
        public RemoteValueAttribute(string key)
        {
            Key = key;
        }
    }

}