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

    private List<RoomInfo> currentRoomList = new List<RoomInfo>();

    private void Awake()
    {
        refreshButton.onClick.AddListener(OnRefreshButtonClick);
        createButton.onClick.AddListener(OnCreateButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        partyMemberUIOpenButton.onClick.AddListener(OnPartyMemberUIOpenButtonClick);
    }

    private void OnPartyMemberUIOpenButtonClick()
    {
        PanelManager.Instance.PanelOpen("PartyMember");
    }

    private void Update()
    {
        //createButton.interactable = !PartyManager.Instance.isInParty;
        //partyMemberUIOpenButton.interactable = PartyManager.Instance.isInParty;
    }

    private void OnRefreshButtonClick()
    {
        //UpdateRoomList(PanelManager.Instance.currentRoomInfoList);
    }

    private void OnCreateButtonClick()
    {
        PanelManager.Instance.PanelOpen("CreateParty");
    }

    private void OnCloseButtonClick()
    {
        PanelManager.Instance.PanelOpen("Lobby");

    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {

        foreach (Transform child in partyListContainer)
        {
            Destroy(child.gameObject);
        }

        List<RoomInfo> destroyCanditate = new List<RoomInfo>();
        destroyCanditate = currentRoomList.FindAll(x => false == roomList.Contains(x));

        foreach (RoomInfo roomInfo in roomList)
        {
            if (currentRoomList.Contains(roomInfo)) continue;
            AddRoomButton(roomInfo);
        }

        foreach (Transform child in partyListContainer)
        {
            if (destroyCanditate.Exists(x => x.Name == child.name))
                Destroy(child.gameObject);
        }

        currentRoomList = roomList;

        //PhotonNetwork.GetCustomRoomList();

        //if (PhotonManager.Instance.partyRoomInfoList != null)
        //{
        //    foreach (PartyInfo party in PhotonManager.Instance.partyRoomInfoList)
        //    {
        //        GameObject partyItem = Instantiate(partyListItemPrefab, partyListContainer);
        //        if (partyItem.TryGetComponent(out PartyInfoDisplay component))
        //            component.Initialize(party);
        //    }
        //}

    }

    public void AddRoomButton(RoomInfo roomInfo)
    {
        GameObject joinButton = Instantiate(partyListItemPrefab, partyListContainer, false);
        joinButton.name = roomInfo.Name;
        if (joinButton.TryGetComponent(out PartyInfoDisplay component))
            component.Initialize(roomInfo);
    }
}
