using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AnimationClipDatabase : ScriptableObject
{
    public enum T { HIT, BLOCK, BITE, SLASH, POWER_UP, POISON, BURN, BURN_POWER_UP, DAGGER }
    public List<EffectAnimation> animations;

    public void Setup()
    {
        foreach(EffectAnimation ea in animations)
        {
            ea.Setup();
        }
    }
    public EffectAnimation Get(int index)
    {
        return GeneralUtils.Copy<EffectAnimation>(animations[index]);
    }

    public EffectAnimation Get(T type)
    {
        return GeneralUtils.Copy<EffectAnimation>(animations.Find(a => a.type == type));
    }

}