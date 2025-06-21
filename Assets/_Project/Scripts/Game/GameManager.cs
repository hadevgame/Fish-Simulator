using System;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Audio;
using UCExtension.GUI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private List<Transform> spawnItemPositions;
    [SerializeField] private GameObject signBoard;
    private bool isStoreOpen = false;
    private bool endday = false;
    public static Action OnCloseStore = null;
    public AudioSO AudioSO {  get; private set; }

    [SerializeField] Canvas PlacingCanvas;
    public static Action<bool> OnPlacingMode = null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        OnPlacingMode = PlacingMode;
        GUIController.Ins.Open<ShopGUI>().TogglePanel(false);
        GUIController.Ins.Open<SceneGUI>();
        GUIController.Ins.Open<NotifiGUI>();
    }
    private void Start()
    {
        OnCloseStore = CloseStore;

        AudioSO = Resources.Load<AudioSO>("Audio");
    }

    
    private void OnDisable()
    {
        OnCloseStore = null;
        OnPlacingMode = null;
    }

    public List<Transform> GetSpawnPoint() 
    {
        return spawnItemPositions;
    }
    private void PlacingMode(bool isOn) 
    {
        PlacingCanvas.enabled = isOn;
    }
    public void CloseStore() 
    {
        endday = true;
        SetStoreStatus(false);
    }
    public void ToggleStore()
    {
        if (endday) return;
        SetStoreStatus(!isStoreOpen);
    }

    public void Setstate() 
    {
        endday = false;
    }
    public bool GetState() 
    {
        if (isStoreOpen == true) return true;
        else return false;
    }

    public void SetStoreStatus(bool isOpen)
    {
        isStoreOpen = isOpen;
        Debug.Log(isOpen ? "Cửa hàng mở!" : "Cửa hàng đóng!");
        if (signBoard != null)
        {
            if (isStoreOpen)
            {
                signBoard.transform.rotation = Quaternion.Euler(0, -90, 0); 
                GameTimeManager.Instance.StartTime(); 
            }
            else
            {
                signBoard.transform.rotation = Quaternion.Euler(0, 90, 0);
                
            }
        }
    }
}
