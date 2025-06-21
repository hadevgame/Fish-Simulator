using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UCExtension;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera shopCam;
    [SerializeField] private CinemachineVirtualCamera checkoutCam;
    [SerializeField] private CinemachineVirtualCamera playerCam;

    public GameObject GetCam(int index) 
    {
        if (index == 1) return playerCam.gameObject;
        if(index == 2) return shopCam.gameObject;
        else return checkoutCam.gameObject;
        
    }
    public void ChangeShopCam() 
    {
        //checkoutCam.enabled = false;
        //shopCam.enabled = true;

        checkoutCam.gameObject.SetActive(false);
        shopCam.gameObject.SetActive(true);
    }
    public void ChangeCheckOutCam()
    {
        //checkoutCam.enabled = true;
        //shopCam.enabled = false;
        checkoutCam.gameObject.SetActive(true);
        shopCam.gameObject.SetActive(false);
    }
}
