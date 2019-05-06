using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SoundDatabase : ScriptableObject
{
    public enum SOUND_TYPE {GLOBAL, THEME, ANIMATION };
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


}