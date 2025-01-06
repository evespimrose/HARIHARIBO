using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HashTable = ExitGames.Client.Photon.Hashtable;
using PhotonRealtimePlayer = Photon.Realtime.Player;


public class PanelManager : MonoBehaviourPunCallbacks
{
    public static PanelManager Instance;

    public LoginPanel login;
    public SignupPanel signup;
    public SelectCharacterPanel selectCharacter;
    public CreateCharacterPanel createCharacter;
    public PopupPanel popup;
    public TwoButtonPopupPanel twoButtonPopup;
    public ChannelPanel channel;

    Dictionary<string, GameObject> panelDic;

    private void Awake()
    {
        Instance = this;
        panelDic = new Dictionary<string, GameObject>()
        {
            { "Login", login.gameObject},
            { "Signup", signup.gameObject},
            { "SelectCharacter", selectCharacter.gameObject},
            { "Popup" , popup.gameObject },
            { "CreateCharacter", createCharacter.gameObject},
            { "TwoButtonPopup", twoButtonPopup.gameObject},
            { "Channel", channel.gameObject},
        };

        PanelOpen("Login");
    }

    public void PanelOpen(string panelName)
    {
        foreach (var row in panelDic)
        {
            row.Value.SetActive(row.Key == panelName);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        //MonoBehaviourPunCallbacks를 상속한 클래스는 OnEnable을
        //재정의 할 때 꼭 부모의 OnEnable을 호출해야 함
        //print("하이");
    }

    public override void OnConnected() //포톤 서버에 접속 되었을 때 호출
    {
        PanelOpen("Menu");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PanelOpen("Login");
    }
    public override void OnCreatedRoom() //방을 생성하였을때 호출
    {
        PanelOpen("Room");

    }
    public override void OnJoinedRoom() //방에 참여
    {
        PanelOpen("Room");
        HashTable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        if (roomCustomProperties.ContainsKey("Difficulty"))
        {
            //room.OnDifficultyChange((Difficulty)roomCustomProperties["Difficulty"]);
        }
    }
    public override void OnLeftRoom() //방에서 떠났을 때 호출
    {
        PanelOpen("Menu");
    }
    public override void OnPlayerEnteredRoom(PhotonRealtimePlayer newPlayer)
    {
        //room.JoinPlayer(newPlayer);
    }
    public override void OnPlayerLeftRoom(PhotonRealtimePlayer otherPlayer)
    {
        //room.LeavePlayer(otherPlayer);
    }
    public override void OnJoinedLobby()
    {
        PanelOpen("Lobby");
    }
    public override void OnLeftLobby()
    {
        PanelOpen("Menu");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //lobby.UpdateRoomList(roomList);
    }

    public override void OnRoomPropertiesUpdate(HashTable properties)
    {
        if (properties.ContainsKey("Difficulty"))
        {
            //room.OnDifficultyChange((Difficulty)properties["Difficulty"]);
        }
    }

    public override void OnPlayerPropertiesUpdate(PhotonRealtimePlayer targetPlayer, HashTable changedProps)
    {
        if (changedProps.ContainsKey("CharacterSelect"))
        {
            //room.OnCharacterSelectChange(targetPlayer, changedProps);
        }
    }
}




