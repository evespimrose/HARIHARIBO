using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.UI;
using PhotonRealtimePlayer = Photon.Realtime.Player;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] private Transform partyMemberContainer;
    [SerializeField] private GameObject partyMemberInfoPrefab;
    public Button closeButton;
    public Button quitButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
    }
    private void OnEnable()
    {
        UpdatePartyMembers();
    }

    private void OnQuitButtonClick()
    {
        PartyManager.Instance.LeaveParty(PhotonNetwork.LocalPlayer);
    }

    private void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }

    public void UpdatePartyMembers()
    {
        foreach (Transform child in partyMemberContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var member in PartyManager.Instance.GetPartyMembers())
        {
            GameObject memberInfoObj = Instantiate(partyMemberInfoPrefab, partyMemberContainer);
            if (memberInfoObj.TryGetComponent(out PartyMemberInfoUI memberInfoUI))
            {
                memberInfoUI.Initialize(member);
            }
        }
    }
}
