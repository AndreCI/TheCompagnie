using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;

[Serializable]
public class EffectAnimation
{
    public AnimationClipDatabase.T type;
    public Sprite animationSpriteList;
    public float timePerFrame = 0.2f;
    public int width;
    public int height;
    [HideInInspector]
    private bool playing;
    [HideInInspector] private bool activate;
    [HideInInspector] private int index;
    [HideInInspector] private float currentTime;
    [HideInInspector] private Texture2D texture;
    [HideInInspector] private List<Sprite> animationList;
    [HideInInspector] private SpriteRenderer renderer;
    [HideInInspector] private UnitUI ui;
    [HideInInspector] private float updatedTimePerFrame;

    public void Setup()
    {
        texture = animationSpriteList.texture;
        animationList = new List<Sprite>();

        for (int j = height - 1; j >= 0; j--)
        {
            for (int i = 0; i< width ; i++)
            {
                float w =  (float)( i * (float)texture.width / (float)width);
                float h =  (float)(j * (float)texture.height / (float)height);
                float w1 = (float)((float)texture.width / (float)width);
                float h1 = (float)((float)texture.height / (float)height);
                Rect rect = new Rect(w, h, w1, h1);
                Sprite s = (Sprite.Create(texture, rect, Vector2.one * 0.5f, 100f));//, border:(new Vector4(0f,0f,texture.width, texture.height))));
                
                animationList.Add(s);
            }
        }
        updatedTimePerFrame = timePerFrame;
    }
    public void FixedUpdate(float fixedDeltaTime)
    {
        if (activate)
        {
            Play();
        }else if (playing) {
            currentTime += fixedDeltaTime;
            if(index >= animationList.Count)
            {
                playing = false;
                renderer.sprite = null;
                ui.currentAnimation = null;

            }
            else if (currentTime > updatedTimePerFrame)
            {
                renderer.sprite = animationList[index];
                index += 1;
                currentTime = 0f;
            }
        }
    }
    public void Activate(UnitUI ui_)
    {
        ui = ui_;
        renderer = ui.effectSpriteRenderer;
        activate = true;
        ui.currentAnimation = this;
        updatedTimePerFrame = timePerFrame * PlayerInfos.Instance.settings.eventSpeed;
    }
    private void Play()
    {
        activate = false;
        playing = true;
        currentTime = 0f;
        index = 0;
    }

   
}
