using API.Ads;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UCExtension;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;

public class CheckOutController : Singleton<CheckOutController>
{
    [SerializeField] private GameObject checkOutCamera;
    [SerializeField] private GameObject mainCamera;

    [SerializeField] private GameObject checkOutCanvas;
    [SerializeField] private GameObject cashpayment;
    [SerializeField] private GameObject cardpayment;
    [SerializeField] private PayMentUI cashpaymentScene;
    [SerializeField] private CardPaymentUI cardpaymentScene;
    [SerializeField] private TotalUI totalScene;
    [SerializeField] private CashDesk cashDesk;
    [SerializeField] private Button btnExit;
    
    public bool IsActive => gameObject.activeSelf; 

    private void Start()
    {
        totalScene.gameObject.SetActive(true);
        btnExit.onClick.AddListener(ExitCheckOut);
        checkOutCamera = CameraManager.Ins.GetCam(3);
        mainCamera = CameraManager.Ins.GetCam(1);
    }
    private void OnEnable()
    {
        btnExit.gameObject.SetActive(true);

    }

    public void ToggleButton(bool isactive) 
    {
        btnExit?.gameObject.SetActive(isactive);
    }
    public void ExitCheckOut() 
    {
        Vibrator.SoftVibrate();
        checkOutCamera.SetActive(false);
        GUIController.Ins.Open<PlayGUI>();
        checkOutCanvas.SetActive(false);
        mainCamera.SetActive(true);
        btnExit.gameObject.SetActive(false);
    }
    public void PayMent(float money, int count , bool iscard) 
    {
        float total = totalScene.GetData();
        if (iscard) 
        {
            cardpayment.SetActive(true);
            cardpaymentScene.gameObject.SetActive(true);
            totalScene.gameObject.SetActive(false);
            cardpaymentScene.UpdateScene(total);
        }
        else 
        {
            cashpayment.SetActive(true);
            cashpaymentScene.gameObject.SetActive(true);
            totalScene.gameObject.SetActive(false);
            cashpaymentScene.UpdateScene(money, count, total);
        }
        
    }
}
