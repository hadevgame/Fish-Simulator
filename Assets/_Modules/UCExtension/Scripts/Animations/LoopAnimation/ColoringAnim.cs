using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Animation;
using UnityEngine;
using UnityEngine.UI;

public class ColoringAnim : LoopingAnim
{
    [SerializeField] float timeDo;

    [SerializeField] Ease ease = Ease.InOutSine;

    [SerializeField] List<Color> colors;

    SpriteRenderer spriteRenderer;

    Image image;

    Tween colorTween;

    string ID => "ColoringAnim" + GetInstanceID();
    public override bool MustStartInEnable => false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            image = GetComponent<Image>();
        }
    }
    public override void Play()
    {
        Stop();
        StartCoroutine(IEAnimation());
    }

    public override void Stop()
    {
        DOTween.Kill(ID);
        StopAllCoroutines();
    }

    IEnumerator IEAnimation()
    {
        while (true)
        {
            foreach (var item in colors)
            {
                if (spriteRenderer)
                {
                    yield return spriteRenderer.DOColor(item, timeDo)
                        .SetEase(Ease.Linear)
                        .SetId(ID)
                        .WaitForCompletion();
                }
                else if (image)
                {
                    yield return image.DOColor(item, timeDo)
                        .SetEase(Ease.Linear)
                        .SetId(ID)
                        .WaitForCompletion();
                }
                else
                {
                    yield return null;
                }
            }
            yield return null;
        }
    }
}
