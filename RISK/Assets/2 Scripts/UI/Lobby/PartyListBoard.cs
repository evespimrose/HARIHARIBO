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

    private void Awake()
    {
        refreshButton.onClick.AddListener(OnRefreshButtonClick);
        createButton.onClick.AddListener(OnCreateButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
        partyMemberUIOpenButton.onClick.AddListener(OnPartyMemberUIOpenButtonClick);
    }

    private void OnPartyMemberUIOpenButtonClick()
    {
        LobbyUI.Instance.PanelOpen("PartyMember");
    }

    private void Update()
    {
        createButton.interactable = !PartyManager.Instance.isInParty;
        partyMemberUIOpenButton.interactable = PartyManager.Instance.isInParty;
    }

    private void OnRefreshButtonClick()
    {
        PhotonManager.Instance.partyRoomInfoList = PhotonManager.Instance.GetPartyList();

        UpdatePartyList();
    }

    private void OnCreateButtonClick()
    {
        LobbyUI.Instance.PanelOpen("CreateParty");
    }

    private void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        UpdatePartyList();
    }

    public void UpdatePartyList()
    {
        foreach (Transform child in partyListContainer)
        {
            Destroy(child.gameObject);
        }

        if (PhotonManager.Instance.partyRoomInfoList != null)
        {
            foreach (PartyInfo party in PhotonManager.Instance.partyRoomInfoList)
            {
                GameObject partyItem = Instantiate(partyListItemPrefab, partyListContainer);
                if (partyItem.TryGetComponent(out PartyInfoDisplay component))
                    component.Initialize(party);
            }
        }
    }
}
