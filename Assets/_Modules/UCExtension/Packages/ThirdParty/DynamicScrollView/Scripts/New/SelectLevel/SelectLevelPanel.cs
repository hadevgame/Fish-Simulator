using dynamicscroll;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.DynamicScroll
{
    public class SelectLevelPanel : MonoBehaviour
    {
        public DynamicScrollRect verticalScroll;
        public GameObject referenceObject;

        private DynamicScroll<ListSelectLevelItemData, SelectLevelItemRow> mVerticalDynamicScroll = new DynamicScroll<ListSelectLevelItemData, SelectLevelItemRow>();

        public void Start()
        {
            List<ListSelectLevelItemData> list = new List<ListSelectLevelItemData>();
            int col = 5;
            int row = 9000 / col;
            for (int i = 0; i < row; i++)
            {
                ListSelectLevelItemData listSelectLevelItemData = new ListSelectLevelItemData();

                if (i % 2 == 0)
                {
                    for (int j = 0; j < col; j++)
                    {
                        listSelectLevelItemData.datas.Add(new SelectLevelItemData() { id = col * i + j });
                    }
                }
                else
                {
                    for (int j = col - 1; j >= 0; j--)
                    {
                        listSelectLevelItemData.datas.Add(new SelectLevelItemData() { id = col * i + j });
                    }
                }

                list.Add(listSelectLevelItemData);
            }
            mVerticalDynamicScroll.spacing = 5f;
            mVerticalDynamicScroll.Initiate(verticalScroll, list.ToArray(), 0, referenceObject);
        }
    }
}