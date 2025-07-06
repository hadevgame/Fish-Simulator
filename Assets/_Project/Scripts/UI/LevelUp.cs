using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : PopupAnim
{
    [SerializeField] private TextMeshProUGUI storeLevelText;
    [SerializeField] private List<GameObject> listSlot;
    [SerializeField] private Button btnContinue;
    [SerializeField] private GameObject light;
    public void ShowLevelUpPanel(int currentLevel, List<string> unlockedItems)
    {
        btnContinue.onClick.AddListener(HideLevelUpPanel);
        gameObject.SetActive(true);
        Show();
        RotateLight();
        storeLevelText.text = "Store Level " + currentLevel;
        

        for (int i = 0; i < listSlot.Count; i++)
        {
            if (i < unlockedItems.Count)
            {
                listSlot[i].gameObject.SetActive(true);
                Text itemText = listSlot[i].GetComponentInChildren<Text>();
                if (itemText != null)
                {
                    itemText.text = unlockedItems[i];
                }
            }
            else
            {
                listSlot[i].SetActive(false);
            }
        }
    }
    private void RotateLight()
    {
        if (light == null) return;

        light.transform.DORotate(
            new Vector3(0, 0, -360),
            5f,
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Incremental);
    }
    public void HideLevelUpPanel()
    {
        Hide();
    }
}
