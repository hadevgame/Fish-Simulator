using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UCExtension.DynamicScroll
{

    public class SelectLevelItem : MonoBehaviour
    {
        [SerializeField] Text levelText;
        public void SetUp(SelectLevelItemData data)
        {
            levelText.text = data.id.ToString();
        }
    }

}