using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyListBoard : MonoBehaviourPunCallbacks
{
    public GameObject partyListItemPrefab;
    public Transform partyListContainer;

    public Button refreshButton;
    public Button createButton;
    public Button closeButton;
    public Button partyMemberUIOpenButton;
    public Button upgadeButton;

    private List<RoomInfo> currentRoomList = new List<RoomInfo>();

    private void Awake()
    {
        refreshButton.onClick.AddListener(OnRefreshButtonClick);
        createButton.onClick.AddListener(OnCreateButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        partyMemberUIOpenButton.onClick.AddListener(OnPartyMemberUIOpenButtonClick);
        upgadeButton.onClick.AddListener(CharacterUpgradeOpen);
    }

    private void OnPartyMemberUIOpenButtonClick()
    {
        PanelManager.Instance.PanelOpen("PartyMember");
    }

    private void Update()
    {
    }

    private void OnRefreshButtonClick()
    {
        
    }

    private void OnCreateButtonClick()
    {
        PanelManager.Instance.PanelOpen("CreateParty");
    }

    private void OnCloseButtonClick()
    {
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {

        foreach (Transform child in partyListContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        foreach (RoomInfo roomInfo in roomList)
        {
            AddRoomButton(roomInfo);
        }

        currentRoomList = roomList;

    }

    public void AddRoomButton(RoomInfo roomInfo)
    {
        GameObject joinButton = Instantiate(partyListItemPrefab, partyListContainer, false);
        joinButton.name = roomInfo.Name;
        if (joinButton.TryGetComponent(out PartyInfoDisplay component))
            component.Initialize(roomInfo);
    }

    public void CharacterUpgradeOpen()
    {
        PanelManager.Instance.PanelOpen("CharacterUpgrade");
    }
}
