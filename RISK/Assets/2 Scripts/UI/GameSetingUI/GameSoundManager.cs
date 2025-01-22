using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSoundManager : SingletonManager<GameSoundManager>
{
    // 배경음악과 효과음의 오디오 소스
    public AudioSource backgroundMusicSource;
    public AudioSource bossEffectSoundSource;
    public AudioSource[] monsterEffectSoundSources;

    public AudioClip backgroundMusicClip;

    // Start는 한 번만 호출됨, 게임 시작 시 저장된 값을 불러옵니다.
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
            backgroundMusicSource.loop = true;  // 배경 음악은 반복 재생
            backgroundMusicSource.Play();       // 배경 음악 재생
        }
    }

    // 몬스터 히트, 다이 사운드 재생
    public void PlayMonsterEffectSound(AudioClip clip)
    {
        if (clip != null)
        {
            PlayWithEffectSources(clip, monsterEffectSoundSources);
        }
        else
        {
            Debug.LogError("사운드를 찾을 수 없습니다.");
        }
    }

    // 보스 스킬 사운드 재생
    public void PlayBossEffectSound(AudioClip clip)
    {
        if (clip != null)
        {
            // 보스 효과음 소스가 이미 재생 중이면 중지하고 새로 재생
            if (bossEffectSoundSource.isPlaying)
            {
                bossEffectSoundSource.Stop();
            }
            bossEffectSoundSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("사운드를 찾을 수 없습니다.");
        }
    }

    private void PlayWithEffectSources(AudioClip clip, AudioSource[] effectSources)
    {
        foreach (var source in effectSources)
        {
            if (!source.isPlaying) // 재생 중이지 않은 소스를 찾아서 재생
            {
                source.PlayOneShot(clip);
                return;
            }
        }

        // 만약 모든 소스가 재생 중이면, 가장 오래된 소스를 중지하고 새 소리 재생
        StopAndPlayNewSound(clip, effectSources);
    }

    private void StopAndPlayNewSound(AudioClip clip, AudioSource[] effectSources)
    {
        // 가장 먼저 재생 중인 소스를 중지하고 새로 재생
        AudioSource oldestSource = effectSources.OrderBy(source => source.time).FirstOrDefault();

        if (oldestSource != null)
        {
            oldestSource.Stop();  // 가장 오래된 소리 중지
            oldestSource.PlayOneShot(clip);  // 새 소리 재생
        }
        else
        {
            Debug.LogError("효과음 소스를 찾을 수 없습니다.");
        }
    }

    // 배경음악 볼륨 설정
    public void SetBackgroundMusicVolume(float volume)
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = volume;
            PlayerPrefs.SetFloat("BackgroundMusicVolume", volume);  // PlayerPrefs에 저장
        }
    }

    // 볼륨 설정 (효과음 전체 소리 크기)
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
        PlayerPrefs.SetFloat("EffectSoundVolume", volume);  // 설정값 저장
    }

    // 저장된 볼륨 설정을 불러오기
    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("BackgroundMusicVolume"))
        {
            float savedBackgroundMusicVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume");
            SetBackgroundMusicVolume(savedBackgroundMusicVolume);
        }
        else
        {
            SetBackgroundMusicVolume(0.5f);  // 기본값 설정
        }

        if (PlayerPrefs.HasKey("EffectSoundVolume"))
        {
            float savedEffectSoundVolume = PlayerPrefs.GetFloat("EffectSoundVolume");
            SetEffectSoundVolume(savedEffectSoundVolume);
        }
        else
        {
            SetEffectSoundVolume(0.5f);  // 기본값 설정
        }
    }
}

