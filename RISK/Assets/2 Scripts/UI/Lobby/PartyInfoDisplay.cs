using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PartyInfoDisplay : MonoBehaviour
{
    public TextMeshProUGUI currentPartyMember;
    public TextMeshProUGUI maxPartyMember;
    public TextMeshProUGUI partyName;
    public TextMeshProUGUI partyGoal;
    public TextMeshProUGUI minPartyLevel;
    public TextMeshProUGUI maxPartyLevel;

    public Button partyJoinButton;

    public void Awake()
    {
    }

    public void Initialize(string Name, int currentMember, int maxMember, int goal, int minLevel, int maxLevel)
    {
        partyName.text = Name;
        currentPartyMember.text = currentMember.ToString();
        partyGoal.text = goal.ToString();
        minPartyLevel.text = minLevel.ToString();
        maxPartyLevel.text = maxLevel.ToString();
        maxPartyMember.text = maxMember.ToString();
    }

    public void Initialize(PartyInfo partyInfo)
    {
        partyName.text = partyInfo.name;
        currentPartyMember.text = partyInfo.currentMemberCount.ToString();
        partyGoal.text = (partyInfo.goal + 1).ToString();
        minPartyLevel.text = partyInfo.minPartyLevel.ToString();
        maxPartyLevel.text = partyInfo.maxPartyLevel.ToString();
        maxPartyMember.text = partyInfo.maxPartyMemberCount.ToString();
    }

    public void Initialize(RoomInfo roomInfo)
    {
        //this.partyInfo = partyInfo;

        PartyInfo partyInfo = new PartyInfo();

        partyInfo.name = roomInfo.Name;
        partyInfo.currentMemberCount = roomInfo.PlayerCount;
        partyInfo.maxPartyMemberCount = roomInfo.MaxPlayers;

        if (roomInfo.CustomProperties.ContainsKey("roomId"))
            partyInfo.partyId = (int)roomInfo.CustomProperties["roomId"];
        if (roomInfo.CustomProperties.ContainsKey("Difficulty"))
            partyInfo.goal = (int)roomInfo.CustomProperties["Difficulty"];
        if (roomInfo.CustomProperties.ContainsKey("minLevel"))
            partyInfo.minPartyLevel = (int)roomInfo.CustomProperties["minLevel"];
        if (roomInfo.CustomProperties.ContainsKey("maxLevel"))
            partyInfo.maxPartyLevel = (int)roomInfo.CustomProperties["maxLevel"];

        partyName.text = roomInfo.Name;
        currentPartyMember.text = partyInfo.currentMemberCount.ToString();
        partyGoal.text = (partyInfo.goal + 1).ToString();
        minPartyLevel.text = partyInfo.minPartyLevel.ToString();
        maxPartyLevel.text = partyInfo.maxPartyLevel.ToString();
        maxPartyMember.text = partyInfo.maxPartyMemberCount.ToString();

        partyJoinButton.onClick.AddListener(() => { PhotonNetwork.JoinRoom(roomInfo.Name); PanelManager.Instance.PanelOpen("PartyMember"); });
    }

    PartyInfoDisplay() { }

    PartyInfoDisplay(string Name, int currentMember, int maxMember, int goal, int minLevel, int maxLevel)
    {
        partyName.text = Name;
        currentPartyMember.text = currentMember.ToString();
        partyGoal.text = goal.ToString();
        minPartyLevel.text = minLevel.ToString();
        maxPartyLevel.text = maxLevel.ToString();
        maxPartyMember.text = maxMember.ToString();
    }

    PartyInfoDisplay(PartyInfo partyInfo)
    {
        partyName.text = partyInfo.name;
        currentPartyMember.text = partyInfo.currentMemberCount.ToString();
        partyGoal.text = "goal.ToString()";
        minPartyLevel.text = "minLevel.ToString()";
        maxPartyLevel.text = "maxLevel.ToString()";
        maxPartyMember.text = partyInfo.maxPartyMemberCount.ToString();
    }
}
