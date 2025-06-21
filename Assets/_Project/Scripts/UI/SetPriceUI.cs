using DG.Tweening;
using TMPro;
using UCExtension;
using UnityEngine;
using UnityEngine.UI;

public class SetPriceUI : Singleton<SetPriceUI>
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text costText;
    [SerializeField] private Text profitText;
    [SerializeField] private Text priceText;
    [SerializeField] private Text marketPriceText;

    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [SerializeField] private Slider priceSlider;
    [SerializeField] private Button okButton;

    [SerializeField] private PopupAnim popupAnim;

    private float cost;
    private float marketPrice;
    private FishData currentFishData;
    private float currentPrice;

    private void Start()
    {
        Hide();
        minusButton.onClick.AddListener(() => ChangePrice(-0.01f));
        plusButton.onClick.AddListener(() => ChangePrice(0.01f));
        priceSlider.onValueChanged.AddListener(OnSliderChanged);
        okButton.onClick.AddListener(ConfirmPrice);
    }

    public void Open(FishData fishData)
    {
        currentFishData = fishData;
        Show();
        iconImage.sprite = fishData.icon;
        nameText.text = fishData.name;
        cost = fishData.basePrice;
        currentPrice = PriceManager.Ins.GetPrice(fishData);
        marketPrice = currentPrice;

        priceSlider.minValue = cost;
        priceSlider.maxValue = cost * 2f;
        priceSlider.value = currentPrice;

        UpdateUI(currentPrice);
        popupAnim.StartPulse(marketPriceText.transform);
        gameObject.SetActive(true);
    }

    private void ChangePrice(float delta)
    {
        float newPrice = Mathf.Clamp(currentPrice + delta, priceSlider.minValue, priceSlider.maxValue);
        priceSlider.value = newPrice;
        UpdateUI(newPrice);
    }

    private void OnSliderChanged(float value)
    {
        UpdateUI(value);
    }

    private void UpdateUI(float price)
    {
        currentPrice = price;
        priceText.text = $"PRICE: ${price:F2}";
        costText.text = $"Cost ${cost:F2}";
        profitText.text = $"PROFIT: ${price - cost:F2}";
        marketPriceText.text = $"Market Price: ${marketPrice:F2}";
    }

    private void ConfirmPrice()
    {
        PriceManager.Ins.SetPrice(currentFishData, currentPrice);
        Hide();
    }

    public void Show(float duration = 0.5f)
    {
        popupAnim.Show(duration);
    }

    public void Hide(float duration = 0.5f)
    {
        popupAnim.Hide(duration);
    }
}
