using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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
        partyJoinButton.onClick.AddListener(OnPartyJoinButtonClick);
    }

    private void OnPartyJoinButtonClick()
    {
        PartyManager.Instance.JoinParty(PhotonNetwork.LocalPlayer);
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
}
