using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingUI : UIPopup
{

    public Button gameExitButton;
    public Button yesButton;
    public Button noButton;
    public GameObject confirmExitPanel;

    public Slider backgroundMusicSlider;
    public Slider effectSoundSlider;

    private GameSoundManager gameSound;

    void Start()
    {
        gameExitButton.onClick.AddListener(ConfirmExit);
        yesButton.onClick.AddListener(GameExit);
        noButton.onClick.AddListener(CancelExit);
        confirmExitPanel.SetActive(false);

        gameSound = GameSoundManager.Instance;

        backgroundMusicSlider.onValueChanged.AddListener(OnBackgroundMusicVolumeChanged);
        effectSoundSlider.onValueChanged.AddListener(OnEffectSoundVolumeChanged);
        VolumeSeting();
    }

    void Update()
    {
        
    }

    public void VolumeSeting()
    {
        if (gameSound != null)
        {
            // 배경음악 볼륨 설정
            backgroundMusicSlider.value = PlayerPrefs.GetFloat("BackgroundMusicVolume", 0.5f);
            // 효과음 볼륨 설정
            effectSoundSlider.value = PlayerPrefs.GetFloat("EffectSoundVolume", 0.5f);
        }
    }

    // 배경음악 볼륨 슬라이더 변경
    public void OnBackgroundMusicVolumeChanged(float volume)
    {
        if (gameSound != null)
        {
            gameSound.SetBackgroundMusicVolume(volume);  // 배경음악 볼륨 설정
        }
    }

    // 효과음 볼륨 슬라이더 변경
    public void OnEffectSoundVolumeChanged(float volume)
    {
        if (gameSound != null)
        {
            gameSound.SetEffectSoundVolume(volume); // 전체 효과음 볼륨 설정
        }
    }

    public void ConfirmExit()
    {
        confirmExitPanel.SetActive(true);
    }

    public void CancelExit()
    {
        confirmExitPanel.SetActive(false);
    }

    public void GameExit()
    {
        Debug.Log("게임종료");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
