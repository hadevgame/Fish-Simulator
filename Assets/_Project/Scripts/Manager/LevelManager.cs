using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UCExtension;
using UCExtension.Audio;

[System.Serializable]
public class LevelData
{
    public int level;
    public int requiredExp;
    public List<string> unlockableItems; 
}
public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private List<LevelData> levelDataList;
    [SerializeField] private Image fillbar;
    [SerializeField] private Text leveltext;
    [SerializeField] private TextMeshProUGUI floattext;
    [SerializeField] private LevelUp levelup; 

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
        AudioManager.Ins.PlaySFX(StoreManager.Instance.AudioSO.GetAudioClip("XP"));
    }

    private void CheckLevelUp()
    {
        while (currentLevel <= levelDataList.Count &&
               currentExp >= levelDataList[currentLevel - 1].requiredExp)
        {
            currentLevel++;
            currentExp = 0;
            AudioManager.Ins.PlaySFX(StoreManager.Instance.AudioSO.GetAudioClip("LVLUP"));
            var unlockedItems = levelDataList[currentLevel - 1].unlockableItems;
            levelup.ShowLevelUpPanel(currentLevel, unlockedItems);
            ItemSlotDisplay.OnUnlockItem?.Invoke(currentLevel);

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
        currentLevel = PlayerPrefs.GetInt("ShopLevel", 1); 
        currentExp = PlayerPrefs.GetInt("ShopExp", 0);     
        UpdateUI();
    }

    private void ShowFloatingText(int amount, Color textColor)
    {
        if (floattext == null) return;

        floattext.text = (amount > 0 ? "+" : "-") + Mathf.Abs(amount).ToString() + "exp";
        floattext.color = textColor;
        floattext.gameObject.SetActive(true);
        floattext.alpha = 1f;

        floattext.transform.localPosition = startPos; 
        floattext.transform.DOLocalMoveY(floattext.transform.localPosition.y + 50, 1f).SetEase(Ease.OutQuad); 
        floattext.DOFade(0, 1f).SetEase(Ease.Linear).OnComplete(() =>
        {
            floattext.gameObject.SetActive(false);
        });
    }
   
}

