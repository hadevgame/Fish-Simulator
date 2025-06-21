using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UCExtension.GUI
{
    public class RateStar : MonoBehaviour
    {
        [SerializeField] Image starImage;

        Button getButton;

        int star;

        public Action<int> OnSelect;

        // Start is called before the first frame update
        void Start()
        {
            getButton = GetComponent<Button>();
            getButton.onClick.AddListener(Click);
        }

        public void SetStar(int star)
        {
            this.star = star;
        }

        public void SetSprite(Sprite sprite)
        {
            starImage.sprite = sprite;
            starImage.SetNativeSize();
        }


        public void Click()
        {
            OnSelect?.Invoke(star);
        }
    }
}