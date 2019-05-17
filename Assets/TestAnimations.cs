using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestAnimations : MonoBehaviour
{

    public SpriteRenderer renderer;
    public AnimationClipDatabase.T current;
    public bool locked = false;
   public AnimationClipDatabase database;
    public float forcedTime=1f;

    public EffectAnimation currentClip;
    [HideInInspector] private List<AnimationClipDatabase.T> allTypes;
    [HideInInspector] private int index = 0;

    void Start()
    {
        allTypes = new List<AnimationClipDatabase.T>();
        foreach(AnimationClipDatabase.T t in Enum.GetValues(typeof(AnimationClipDatabase.T)))
        {
            allTypes.Add(t);
        }
        database.Setup();
        
    }
    public void toggleLock()
    {
        locked = !locked;
    }

    private void FixedUpdate()
    {
        currentClip?.FixedUpdate(Time.fixedDeltaTime);
    }
    public void Animate()
    {
        if (!locked) { index = allTypes.IndexOf(current)+1; current = allTypes[index];  }
        currentClip = database.Get(current);
        currentClip?.Setup();
        currentClip?.ActivateFromRenderer(renderer, forcedTime: forcedTime);
    }
}
