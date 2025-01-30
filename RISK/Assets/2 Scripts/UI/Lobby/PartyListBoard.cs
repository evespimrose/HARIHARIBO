using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    private void OnPartyMemberUIOpenButtonClick()
    {
        PanelManager.Instance.PanelOpen("PartyMember");
    }

    public override void OnEnable()
    {
        base.OnEnable();
        refreshButton.onClick.AddListener(OnRefreshButtonClick);
        createButton.onClick.AddListener(OnCreateButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        partyMemberUIOpenButton.onClick.AddListener(OnPartyMemberUIOpenButtonClick);
        upgadeButton.onClick.AddListener(CharacterUpgradeOpen);

    }

    public override void OnDisable()
    {
        base.OnDisable();
        refreshButton.onClick.RemoveAllListeners();
        createButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();
        partyMemberUIOpenButton.onClick.RemoveAllListeners();
        upgadeButton.onClick.RemoveAllListeners();
        UpdateRoomList(currentRoomList);
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

    public void UpdateRoomList(List<RoomInfo> roomList)
    {

        foreach (Transform child in partyListContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.CustomProperties.TryGetValue("IsPlaying", out object isPlaying) && (bool)isPlaying)
                continue;

            AddRoomButton(roomInfo);
        }

        currentRoomList = roomList;

    }

    public void UpdateRoomList()
    {
        currentRoomList.Clear();

        foreach (Transform child in partyListContainer)
        {
            DestroyImmediate(child.gameObject);
        }

        foreach (RoomInfo roomInfo in PanelManager.Instance.currentRoomInfoList)
        {
            if (roomInfo.CustomProperties.TryGetValue("IsPlaying", out object isPlaying) && (bool)isPlaying)
                continue;

            AddRoomButton(roomInfo);
        }

        currentRoomList = PanelManager.Instance.currentRoomInfoList;

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
