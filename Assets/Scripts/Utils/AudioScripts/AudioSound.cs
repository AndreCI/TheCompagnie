using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class AudioSound
{
    public enum AUDIO_SET { NONE, OVERWORLD_THEME, COMBAT_THEME, CARD_DRAW, CARD_PLAY, CARD_SHUFFLE};
    [Header("General Info")]
    public string Name;
    public bool loop;
    public AUDIO_SET set;
    [Header("Audio information")]
    public AudioClip file;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Header("Database Infos")]
    public int ID;
    public SoundDatabase.SOUND_TYPE audioType;
}