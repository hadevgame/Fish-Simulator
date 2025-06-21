using UnityEngine;
using UnityEngine.UI;

namespace UCExtension.GUI
{
    public class BaseDataView<T> : ItemView<T> where T : BaseData
    {
        [SerializeField] Text nameText;

        [SerializeField] Text descriptionText;

        [SerializeField] Image avatarImage;

        [SerializeField] bool setNativeSizeAvatar;

        public override void SetData(T data,int index)
        {
            base.SetData(data, index);
            nameText.text = data.Name;
            descriptionText.text = data.Description;
            avatarImage.sprite = data.Avatar;
            if (setNativeSizeAvatar)
            {
                avatarImage.SetNativeSize();
            }
        }
    }
}