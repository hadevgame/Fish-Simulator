using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UCExtension;
using UCExtension.Audio;
using API.LogEvent;

[System.Serializable]
public class ShopLevelData
{
    public int level;
    public int requiredExp;
    public List<string> unlockableItems; // ID hoặc tên item unlock ở cấp này
}
public class ShopLevelManager : Singleton<ShopLevelManager>
{
    [SerializeField] private List<ShopLevelData> levelDataList;
    [SerializeField] private Image fillbar;
    [SerializeField] private Text leveltext;
    [SerializeField] private TextMeshProUGUI floattext;
    [SerializeField] private LevelUp levelup; // Reference to the new script

    public static Action<int> OnAddExp = null;
    private Vector3 startPos;

    private int currentExp = 0;
    private int currentLevel = 1;

    public int CurrentLevel => currentLevel;
    public int CurrentExp => currentExp;

    private void Start()
    {
        OnAddExp = AddExp;
        startPos = floattext.transform.localPosition;
    }

    private void OnDisable()
    {
        OnAddExp = null;
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        UpdateUI();
        CheckLevelUp();
        Color lightBlue = new Color(142, 229, 245);
        ShowFloatingText(amount, lightBlue);
        AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("XP"));
    }

    private void CheckLevelUp()
    {
        while (currentLevel <= levelDataList.Count &&
               currentExp >= levelDataList[currentLevel - 1].requiredExp)
        {
            currentLevel++;
            currentExp = 0;
            AudioManager.Ins.PlaySFX(GameManager.Instance.AudioSO.GetAudioClip("LVLUP"));
            FirebaseLogger.Ins.LogEvent("level_up", new Firebase.Analytics.Parameter("level", currentLevel.ToString()));
            var unlockedItems = levelDataList[currentLevel - 1].unlockableItems;
            levelup.ShowLevelUpPanel(currentLevel, unlockedItems);
            ItemSlotDisplay.OnUnlockItem?.Invoke(currentLevel);
            //if(currentLevel == 2) Instruction.Ins.ShowInstruction("Open shop to buy new fish");

        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        float fillAmount = (float)currentExp / levelDataList[currentLevel - 1].requiredExp;
        fillbar.fillAmount = Mathf.Clamp01(fillAmount);
        leveltext.text = currentLevel.ToString();
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt("ShopLevel", currentLevel);
        PlayerPrefs.SetInt("ShopExp", currentExp);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        currentLevel = PlayerPrefs.GetInt("ShopLevel", 1); // default level = 1
        currentExp = PlayerPrefs.GetInt("ShopExp", 0);     // default exp = 0
        UpdateUI();
    }

    private void ShowFloatingText(int amount, Color textColor)
    {
        if (floattext == null) return;

        floattext.text = (amount > 0 ? "+" : "-") + Mathf.Abs(amount).ToString() + "exp";
        floattext.color = textColor;
        floattext.gameObject.SetActive(true);
        floattext.alpha = 1f;

        floattext.transform.localPosition = startPos; // Reset position
        floattext.transform.DOLocalMoveY(floattext.transform.localPosition.y + 50, 1f).SetEase(Ease.OutQuad); // Move up
        floattext.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            floattext.gameObject.SetActive(false);
        });
    }
   
}

