using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PhotonRoomViewer : MonoBehaviourPunCallbacks
{
    [Header("UI Elements")]
    public string roomInfoText;
    public string playerInfoText;

    private List<RoomInfo> roomList = new List<RoomInfo>();

    private void Start()
    {
        UpdateRoomInfo();
    }

    private void UpdateRoomInfo()
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

    public override void OnRoomListUpdate(List<RoomInfo> rooms)
    {
        roomList = rooms;
        UpdateRoomInfo();
    }
}