using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HashTable = ExitGames.Client.Photon.Hashtable;
using PhotonRealtimePlayer = Photon.Realtime.Player;

public class PhotonManager : PhotonSingletonManager<PhotonManager>
{
    //public Transform dungeonroomListContent;
    //public GameObject dungeonroomListItemPrefab;
    public Transform partyroomListContent;
    public GameObject partyroomListItemPrefab;

    public string roomInfoText;
    public string playerInfoText;

    //public List<RoomInfo> dungeonRoomInfoList = new List<RoomInfo>();
    public List<RoomInfo> partyRoomInfoList = new List<RoomInfo>();

    private void Start()
    {
        //Active at GameScene
        //GameManager.isGameReady = true;
    }

    public override void OnConnectedToMaster()
    {
        string roomName = "LobbyRoom";

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new HashTable { { "Lobby", true } },
            CustomRoomPropertiesForLobby = new string[] { "Lobby" }
        };

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room Failed: {message}");
    }

    public void CreatePartyRoom(string dungeonName, int difficulty)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new HashTable { { "RoomType", "Party" }, { "Difficulty", difficulty } },
            CustomRoomPropertiesForLobby = new string[] { "RoomType", "Difficulty" }
        };

        PhotonNetwork.CreateRoom(dungeonName, roomOptions, TypedLobby.Default);
    }
    public void GetPartyRoomList()
    {
        string sqlLobbyFilter = "RoomType = 'Party'";
        PhotonNetwork.GetCustomRoomList(TypedLobby.Default, sqlLobbyFilter);
    }

    public void CreateDungeonRoom()
    {
        if (!PartyManager.Instance.IsPartyLeader(PhotonNetwork.LocalPlayer))
        {
            Debug.LogError("Only the party leader can create a dungeon room.");
            return;
        }

        string roomName = "Dungeon_" + Random.Range(1000, 9999);

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)PartyManager.Instance.GetPartyMembers().Count,
            IsVisible = false,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    private void UpdateRoomInfo(List<RoomInfo> roomList)
    {
        roomInfoText = "Rooms:\n";
        playerInfoText = "Players:\n";

        foreach (RoomInfo room in roomList)
        {
            roomInfoText += $"Room Name: {room.Name}, Players: {room.PlayerCount}/{room.MaxPlayers}\n";

            if (room.CustomProperties.Count > 0)
            {
                foreach (var property in room.CustomProperties)
                {
                    roomInfoText += $"  {property.Key}: {property.Value}\n";
                }
            }
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerInfoText += $"Player: {player.NickName}, ID: {player.ActorNumber}\n";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in partyroomListContent)
        {
            Destroy(child.gameObject);
        }
        partyRoomInfoList.Clear();

        UpdateRoomInfo(roomList);

        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.CustomProperties.ContainsKey("RoomType") && roomInfo.CustomProperties["RoomType"].ToString() == "party")
            {
                //GameObject roomItem = Instantiate(partyroomListItemPrefab, partyroomListContent);
                //roomItem.GetComponentInChildren<TMP_Text>().text = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";
                //Button joinButton = roomItem.GetComponentInChildren<Button>();
                //joinButton.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));
                partyRoomInfoList.Add(roomInfo);
            }

        }
    }
    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RoomType")
            && PhotonNetwork.CurrentRoom.CustomProperties["RoomType"].ToString() == "Dungeon"
            && PartyManager.Instance.IsPartyLeader(PhotonNetwork.LocalPlayer))
        {
            foreach (PhotonRealtimePlayer member in PartyManager.Instance.GetPartyMembers())
            {
                if (member != PhotonNetwork.LocalPlayer)
                {
                    photonView.RPC("ForceJoinDungeon", member, PhotonNetwork.CurrentRoom.Name);
                }
            }
            PhotonNetwork.JoinRoom(PhotonNetwork.CurrentRoom.Name);
        }
    }

    [PunRPC]
    private void ForceJoinDungeon(string roomName, PhotonMessageInfo info)
    {
        if (!PartyManager.Instance.GetPartyMembers().Contains(info.Sender)) return;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RoomType", out object dungeonRoomType) && dungeonRoomType.ToString() == "Dungeon")
        {
            Debug.Log($"Joined dungeon room: {PhotonNetwork.CurrentRoom.Name}");
            PhotonNetwork.LoadLevel("DungeonScene");
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RoomType", out object partyroomType) && partyroomType.ToString() == "Party")
        {
            PartyManager.Instance.JoinParty(PhotonNetwork.LocalPlayer);
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PartyManager.Instance.SetPartyLeader(PhotonNetwork.LocalPlayer);
            }
        }

        photonView.RPC("RequestExistingPlayers", RpcTarget.All);
    }

    [PunRPC]
    private void RequestExistingPlayers()
    {
        if (UnitManager.Instance.LocalPlayer != null)
        {
            photonView.RPC("SyncPlayerInfo", RpcTarget.All,
                PhotonNetwork.LocalPlayer.ActorNumber,
                UnitManager.Instance.LocalPlayer.transform.position,
                UnitManager.Instance.LocalPlayer.transform.rotation);
        }
    }

    [PunRPC]
    private void SyncPlayerInfo(int actorNumber, Vector3 position, Quaternion rotation)
    {
        if (actorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            return;

        if (!UnitManager.Instance.HasPlayer(actorNumber))
        {
            GameObject playerObj = PhotonNetwork.Instantiate("Player", position, rotation);
            UnitManager.Instance.RegisterPlayer(playerObj, actorNumber);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
        UnitManager.Instance.UnregisterPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"Player left room: {otherPlayer.NickName}");

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RoomType", out object roomType) && roomType.ToString() == "Party")
        {
            if (PartyManager.Instance.IsPartyLeader(otherPlayer))
            {
                foreach (var member in PartyManager.Instance.GetPartyMembers())
                {
                    PartyManager.Instance.LeaveParty(member);
                }
                PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Party Disbanded", $"{otherPlayer.NickName} has disbanded the party.");
            }
            else
            {
                PartyManager.Instance.LeaveParty(otherPlayer);
                PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Party Disbanded", $"{otherPlayer.NickName} has left the party.");
            }
        }
    }
}
