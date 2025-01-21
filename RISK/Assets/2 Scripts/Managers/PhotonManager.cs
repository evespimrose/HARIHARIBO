using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HashTable = ExitGames.Client.Photon.Hashtable;
using PhotonRealtimePlayer = Photon.Realtime.Player;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class PartyInfo
{
    public string name;
    public int partyId;
    public int currentMemberCount;
    public int[] currentMemberActorNumber;
    public int currentLeaderActorNumber;
    public int maxPartyMemberCount;
    public int goal;
    public int minPartyLevel;
    public int maxPartyLevel;

    public PartyInfo() { name = " "; partyId = -1; currentMemberCount = -1; currentMemberActorNumber = new int[] { }; maxPartyMemberCount = 4; goal = 1; minPartyLevel = 1; maxPartyLevel = 99; }

    public PartyInfo(string name, int partyId, int currentMember = 1, int[] currentMemberActorNumber = null, int maxPartyMemberCount = 4, int goal = 1, int minLevel = 1, int maxLevel = 99)
    {
        this.name = name;
        this.partyId = partyId;
        this.currentMemberCount = currentMember;
        this.currentMemberActorNumber = currentMemberActorNumber;
        this.maxPartyMemberCount = maxPartyMemberCount;
        this.goal = goal;
        minPartyLevel = minLevel;
        maxPartyLevel = maxLevel;
    }
}

public class PhotonManager : PhotonSingletonManager<PhotonManager>
{
    private const string PARTY_LIST_KEY = "PartyList";

    public string roomInfoText;
    public string playerInfoText;
    public List<PartyInfo> partyRoomInfoList = new List<PartyInfo>();

    private void Start()
    {

    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        //print("AutomaticallySyncScene = true");

        string roomName = "LobbyRoom";

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new HashTable { { "RoomType", "Lobby" }, { "Difficulty", 0 }, { "PartyList", JsonConvert.SerializeObject(new List<PartyInfo>()) } },
            CustomRoomPropertiesForLobby = new string[] { "RoomType", "Difficulty", "PartyList" }
        };

        //string roomName = "DungeonRoom";

