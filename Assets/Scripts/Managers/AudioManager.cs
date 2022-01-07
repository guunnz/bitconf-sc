using System;
using System.Collections;
using UnityEngine;
using System.Linq;


[System.Serializable]
public class Sound
{
    public Config.Types.SFX sfxType;
    public AudioClip[] clips;
    [Range(0f, 2f)]
    public float volume = 1;

    internal float startingVolume;

    public bool loop;
    [HideInInspector]
    public AudioSource source;

    public bool PlayOnAwake;

    public bool BackgroundMusic;

    public bool RandomizePitchModerately;

    [Range(.1f, 3f)]
    public float pitch = 1;
}

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    static public AudioManager DO;
    //private float BackgroundMusicVolume = 0.4f;

    private void Awake()
    {
        DO = this;
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clips[0];
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = s.PlayOnAwake;
            s.startingVolume = s.volume;
        }
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt(PlayerPrefValues.StartupAudio) == 0)
        {
            PlayerPrefs.SetInt(PlayerPrefValues.StartupAudio, 1);
            PlayerPrefs.SetInt(PlayerPrefValues.Music, 1);
            PlayerPrefs.SetInt(PlayerPrefValues.SFX, 1);
        }
        //PlayerPrefs.SetInt(PlayerPrefValues.Music, 1);
        foreach (Sound s in sounds)
        {
            if (s.PlayOnAwake == true)
            {
                s.source.Play();
                s.source.volume *= s.BackgroundMusic ? PlayerPrefs.GetInt(PlayerPrefValues.Music) : PlayerPrefs.GetInt(PlayerPrefValues.SFX);
            }
        }
    }

    public void StopAllBackgroundMusic()
    {
        foreach (Sound s in sounds)
        {
            if (s.BackgroundMusic == true)
            {
                StartCoroutine(FadeAndStop(s));
            }
        }
    }

    public void PlayBackgroundMusic(Config.Types.SFX sfxType)
    {
        foreach (Sound s in sounds)
        {
            if (s.BackgroundMusic == true && s.sfxType != sfxType)
            {
                StartCoroutine(FadeAndStop(s));
            }
        }
        Sound S = Array.Find(sounds, sound => sound.sfxType == sfxType);
        StartCoroutine(FadeAndStart(S, 0));
    }

    public void PlayByVolume(Config.Types.SFX sfxType, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.sfxType == sfxType);
        s.source.volume = volume;
    }
    public void StopByVolume(Config.Types.SFX sfxType)
    {
        Sound s = Array.Find(sounds, sound => sound.sfxType == sfxType);
        s.source.volume = 0;
    }

    public void ToggleByVolume(Config.Types.SFX sfxType)
    {
        Sound s = Array.Find(sounds, sound => sound.sfxType == sfxType);
        if (s.source.volume > 0)
        {
            s.source.volume = 0;
        }
        else
        {
            s.source.volume = 0.025f;
        }
    }


    public IEnumerator FadeAndStart(Sound sound, float volume = 0)
    {
        if (sound.volume >= 0 && volume == 0)
        {
            float vol = sound.startingVolume * (sound.BackgroundMusic ? PlayerPrefs.GetInt(PlayerPrefValues.Music) : PlayerPrefs.GetInt(PlayerPrefValues.SFX)); ;
            sound.source.volume = 0; 
            sound.source.Play();
            while (sound.source.volume <= vol)
            {
                sound.source.volume += Time.deltaTime / 8;
                yield return null;
            }
            sound.source.volume = vol;
        }
        else if (volume > 0)
        {
            sound.source.volume = sound.startingVolume * (sound.BackgroundMusic ? PlayerPrefs.GetInt(PlayerPrefValues.Music) : PlayerPrefs.GetInt(PlayerPrefValues.SFX));
            sound.source.Play();
            while (sound.source.volume < volume)
            {
                sound.source.volume += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            Debug.LogError("Trying to fade and start an audio that has no volume or volume parameter is null");
        }
    }

    public IEnumerator FadeAndStop(Sound sound)
    {
        while (sound.source.volume > 0)
        {
            sound.source.volume -= Time.deltaTime;
            yield return null;
        }
        sound.source.Stop();
    }

    public void Play(Config.Types.SFX sfxType)
    {
        Sound s = Array.Find(sounds, sound => sound.sfxType == sfxType);

        if (s == null)
            return;
        s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];

        if (s == null)
        {
            return;
        }
        if (s.RandomizePitchModerately)
        {
            s.source.pitch = s.pitch - UnityEngine.Random.Range(-0.05f, 0.05f);
        }
        s.source.volume = s.startingVolume * (s.BackgroundMusic ? PlayerPrefs.GetInt(PlayerPrefValues.Music) : PlayerPrefs.GetInt(PlayerPrefValues.SFX));
        s.source.Play();
    }

    public void Play(int sfx)
    {
       Config.Types.SFX sfxType = (Config.Types.SFX)sfx;
        Sound s = Array.Find(sounds, sound => sound.sfxType == sfxType);

        if (s == null)
            return;
        s.source.clip = s.clips[UnityEngine.Random.Range(0, s.clips.Length)];

        if (s == null)
        {
            return;
        }
        if (s.RandomizePitchModerately)
        {
            s.source.pitch = s.pitch - UnityEngine.Random.Range(-0.05f, 0.05f);
        }
        s.source.volume = s.startingVolume * (s.BackgroundMusic ? PlayerPrefs.GetInt(PlayerPrefValues.Music) : PlayerPrefs.GetInt(PlayerPrefValues.SFX));
        s.source.Play();
    }

    public void Stop(Config.Types.SFX sfxType)
    {
        try
        {
            Sound s = Array.Find(sounds, sound => sound.sfxType == sfxType);
            s.source.Stop();
        }
        catch
        {
            Debug.Log(name);
        }
    }

    public void SetVolumes()
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = s.startingVolume * (s.BackgroundMusic ? PlayerPrefs.GetInt(PlayerPrefValues.Music) : PlayerPrefs.GetInt(PlayerPrefValues.SFX));
        }
    }
}