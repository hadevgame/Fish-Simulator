using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using UCExtension;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ListBaseData<T> : BaseData where T : BaseData
{
    [FoldoutGroup("Datas")]
    [HorizontalGroup("Datas/GetData")]
    public Object DataFolder;

    [FoldoutGroup("Datas")]
    [HorizontalGroup("Datas/FindAvatars")]
    public Object AvatarsFolder;

    [FoldoutGroup("Datas")]
    public List<T> Datas;

    public T GetData(string ID)
    {
        foreach (var item in Datas)
        {
            if (item.HasSameID(ID))
            {
                return item;
            }
        }
        return null;
    }

    public bool Has(string ID)
    {
        foreach (var item in Datas)
        {
            if (item.HasSameID(ID))
            {
                return true;
            }
        }
        return false;
    }
#if UNITY_EDITOR    
    [Button]
    [FoldoutGroup("Datas")]
    [HorizontalGroup("Datas/GetData")]
    public void GetDatas()
    {
        string path = AssetDatabase.GetAssetPath(DataFolder);
        Datas = EditorExtension.LoadAssets<T>(path, FileNameExtension.SCRIPTABLE_OBJECT);
        Datas = Datas.OrderBy(x => x.name)
                    .OrderBy(x => x.SortPriority)
                    .ToList();
        this.SetDirtyAndSave();
    }

    [Button]
    [FoldoutGroup("Datas")]
    [HorizontalGroup("Datas/FindAvatars")]
    public virtual void FindAvatars()
    {
        foreach (var item in Datas)
        {
            item.Avatar = EditorExtension.FindSprite(AvatarsFolder, item.name);
            item.SetDirtyAndSave();
        }
    }
    [Button]
    [FoldoutGroup("Datas")]
    public virtual void UpdateOrder()
    {
        int index = 0;
        foreach (var item in Datas)
        {
            int order = index;
            item.SortPriority = order;
            index++;
            item.SetDirtyAndSave();
        }
    }
#endif
}