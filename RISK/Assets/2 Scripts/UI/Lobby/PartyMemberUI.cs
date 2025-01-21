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
    public Button gameStartButton;


    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);
        gameStartButton.onClick.AddListener(OnGameStartButtonClick);
        gameStartButton.interactable = false == PhotonNetwork.IsMasterClient;
    }

    private void OnGameStartButtonClick()
    {
        if (false == PhotonNetwork.IsMasterClient) return;
        else
        {
            GameManager.Instance.photonView.RPC("SetGameReady", RpcTarget.All);
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    private void Update()
    {
        if(partyMemberContainer.childCount != GameManager.Instance.connectedPlayers.Count)
        {
            print("TRIGGERED!!!!");
            UpdatePartyMembers();
        }
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
                print($"member : {member.nickName}, {member.level}, {member.classType}, {member.atk}");
                GameObject memberInfoObj = Instantiate(partyMemberInfoPrefab, partyMemberContainer);
                if (memberInfoObj.TryGetComponent(out PartyMemberInfoUI memberInfoUI))
                {
                    memberInfoUI.Initialize(member);
                }
            }
        }
    }
}
