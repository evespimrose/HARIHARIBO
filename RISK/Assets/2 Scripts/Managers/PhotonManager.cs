using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HashTable = ExitGames.Client.Photon.Hashtable;

public class PhotonManager : MonoBehaviourPunCallbacks
{
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

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room Failed: {message}");
    }

    public void CreateDungeonRoom(string dungeonName)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new HashTable { { "RoomType", "Dungeon" }, { "DungeonLevel", 1 } },
            CustomRoomPropertiesForLobby = new string[] { "RoomType", "DungeonLevel" }
        };

        PhotonNetwork.CreateRoom(dungeonName, roomOptions, null);
    }
    public void GetDungeonRoomList()
    {
        string sqlLobbyFilter = "RoomType = 'Dungeon'";
        PhotonNetwork.GetCustomRoomList(TypedLobby.Default, sqlLobbyFilter);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (room.CustomProperties.ContainsKey("RoomType") &&
                room.CustomProperties["RoomType"].ToString() == "Dungeon")
            {
                Debug.Log($"Dungeon Room Found: {room.Name}");
            }
        }
    }

}
