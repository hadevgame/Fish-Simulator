using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TutorialHoleController : UIBehaviour, ICanvasRaycastFilter
{
    [NonSerialized]
    private RectTransform m_RectTransform;
    public RectTransform rectTransform
    {
        get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
    }

    public static bool IsInTutorialZone;

    //[NonSerialized]
    //private Texture2D m_Texture;
    //public Texture2D texture
    //{
    //    get { return m_Texture ?? (m_Texture = GetComponent<Image>().sprite.texture); }
    //}

    public virtual bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        bool result = !isActiveAndEnabled || !RectTransformUtility.RectangleContainsScreenPoint(rectTransform, sp, eventCamera);
        IsInTutorialZone = !result;
        return result;
        //if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, sp, eventCamera))
        //{
        //    Debug.Log("ContainsScreenPoint");
        //    Vector2 localCursor = new Vector2();
        //    //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, null, out localCursor))
        //    //{
        //    //    Vector2 ptPivotCancelledLocation = new Vector2(localCursor.x - rectTransform.rect.x, localCursor.y - rectTransform.rect.y);
        //    //    Vector2 ptLocationRelativeToImageInScreenCoordinates = new Vector2(ptPivotCancelledLocation.x, ptPivotCancelledLocation.y);
        //    //    Vector2 ptLocationRelativeToImage01 = new Vector2(ptLocationRelativeToImageInScreenCoordinates.x / rectTransform.rect.width, ptLocationRelativeToImageInScreenCoordinates.y / rectTransform.rect.height);
        //    //    Color pixel = texture.GetPixel(Mathf.RoundToInt(ptLocationRelativeToImage01.x * texture.width), Mathf.RoundToInt(ptLocationRelativeToImage01.y * texture.height));
        //    //    if (pixel.a > 0.3f)
        //    //    {
        //    //        return false;
        //    //    }
        //    //    else
        //    //    {
        //    //        return true;
        //    //    }
        //    //}
        //    return false;
        //}
        //Debug.Log("NotContains");
        //return true;
    }
}
