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
    public Transform roomListContent;
    public GameObject roomListItemPrefab;

    public string roomInfoText;
    public string playerInfoText;

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

    public void CreatePartyRoom(string dungeonName)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new HashTable { { "RoomType", "Party" }, { "DungeonLevel", 1 } },
            CustomRoomPropertiesForLobby = new string[] { "RoomType", "DungeonLevel" }
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
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        UpdateRoomInfo(roomList);

        foreach (RoomInfo roomInfo in roomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListContent);
            roomItem.GetComponentInChildren<TMP_Text>().text = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";

            if (roomInfo.CustomProperties.ContainsKey("RoomType") && roomInfo.CustomProperties["RoomType"].ToString() == "Dungeon")
            {
                Debug.Log($"Dungeon Room Found: {roomInfo.Name}");
            }

            Button joinButton = roomItem.GetComponentInChildren<Button>();
            joinButton.onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));
        }
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Dungeon room created successfully.");

        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RoomType") 
            && PhotonNetwork.CurrentRoom.CustomProperties["RoomType"].ToString() == "Dungeon" 
            && PartyManager.Instance.IsPartyLeader(PhotonNetwork.LocalPlayer))
        {
            foreach (PhotonRealtimePlayer member in PartyManager.Instance.GetPartyMembers())
            {
                photonView.RPC("ForceJoinDungeon", member, PhotonNetwork.CurrentRoom.Name);
            }
        }
    }

    [PunRPC]
    private void ForceJoinDungeon(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined dungeon room: {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.LoadLevel("DungeonScene");
    }
}
