using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSoundManager : SingletonManager<GameSoundManager>
{
    //audio sources of background music and sound effects
    public AudioSource backgroundMusicSource;
    public AudioSource bossEffectSoundSource;
    public AudioSource[] monsterEffectSoundSources;

    public AudioClip backgroundMusicClip;

    void Start()
    {
        LoadSettings();
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicClip != null)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.loop = true;  
            backgroundMusicSource.Play();      
        }
    }

    public void PlayMonsterEffectSound(AudioClip clip)
    {
        if (clip != null)
        {
            PlayWithEffectSources(clip, monsterEffectSoundSources);
        }
        else
        {
            Debug.LogError("Sound not found.");
        }
    }

    public void PlayBossEffectSound(AudioClip clip)
    {
        if (clip != null)
        {
            //If the boss sound source is already playing, stop and play a new one
            if (bossEffectSoundSource.isPlaying)
            {
                bossEffectSoundSource.Stop();
            }
            bossEffectSoundSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("Sound not found.");
        }
    }

    private void PlayWithEffectSources(AudioClip clip, AudioSource[] effectSources)
    {
        foreach (var source in effectSources)
        {
            if (!source.isPlaying) //Find a source that is not playing and play it
            {
                source.PlayOneShot(clip);
                return;
            }
        }

        //If all the sources are playing, stop the oldest source and play new sound
        StopAndPlayNewSound(clip, effectSources);
    }

    private void StopAndPlayNewSound(AudioClip clip, AudioSource[] effectSources)
    {
        //Stop the source that is playing first and then play a new one
        AudioSource oldestSource = effectSources.OrderBy(source => source.time).FirstOrDefault();

        if (oldestSource != null)
        {
            oldestSource.Stop();  //Stop the oldest sound
            oldestSource.PlayOneShot(clip);  //Play New Sound
        }
        else
        {
            Debug.LogError("Sound effect source not found");
        }
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = volume;
            PlayerPrefs.SetFloat("BackgroundMusicVolume", volume);  
        }
    }

    public void SetEffectSoundVolume(float volume)
    {
        foreach (var source in monsterEffectSoundSources)
        {
            source.volume = volume;
        }
        if (bossEffectSoundSource != null)
        {
            bossEffectSoundSource.volume = volume;
        }
        PlayerPrefs.SetFloat("EffectSoundVolume", volume); 
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("BackgroundMusicVolume"))
        {
            float savedBackgroundMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume");
            SetBackgroundMusicVolume(savedBackgroundMusicVolume);
        }
        else
        {
            SetBackgroundMusicVolume(0.5f); 
        }

        if (PlayerPrefs.HasKey("EffectSoundVolume"))
        {
            float savedEffectSoundVolume = PlayerPrefs.GetFloat("EffectSoundVolume");
            SetEffectSoundVolume(savedEffectSoundVolume);
        }
        else
        {
            SetEffectSoundVolume(0.5f); 
        }
    }
}

