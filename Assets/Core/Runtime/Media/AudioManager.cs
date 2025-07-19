using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    
    private AudioSource music;
    private AudioSource[] sfx;
    
    [Range(16, 1024)] public int sfxCount = 64;
    
    private int playIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Init();
        playIndex = 0;
    }

    private void Init()
    {
        InitMusic();
        InitSFX();
    }

    private void Update()
    {
        music.volume = musicVolume * masterVolume;
        foreach (var s in sfx)
        {
            s.volume = sfxVolume * masterVolume;
        }
    }

    private void InitMusic()
    {
        GameObject musicObj = new GameObject("Music");
        musicObj.transform.SetParent(transform);
        
        AudioSource s = musicObj.AddComponent<AudioSource>();
        s.clip = null;
        s.playOnAwake = false;
        s.volume = musicVolume * masterVolume;
        s.loop = true;

        music = s;
    }

    private void InitSFX()
    {
        Transform sfxParent = new GameObject("SFX").transform;
        sfxParent.parent = transform;
        
        AudioSource[] s = new AudioSource[sfxCount];
        
        for (int i = 0; i < sfxCount; i++)
        {
            GameObject sfxObj = new GameObject($"SFX [{i}]");
            sfxObj.transform.SetParent(sfxParent);
            
            s[i] = sfxObj.AddComponent<AudioSource>();
            s[i].clip = null;
            s[i].playOnAwake = false;
            s[i].volume = sfxVolume * masterVolume;
            s[i].loop = false;
        }
        
        sfx = s;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        music.clip = clip;
        music.loop = loop;
        music.Play();
    }

    public void StopMusic()
    {
        music.clip = null;
        music.loop = true;
        music.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfx[playIndex].PlayOneShot(clip);
        if (++playIndex >= sfx.Length)
        {
            playIndex = 0;
        }
    }

    public static void SetMasterVolume(float volume)
    {
        Instance.masterVolume = volume;
    }

    public static void SetMusicVolume(float volume)
    {
        Instance.musicVolume = volume;
    }

    public static void SetSFXVolume(float volume)
    {
        Instance.sfxVolume = volume;
    }
}