using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension.Animation
{
    public class BGLoop : MonoBehaviour
    {
        [SerializeField] float timeMove;
        // Start is called before the first frame update
        void Start()
        {
            RectTransform rect = GetComponent<RectTransform>();
            rect.DOAnchorPos(new Vector2(0, -rect.sizeDelta.y), timeMove).SetLoops(-1).SetEase(Ease.Linear).SetUpdate(true);
        }

    }
}