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

    public Slider volumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        gameExitButton.onClick.AddListener(ConfirmExit);
        yesButton.onClick.AddListener(GameExit);
        noButton.onClick.AddListener(CancelExit);
        confirmExitPanel.SetActive(false);

        VolumeSeting();
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VolumeSeting()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");
            AudioListener.volume = savedVolume;
            volumeSlider.value = savedVolume;
        }
        else
        {
            AudioListener.volume = 0.5f;
            volumeSlider.value = 0.5f;
        }
    }

    public void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
        Debug.Log("볼륨 변경: " + volume);
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
