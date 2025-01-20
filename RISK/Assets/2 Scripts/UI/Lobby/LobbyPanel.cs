using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    public Button createRoomButton;
    public Button FindRoomButton;


    private void Awake()
    {
        createRoomButton.onClick.AddListener(() => { PanelManager.Instance.PanelOpen("CreateParty"); });
        FindRoomButton.onClick.AddListener(() => { PanelManager.Instance.PanelOpen("PartyListBoard"); });

    }
}
