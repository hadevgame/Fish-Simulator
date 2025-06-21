using API.RemoteConfig;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

public class AppRemoteDatas : Singleton<AppRemoteDatas>
{
    [RemoteValue("can_play_inter")]
    public bool CanPlayInter = true;
    protected override void SetUp()
    {
        base.SetUp();
        var remoteConfig = FindObjectOfType<RemoteConfigManager>();
        if (remoteConfig)
        {
            remoteConfig.Bind(this);
        }
    }
}