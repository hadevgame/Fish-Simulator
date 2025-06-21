using System.Collections;
using System.Collections.Generic;
using UCExtension.Animation;
using UnityEngine;

public class MaterialOffsetAnim : LoopingAnim
{
    [SerializeField] Vector2 deltaSpeed;

    Renderer render;

    public bool isPlaying { get; set; }

    public override bool MustStartInEnable => false;

    public override void Play()
    {
        isPlaying = true;
    }

    public override void Stop()
    {
        isPlaying = false;
    }

    private void Awake()
    {
        render = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            render.material.mainTextureOffset += deltaSpeed * Time.deltaTime;
        }
    }
}
