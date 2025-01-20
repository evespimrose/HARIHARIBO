using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PhotonRealtimePlayer = Photon.Realtime.Player;

public class PartyMemberUI : MonoBehaviourPunCallbacks
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
    public override void OnEnable()
    {
        base.OnEnable();
    }

    public IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        yield return new WaitForSeconds(1f);

        UpdatePartyMembers();
    }

    private void OnQuitButtonClick()
    {
        PartyManager.Instance.LeaveParty(PhotonNetwork.LocalPlayer);
        LobbyUI.Instance.PopupOpen<PopupPanel>().SetPopup("Party Quit", "SuccessFully Left Party.", () => { OnCloseButtonClick(); LobbyUI.Instance.PopupClose(); });
    }

    private void OnCloseButtonClick()
    {
        PanelManager.Instance.PanelOpen("PartyListBoard");
    }

    public void UpdatePartyMembers()
    {
        foreach (Transform child in partyMemberContainer)
        {
            Destroy(child.gameObject);
        }

        if (PhotonNetwork.CurrentRoom.Players.Count > 0)
        {
            print("UpdatePartyMembers");
            foreach (var member in GameManager.Instance.connectedPlayers)
            {
                print("connectedPlayersconnectedPlayers");

                GameObject memberInfoObj = Instantiate(partyMemberInfoPrefab, partyMemberContainer);
                if (memberInfoObj.TryGetComponent(out PartyMemberInfoUI memberInfoUI))
                {
                    memberInfoUI.Initialize(member);
                }
            }
        }
    }
}
