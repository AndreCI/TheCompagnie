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
    [Header("General Info")]
    public string Name;
    public bool loop;
    [Header("Audio information")]
    public AudioClip file;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    [Header("Database Infos")]
    public int ID;
    public SoundDatabase.SOUND_TYPE audioType;
}