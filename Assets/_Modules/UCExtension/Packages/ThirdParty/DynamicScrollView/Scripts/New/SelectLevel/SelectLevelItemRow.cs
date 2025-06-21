using dynamicscroll;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UCExtension.DynamicScroll
{
    public class SelectLevelItemRow : DynamicScrollObject<ListSelectLevelItemData>
    {
        //[SerializeField] LevelItem selectLevelItemPrefab;

        [SerializeField] Transform selectLevelItemParent;
        public override float CurrentHeight { get; set; }
        public override float CurrentWidth { get; set; }
        bool isInit = false;
        //List<LevelItem> listSelectLevelItem = new List<LevelItem>();
        public void Awake()
        {
            CurrentHeight = GetComponent<RectTransform>().rect.height;
            CurrentWidth = GetComponent<RectTransform>().rect.width;
        }

        public override void UpdateScrollObject(ListSelectLevelItemData item, int index)
        {
            base.UpdateScrollObject(item, index);
            Debug.Log(item.datas.Count);
            if (!isInit)
            {
                isInit = true;
                Init(item);
            }
            else
            {
                SetUp(item);
            }
        }

        public void Init(ListSelectLevelItemData item)
        {
            //foreach (var data in item.datas)
            //{
            //    var selectLevelItem = Instantiate(selectLevelItemPrefab, selectLevelItemParent);
            //    selectLevelItem.SetUp(data);
            //    listSelectLevelItem.Add(selectLevelItem);
            //}
        }

        public void SetUp(ListSelectLevelItemData item)
        {
            //for(int i = 0; i < listSelectLevelItem.Count; i++)
            //{
            //    listSelectLevelItem[i].SetUp(item.datas[i]);
            //}
        }
    }
}