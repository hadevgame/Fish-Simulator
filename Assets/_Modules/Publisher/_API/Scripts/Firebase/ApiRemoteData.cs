using API.RemoteConfig;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

public class ApiRemoteData : Singleton<ApiRemoteData>
{
    [SerializeField] RemoteConfigManager remoteManager;

    [RemoteValue("use_rate")]
    public bool UseRate = false;

    [RemoteValue("notifycation_settings")]
    public string NotifySettings;

    protected override void SetUp()
    {
        base.SetUp();
        remoteManager.Bind(this);
    }
}
