using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SoundDatabase : ScriptableObject
{
    public enum SOUND_TYPE {GLOBAL, THEME, ANIMATION, MISC, REACTION, STATUS };
    public SOUND_TYPE type;
    public List<SoundDatabase> childrenDatabase;
    public List<AudioSound> sounds;

    public List<AudioSound> Setup()
    {
        if(type == SOUND_TYPE.GLOBAL && childrenDatabase.Count > 0)
        {
            sounds = new List<AudioSound>();
            int offsetId = 0;
            List<AudioSound> newSounds = new List<AudioSound>();
            foreach(SoundDatabase db in childrenDatabase)
            {
                newSounds = db.Setup();
                foreach(AudioSound a in newSounds)
                {
                    a.ID += offsetId;
                }
                offsetId += newSounds.Count;
                sounds.AddRange(newSounds);
            }
        }else if (childrenDatabase.Count == 0)
        {
            int id = 0;
            foreach(AudioSound a in sounds)
            {
                a.ID = id;
                id += 1;
                a.audioType = type;
            }
            return sounds;
        }
        else
        {
            Debug.Log("ISSUE WITH SOUND DATABASE");
            return null;
        }
        return sounds;
    }
    public AudioSound Get(string Name)
    {
        return sounds.Find(x => x.Name == Name);
    }

    public AudioSound GetRandom(AudioSound.AUDIO_SET set)
    {
        if (sounds.FindAll(x => x.set == set).Count > 0)
        {
            return Shuffle(sounds.FindAll(x => x.set == set).ToList()).First();
        }
        return null;
    }
    public static List<AudioSound> Shuffle(List<AudioSound> sounds)
    {
        int n = sounds.Count;
        while (n > 1)
        {
            n--;
            int k = Utils.rdx.Next(n + 1);
            AudioSound value = sounds[k];
            sounds[k] = sounds[n];
            sounds[n] = value;
        }
        return sounds;
    }

}