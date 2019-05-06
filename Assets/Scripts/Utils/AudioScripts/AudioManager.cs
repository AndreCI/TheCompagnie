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

    public float volume { get => volume_; set {
            if (volume_ == 0) { source.volume = 1; }
            else { source.volume /= volume_; }
            volume_ = value;
            source.volume *= volume;
        } }

    private float volume_;
    private AudioSource source;

    private void Start()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        GameObject.DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        database.Setup();
        source = gameObject.AddComponent<AudioSource>();
        volume = 1f;
        Play("TitleTheme");
    }

    public void Play(string Name)
    {
        Play(database.Get(Name));
    }

    private void Play(AudioSound audio)
    {
        source.clip = audio.file;
        source.volume = volume * audio.volume;
        source.pitch = audio.pitch;
        source.loop = audio.loop;
        source.Play();
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Overworld")
        {
            Play("OverworldTheme");
        }else if(scene.name == "Combat")
        {
            Play("CombatTheme");
        }else if(scene.name == "TitleScreen")
        {
            Play("TitleTheme");
        }
    }
}