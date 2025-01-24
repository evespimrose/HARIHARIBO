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
    public Button quitButton;
    public Button gameStartButton;


    private void Awake()
    {
        quitButton.onClick.AddListener(OnQuitButtonClick);
        gameStartButton.onClick.AddListener(OnGameStartButtonClick);
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
        gameStartButton.interactable = !false == PhotonNetwork.IsMasterClient;

        if (partyMemberContainer.childCount != GameManager.Instance.connectedPlayers.Count)
        {
            print("TRIGGERED!!!!");
            UpdateRoomMembers();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.chat.gameObject.SetActive(true);
    }

    public override void OnDisable()
    {
        foreach (Transform child in partyMemberContainer)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    public IEnumerator Start()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        yield return new WaitForSeconds(1f);

        UpdateRoomMembers();
    }

    private void OnQuitButtonClick()
    {
        LobbyUI.Instance.PopupOpen<PopupPanel>().SetPopup("Party Quit", "SuccessFully Left Party.", () => { OnCloseButtonClick(); PanelManager.Instance.PopupClose(); });
    }

    private void OnCloseButtonClick()
    {
        PanelManager.Instance.PanelOpen("PartyListBoard");
    }

    public void UpdateRoomMembers()
    {
        foreach (Transform child in partyMemberContainer)
        {
            Destroy(child.gameObject);
        }

        if (PhotonNetwork.CurrentRoom.Players.Count > 0)
        {
            foreach (var member in GameManager.Instance.connectedPlayers)
            {
                GameObject memberInfoObj = Instantiate(partyMemberInfoPrefab, partyMemberContainer);
                if (memberInfoObj.TryGetComponent(out PartyMemberInfoUI memberInfoUI))
                {
                    memberInfoUI.Initialize(member);
                }
            }
        }
    }
}
