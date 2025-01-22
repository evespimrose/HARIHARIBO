using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingOpen : MonoBehaviour
{
    private Button openButton;

    public GameObject settingPanel;

    void Start()
    {
        openButton = this.gameObject.GetComponent<Button>();
        openButton.onClick.AddListener(SettingPanelOpen);
    }

    public void SettingPanelOpen()
    {
        settingPanel.SetActive(true);
    }
}
