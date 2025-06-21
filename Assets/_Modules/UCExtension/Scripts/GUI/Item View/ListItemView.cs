using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UCExtension.GUI
{
    public class ListItemView<T, S> : MonoBehaviour where T : ItemView<S>
    {
        [SerializeField] T prefab;

        [SerializeField] Transform itemParent;

        List<T> unuseItems = new List<T>();

        List<T> usingItems = new List<T>();

        public List<T> UsingItems => usingItems;

        private void Awake()
        {
            unuseItems = itemParent.GetComponentsInChildren<T>().ToList();
        }

        public void Display<Z>(List<Z> datas) where Z : S
        {
            usingItems.AddRange(unuseItems);
            unuseItems.Clear();
            for (int i = usingItems.Count; i < datas.Count; i++)
            {
                var billItem = Instantiate(prefab, itemParent);
                OnItemSpawned(billItem);
                usingItems.Add(billItem);
            }
            for (int i = 0; i < usingItems.Count; i++)
            {
                if (i < datas.Count)
                {
                    int index = i;
                    usingItems[i].gameObject.SetActive(true);
                    usingItems[i].SetData(datas[i], index);
                    usingItems[i].transform.SetSiblingIndex( index);
                    OnItemDisplayed(usingItems[i], i);
                }
                else
                {
                    usingItems[i].gameObject.SetActive(false);
                }
            }
            int unuseItemCount = UsingItems.Count - datas.Count;
            for (int i = 0; i < unuseItemCount; i++)
            {
                unuseItems.Add(usingItems.PopLastElement());
            }
            OnDisplay();
        }

        protected virtual void OnDisplay()
        {

        }

        protected virtual void OnItemSpawned(T item)
        {

        }

        protected virtual void OnItemDisplayed(T item, int index)
        {

        }

        public void AddUseItem(T item)
        {
            item.transform.SetParent(itemParent);
            usingItems.Add(item);
            item.gameObject.SetActive(true);
        }

        public void AddUnuseItem(T item)
        {
            item.transform.SetParent(itemParent);
            unuseItems.Add(item);
            item.gameObject.SetActive(false);
        }

        public void ResetView()
        {
            foreach (var item in UsingItems)
            {
                item.ResetView();
            }
        }
    }
}