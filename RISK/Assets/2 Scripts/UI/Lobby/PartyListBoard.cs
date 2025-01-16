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

    private void Awake()
    {
        refreshButton.onClick.AddListener(OnRefreshButtonClick);
        createButton.onClick.AddListener(OnCreateButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnRefreshButtonClick()
    {

    }

    private void OnCreateButtonClick()
    {
        LobbyUI.Instance.PanelOpen("CreateParty");
    }

    private void OnCloseButtonClick()
    {

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
                partyItem.GetComponentInChildren<TextMeshProUGUI>().text = party.name;
                //Button joinButton = partyItem.GetComponentInChildren<Button>();
                //joinButton.onClick.AddListener(() => PartyManager.Instance.JoinParty(PhotonNetwork.LocalPlayer));
            }
        }

    }

}
