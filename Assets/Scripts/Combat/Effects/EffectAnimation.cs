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
    public int width;
    public int height;
    public float scale = 1f;
    public bool reverseTime = false;
    [Header("Remove and add specific parts of the sprite")]
    public int minWidth;
    public int minHeight;
    public int maxWidth;
    public int maxHeight;
    public int repeat = 0;
    public Color modifyColor;

    [HideInInspector]
    public bool playing;
    public int ID;
    [HideInInspector] public bool activate;
    [HideInInspector] private int index;
    [HideInInspector] private float currentTime;
    [HideInInspector] private Texture2D texture;
    [HideInInspector] private List<Sprite> animationList;
    [HideInInspector] private SpriteRenderer renderer;
    [HideInInspector] private float updatedTimePerFrame;


    public EffectAnimation GetEffect()
    {
        EffectAnimation e = GeneralUtils.Copy<EffectAnimation>(this);
        e.Setup();
        return e;
    }
    public void Setup()
    {
        if(type == AnimationClipDatabase.T.NONE)
        {
            return;
        }
        texture = animationSpriteList.texture;
        animationList = new List<Sprite>();

        int jstart = maxHeight >0 ? Mathf.Min(height - 1, maxHeight): height - 1;
        int jend = Mathf.Max(0, minHeight);
        int istart = Mathf.Max(0, minWidth);
        int iend = maxWidth > 0 ? Mathf.Min(width, maxWidth) : width;
        for (int repeatIndex = 0; repeatIndex < repeat + 1; repeatIndex++)
        {
            for (int j = height - 1; j >= 0; j--)
            {
                for (int i = 0; i < width; i++)
                {
                    if ((i >= istart || j != jstart) && (i <= iend || j != jend) && j >= jend && j <= jstart)
                    {
                        float w = (float)(i * (float)texture.width / (float)width);
                        float h = (float)(j * (float)texture.height / (float)height);
                        float w1 = (float)((float)texture.width / (float)width);
                        float h1 = (float)((float)texture.height / (float)height);
                        Rect rect = new Rect(w, h, w1, h1);
                        Sprite s = (Sprite.Create(texture, rect, Vector2.one * 0.5f, 100f));//, border:(new Vector4(0f,0f,texture.width, texture.height))));
                        
                        animationList.Add(s);
                    }
                }
            }
        }
    }
    public void FixedUpdate(float fixedDeltaTime)
    {
        if (type != AnimationClipDatabase.T.NONE)
        {
            if (activate)
            {
                Play();
            }
            else if (playing)
            {
                currentTime += fixedDeltaTime;
                if (index >= animationList.Count)
                {
                    playing = false;
                    renderer.sprite = null;

                }
                else if (currentTime > updatedTimePerFrame)
                {
                    renderer.sprite = animationList[(reverseTime? animationList.Count - 1 - index : index)];
                    index += 1;
                    currentTime = 0f;
                }
            }
        }
    }

 /*   public void Activate(EffectAnimationHandler handler, float timeFactor=1f, float forcedTime = 0f)
    {
        if (type != AnimationClipDatabase.T.NONE)
        {
            Debug.Log("here " + animationList.Count);
            renderer = handler.sRenderers;
            renderer.transform.localScale = new Vector3(scale, scale, 1f);
            activate = true;
            updatedTimePerFrame = forcedTime > 0 ? (forcedTime) : (timeFactor * TurnManager.Instance.timePerEvent);
            updatedTimePerFrame = (updatedTimePerFrame) / ((float)animationList.Count);
        }
    }*/

    public void ActivateFromRenderer(SpriteRenderer renderer_, float timeFactor = 1f, float forcedTime = 0f)
    {
        if (type != AnimationClipDatabase.T.NONE)
        {
            renderer = renderer_;
            renderer.transform.localScale = new Vector3(scale, scale, 1f);
            activate = true;
            updatedTimePerFrame = forcedTime > 0 ? (forcedTime) : (timeFactor * TurnManager.Instance.timePerEvent);
            updatedTimePerFrame = (updatedTimePerFrame) / ((float)animationList.Count);
        }
    }
    private void Play()
    {
        activate = false;
        playing = true;
        currentTime = 0f;
        index = 0;
    }

   
}
