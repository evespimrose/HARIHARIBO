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

    private string openPanel = null;

    private Dictionary<string, GameObject> panelDic;
    private Dictionary<string, GameObject> popupDic;
    private Stack<UIPopup> openPopups = new Stack<UIPopup>();

    private void Awake()
    {
        Instance = this;
        panelDic = new Dictionary<string, GameObject>()
        {
            { "Login", login.gameObject },
            { "Signup", signup.gameObject },
            { "SelectCharacter", selectCharacter.gameObject },
            { "CreateCharacter", createCharacter.gameObject }
        };

        popupDic = new Dictionary<string, GameObject>()
        {
            { "Popup", popup.gameObject },
            { "TwoButtonPopup", twoButtonPopup.gameObject }
        };

        PanelOpen("Login");
    }

    public void PanelOpen(string panelName)
    {
        if (openPanel == panelName) return;

        if (openPanel != null && panelDic.TryGetValue(openPanel, out var previousPanel))
        {
            previousPanel.SetActive(false);
        }

        if (panelDic.TryGetValue(panelName, out var newPanel))
        {
            newPanel.SetActive(true);
            openPanel = panelName;
        }
        else
        {
            Debug.LogWarning($"Panel '{panelName}' not found in panelDic.");
        }
    }

    public T PopupOpen<T>() where T : UIPopup
    {
        foreach (GameObject popup in popupDic.Values)
        {
            if (popup.TryGetComponent(out T popupComponent))
            {
                if (!popup.activeSelf) popup.SetActive(true);
                openPopups.Push(popupComponent);
                return popupComponent;
            }
        }

        return null;
    }

    public void PopupClose()
    {
        if (openPopups.Count > 0)
        {
            UIPopup targetPopup = openPopups.Pop();
            targetPopup.gameObject.SetActive(false);
        }

    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnConnected()
    { PanelOpen("Channel"); }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PanelOpen("Login");
    }
    //public override void OnCreatedRoom()
    //{
    //    PanelOpen("Room");

    //}
    //public override void OnJoinedRoom()
    //{
    //    PanelOpen("Room");
    //    HashTable roomCustomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
    //    if (roomCustomProperties.ContainsKey("Difficulty"))
    //    {
    //        //room.OnDifficultyChange((Difficulty)roomCustomProperties["Difficulty"]);
    //    }
    //}
    //public override void OnLeftRoom() //�濡�� ������ �� ȣ��
    //{
    //    PanelOpen("Menu");
    //}
    //public override void OnPlayerEnteredRoom(PhotonRealtimePlayer newPlayer)
    //{
    //    //room.JoinPlayer(newPlayer);
    //}
    //public override void OnPlayerLeftRoom(PhotonRealtimePlayer otherPlayer)
    //{
    //    //room.LeavePlayer(otherPlayer);
    //}
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



