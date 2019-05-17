using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance { get => instance; }

    public SoundDatabase database;

    private Dictionary<SoundDatabase.SOUND_TYPE, float> volume_;
    private Dictionary<SoundDatabase.SOUND_TYPE, AudioSource> sources;

    public float GetVolume(SoundDatabase.SOUND_TYPE type)
    {
        return volume_[type];
    }
    public void SetVolume(float value, SoundDatabase.SOUND_TYPE type = SoundDatabase.SOUND_TYPE.GLOBAL)
    {
        if(type == SoundDatabase.SOUND_TYPE.GLOBAL) { foreach(SoundDatabase.SOUND_TYPE t in sources.Keys) { SetVolume(value, t); } return; }
       if(volume_[type] == 0) { sources[type].volume = 1f; }
        else { sources[type].volume /= volume_[type]; }
        volume_[type] = value;
        sources[type].volume *= volume_[type];
    }

    private void Start()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        GameObject.DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        database.Setup();
        sources = new Dictionary<SoundDatabase.SOUND_TYPE, AudioSource>();
        volume_ = new Dictionary<SoundDatabase.SOUND_TYPE, float>();
        foreach(SoundDatabase.SOUND_TYPE ctype in Enum.GetValues(typeof(SoundDatabase.SOUND_TYPE)))
        {
            if (ctype != SoundDatabase.SOUND_TYPE.GLOBAL)
            {
                sources.Add(ctype, gameObject.AddComponent<AudioSource>());
                volume_.Add(ctype, 1);
                
            }

        }
        SetVolume(PlayerSettings.Instance.themeVolume);
        Play("TitleTheme");
    }

    public void Play(string Name)
    {
        Play(database.Get(Name));
    }

    public void PlayFromSet(AudioSound.AUDIO_SET set)
    {
        Play(database.GetRandom(set));
    }

    private void Play(AudioSound audio)
    {
       
        sources[audio.audioType].clip = audio.file;
        sources[audio.audioType].volume = volume_[audio.audioType] * audio.volume;
        sources[audio.audioType].pitch = audio.pitch;
        sources[audio.audioType].loop = audio.loop;
        sources[audio.audioType].Play();
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Overworld")
        {
            PlayFromSet(AudioSound.AUDIO_SET.OVERWORLD_THEME);
        }
        else if(scene.name == "Combat")
        {
            PlayFromSet(AudioSound.AUDIO_SET.COMBAT_THEME);
        }else if(scene.name == "TitleScreen")
        {
            Play("TitleTheme");
        }
    }
}