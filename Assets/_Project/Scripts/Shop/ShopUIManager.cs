using TMPro;
using UCExtension;
using UCExtension.GUI;
using UnityEngine;
using UnityEngine.UI;

public class ShopGUI : BaseGUI
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Text totalPriceText;
    [SerializeField] private Sprite imageclick;
    [SerializeField] private Sprite image;
    public Button buttonProduct;
    public Button buttonTank;
    public Button buttonExit;
    public Button buttonBuy;
    public Button buttonClear;
    public Button buttonAddtocart;

    [SerializeField] private GameObject panelProduct;
    [SerializeField] private GameObject panelTank;
    ShopController shopController;
    private void Start()
    {
        buttonBuy.onClick.AddListener(Buy);
        buttonClear.onClick.AddListener(Clear);
        buttonExit.onClick.AddListener(ExitShop);
        buttonProduct.onClick.AddListener(ShowProductPanel);
        buttonTank.onClick.AddListener(ShowTankPanel);
        shopController = GetComponent<ShopController>();
    }

    public void TogglePanel(bool isopen) 
    {
        shopPanel.SetActive(isopen);
        Vibrator.SoftVibrate();
    }
    public void UpdateTotalPrice(float amount)
    {
        totalPriceText.text = "$" + amount.ToString();
    }

    public void ShowMessage(string message, bool isvalid)
    {
        if(isvalid) NotifiGUI.Instance.ShowPopup(message,Color.red);
        else
            NotifiGUI.Instance.ShowPopup(message);
    }

    public void ResetCartItem(GameObject cartItem)
    {
        cartItem.SetActive(false);
    }

    public void ExitShop()
    {
        ShopController.Ins.ExitShop();
    }

    public void Buy() 
    {
        shopController.BuyItem();
    }

    public void Clear() 
    {
        shopController.ClearCart();
    }
    public void ShowProductPanel()
    {
        Vibrator.SoftVibrate();
        buttonProduct.GetComponent<Image>().sprite = imageclick;
        buttonProduct.transform.localScale = new Vector3(1.2f, 1.2f, 1);

        buttonTank.GetComponent<Image>().sprite = image;
        buttonTank.transform.localScale = new Vector3(1, 1, 1);

        panelProduct.SetActive(true);
        panelTank.SetActive(false);
    }

    public void ShowTankPanel()
    {
        Vibrator.SoftVibrate();
        buttonProduct.GetComponent<Image>().sprite = image;
        buttonProduct.transform.localScale = new Vector3(1, 1, 1);

        buttonTank.GetComponent<Image>().sprite = imageclick;
        buttonTank.transform.localScale = new Vector3(1.2f, 1.2f, 1);

        panelProduct.SetActive(false);
        panelTank.SetActive(true);
    }
}