        //RoomOptions roomOptions = new RoomOptions
        //{
        //    MaxPlayers = 20,
        //    IsVisible = true,
        //    IsOpen = true,
        //    CustomRoomProperties = new HashTable { { "RoomType", "Dungeon" }, { "Difficulty", 1 } },
        //    CustomRoomPropertiesForLobby = new string[] { "RoomType", "Difficulty" }
        //};

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Join Room Failed: {message}");
    }

    public void SetNewPartyInfo(PartyInfo newPartyInfo)
    {
        List<PartyInfo> partyList = GetPartyList();

        partyList.Add(newPartyInfo);

        string jsonPartyList = JsonConvert.SerializeObject(partyList);

        HashTable fixedcustomProperties = new HashTable
        {
            { PARTY_LIST_KEY, jsonPartyList }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(fixedcustomProperties);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public void UpdatePartyInfo(PartyInfo partyInfo)
    {
        bool partyExists = false;

        for (int i = 0; i < partyRoomInfoList.Count; ++i)
        {
            if (partyRoomInfoList[i].partyId == partyInfo.partyId)
            {
                partyRoomInfoList[i] = partyInfo;
                partyExists = true;
                break;
            }
        }

        if (!partyExists)
        {
            partyRoomInfoList.Add(partyInfo);
        }

        string jsonPartyList = JsonConvert.SerializeObject(partyRoomInfoList);

        HashTable fixedcustomProperties = new HashTable
        {
            { PARTY_LIST_KEY, jsonPartyList }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(fixedcustomProperties);
    }

    public void UpdatePartyInfo<T>(string key, T value)
    {
        string json = JsonConvert.SerializeObject(value);

        HashTable fixedcustomProperties = new HashTable
        {
            { key, json }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(fixedcustomProperties);
    }

    public List<PartyInfo> GetPartyList()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(PARTY_LIST_KEY, out object partyListJson))
        {
            try
            {
                return JsonConvert.DeserializeObject<List<PartyInfo>>(partyListJson.ToString());
            }
            catch (JsonException e)
            {
                Debug.LogWarning($"??紐꾧께??紐꾧께??紐꾧께?饔낃엑iled to parse party list JSON: {e.Message}");
            }
        }

        return new List<PartyInfo>();
    }

    public void RemovePartyInfo(int partyId)
    {
        List<PartyInfo> partyList = GetPartyList();

        partyList.RemoveAll(party => party.partyId == partyId);

        string jsonPartyList = JsonConvert.SerializeObject(partyList);

        HashTable fixedcustomProperties = new HashTable
        {
            { PARTY_LIST_KEY, jsonPartyList }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(fixedcustomProperties);
    }

    public void CreateDungeonRoom(int difficulty)
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
            IsOpen = true,
            CustomRoomProperties = new HashTable { { "RoomType", "Dungeon" }, { "Difficulty", difficulty } },
            CustomRoomPropertiesForLobby = new string[] { "RoomType", "Difficulty" }
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void CreateDungeonRoom(string name, int maxMember, int difficulty, int minLevel, int maxLevel)
    {
        if (!PartyManager.Instance.IsPartyLeader(PhotonNetwork.LocalPlayer))
        {
            Debug.LogError("Only the party leader can create a dungeon room.");
            return;
        }

        string roomName = name;

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)maxMember,
            IsVisible = false,
            IsOpen = true,
            CustomRoomProperties = new HashTable { { "RoomType", "Dungeon" }, { "Difficulty", difficulty }, { "MinLevel", minLevel }, { "MaxLevel", maxLevel } },
            CustomRoomPropertiesForLobby = new string[] { "RoomType", "Difficulty", "MinLevel", "MaxLevel" }
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void CreateDungeonRoom(PartyInfo partyInfo)
    {
        if (!PartyManager.Instance.IsPartyLeader(PhotonNetwork.LocalPlayer))
        {
            LobbyUI.Instance.PopupOpen<PopupPanel>().SetPopup("Error", "You Are Not Party's Leader.");
            return;
        }

        string roomName = name;

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)partyInfo.maxPartyMemberCount,
            IsVisible = false,
            IsOpen = true,
            CustomRoomProperties = new HashTable { { "RoomType", "Dungeon" }, { "Difficulty", partyInfo.goal } },
            CustomRoomPropertiesForLobby = new string[] { "RoomType", "Difficulty" }
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
        //foreach (var player in PartyManager.Instance.GetPartyMembers())
        //{
        //    photonView.RPC("ForceJoinDungeon", player);
        //}
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



    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{

    //}

    public override void OnCreatedRoom()
    {
        //if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("RoomType")
        //    && PhotonNetwork.CurrentRoom.CustomProperties["RoomType"].ToString() == "Dungeon"
        //    && PartyManager.Instance.IsPartyLeader(PhotonNetwork.LocalPlayer))
        //{

        //    PhotonNetwork.JoinRoom(PhotonNetwork.CurrentRoom.Name);
        //}
    }

    [PunRPC]
    private void ForceJoinDungeon(string roomName, PhotonMessageInfo info)
    {
        if (!PartyManager.Instance.GetPartyMembers().Contains(info.Sender)) return;
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        print("OnJoinedRoom");

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RoomType", out object dungeonRoomType) && dungeonRoomType.ToString() == "Dungeon")
        {
            Debug.Log($"Joined dungeon room: {PhotonNetwork.CurrentRoom.Name}");
            PhotonNetwork.LoadLevel("DungeonScene");
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RoomType", out object lobbyroomType) && lobbyroomType.ToString() == "Lobby")
        {

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("LobbyScene");
                print("PhotonNetwork.LoadLevel(\"LobbyScene\");\n");

            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from Photon: {cause}");
        //if (PhotonNetwork.LocalPlayer != null && UnitManager.Instance.LocalPlayer != null)
        //{
        //    UnitManager.Instance.UnregisterPlayer(UnitManager.Instance.LocalPlayer.GetComponent<PhotonView>().ViewID);
        //}
        var loadComplete = SceneManager.LoadSceneAsync("TitleScene");

    }

    public override void OnPlayerLeftRoom(PhotonRealtimePlayer otherPlayer)
    {
        Debug.Log($"Player left room: {otherPlayer.NickName}");

    }

    public override void OnRoomPropertiesUpdate(HashTable propertiesThatChanged)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("RoomType", out object dungeonRoomType) && dungeonRoomType.ToString() == "Lobby")
        {
            if (propertiesThatChanged.ContainsKey(PARTY_LIST_KEY))
            {
                string partyList = propertiesThatChanged[PARTY_LIST_KEY].ToString();
                print(partyList);
                partyRoomInfoList = JsonConvert.DeserializeObject<List<PartyInfo>>(partyList);

                if (PartyManager.Instance.currentPartyInfo != null)
                {
                    var updatedPartyInfo = partyRoomInfoList.FirstOrDefault(p => p.partyId == PartyManager.Instance.currentPartyInfo.partyId);
                    if (updatedPartyInfo != null)
                    {
                        PartyManager.Instance.UpdateInfo(updatedPartyInfo);
                    }
                }

                LobbyUI.Instance.board.UpdatePartyList();
            }
        }
    }


}
