using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingClose : MonoBehaviour
{
    private Button closeButton;

    public GameObject settingPanel;

    // Start is called before the first frame update
    void Start()
    {
        closeButton = this.gameObject.GetComponent<Button>();
        closeButton.onClick.AddListener(CloseSettingPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }
}
