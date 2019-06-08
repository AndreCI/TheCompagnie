using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AnimationClipDatabase : ScriptableObject
{
    public enum T
    {
        NONE,
        BLOCK,
        BITE,
        SLASH,
        POWER_UP,
        POISON,
        BRUN_LASER,
        BURN_VERTICAL,
        DAGGER,
        MAGGIC_EFFECT_CIRCLE,
        FIRE_CLAW,
        FIRE_EXPLOSION,
        MAGICATTACK,
        THUNDER,
        WIND,
        HEAL,
        HIT,
        SPARKLE,
        MAGIC_SPHERE,
        VOID_SLASH,
        ICE_HIT,
        SLASH_MANY,
        HIT_BIG,
        WIND_HIT,
        VOID_SMOKE,
        LIGHT_HIT,
        LIGHT_HIT2,
        CURSE_FACE,
        POISON_VIOLET,
        CURSEE_UP,
        HIT_MANY,
        SLASH_V,
        WATER_BUBBLE,
        DARK_PLANET,
        YELLOW_CIRCLE,
        LIGHT_EXPLOSION,
        POWER_UP_2,
        BLUE_SHIELD,
        FIRE_TORNADO,
        VIOLET_QUICK_SPARK,
        CURSE_TENTACULES,
        SLASH_DOUBLE_VIOLET,
        SLASH_DOUBLE_CYAN,
        POWER_DOWN,
        STATUS
    }

    public List<EffectAnimation> animations;
    public List<AnimationClipDatabase> children;

    public void Setup()
    {
        if (children != null && children.Count > 0)
        {
            List<EffectAnimation> litems = new List<EffectAnimation>();
            int idOffset = 0;
            foreach (AnimationClipDatabase database in children)
            {
                database.Setup();
                foreach (EffectAnimation e in database.animations)
                {
                    e.ID += idOffset;
                    litems.Add(e);
                }
                idOffset += database.animations.Count;
            }
            animations = litems;
        }
        else
        {
            foreach (EffectAnimation ea in animations)
            {
                ea.Setup();
            }
        }
    }
    public List<EffectAnimation> GetAnimations()
    {
        return animations;
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