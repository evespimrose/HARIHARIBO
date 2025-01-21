using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonRealtimePlayer = Photon.Realtime.Player;
using HashTable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Newtonsoft.Json;
using Photon.Pun;

public class PartyManager : PhotonSingletonManager<PartyManager>
{
    [SerializeField] private List<PhotonRealtimePlayer> partyMembers = new List<PhotonRealtimePlayer>();
    [SerializeField] private PhotonRealtimePlayer partyLeader;

    public PartyInfo currentPartyInfo;

    public bool isInParty = false;

    public void CreateParty(PartyInfo newPartyInfo)
    {
        currentPartyInfo = newPartyInfo;
        PhotonManager.Instance.SetNewPartyInfo(currentPartyInfo);
        JoinParty(PhotonNetwork.LocalPlayer);
    }

    public void JoinParty(PhotonRealtimePlayer player)
    {
        if (!partyMembers.Contains(player))
        {
            if (partyMembers.Count == 0)
                SetPartyLeader(player);
            partyMembers.Add(player);
            isInParty = true;
            currentPartyInfo.currentMemberCount = partyMembers.Count;
            UpdatePartyInfo();
            Debug.Log($"{player.NickName} joined the party!");
        }
    }

    public void JoinParty(PhotonRealtimePlayer player, PartyInfo info)
    {
        UpdateInfo(info);

        if (!partyMembers.Contains(player))
        {
            if (partyMembers.Count == 0)
                SetPartyLeader(player);
            partyMembers.Add(player);
            isInParty = true;

            currentPartyInfo.currentMemberCount = partyMembers.Count;
            UpdatePartyInfo();
            Debug.Log($"{player.NickName} joined the party!");
        }
    }

    public void LeaveParty(PhotonRealtimePlayer player)
    {
        if (currentPartyInfo == null) return;

        if (PhotonNetwork.LocalPlayer == player)
        {
            if (IsPartyLeader(player) && partyMembers.Count > 1)
            {
                var nextLeader = partyMembers.Find(m => m != player);
                currentPartyInfo.currentLeaderActorNumber = nextLeader.ActorNumber;
            }

            partyLeader = null;
            partyMembers = null;

            if (currentPartyInfo.currentMemberCount <= 1)
            {
                PhotonManager.Instance.RemovePartyInfo(currentPartyInfo.partyId);
            }
            else
            {
                var memberList = new List<int>(currentPartyInfo.currentMemberActorNumber);
                memberList.Remove(player.ActorNumber);
                currentPartyInfo.currentMemberActorNumber = memberList.ToArray();
                currentPartyInfo.currentMemberCount--;

                PhotonManager.Instance.UpdatePartyInfo(currentPartyInfo);
            }

            currentPartyInfo = null;
            isInParty = false;

        }
        else if (partyMembers.Contains(player))
        {
            partyMembers.Remove(player);

            var memberList = new List<int>(currentPartyInfo.currentMemberActorNumber);
            memberList.Remove(player.ActorNumber);
            currentPartyInfo.currentMemberActorNumber = memberList.ToArray();
            currentPartyInfo.currentMemberCount = partyMembers.Count;

            if (IsPartyLeader(player) && partyMembers.Count > 0)
            {
                var nextLeader = partyMembers[0];
                SetPartyLeader(nextLeader);
                currentPartyInfo.currentLeaderActorNumber = nextLeader.ActorNumber;
            }

            PhotonManager.Instance.UpdatePartyInfo(currentPartyInfo);
            isInParty = false;

            Debug.Log($"{player.NickName} left the party!");
        }
    }

    public void SetPartyLeader(PhotonRealtimePlayer leader)
    {
        partyLeader = leader;
        Debug.Log($"{leader.NickName} is now the party leader!");
    }

    private void UpdatePartyInfo()
    {
        Debug.Log($"UpdatePartyInfo! : {currentPartyInfo}");
        PhotonManager.Instance.UpdatePartyInfo(currentPartyInfo);
    }

    public bool IsPartyLeader(PhotonRealtimePlayer player)
    {
        return player == partyLeader;
    }

    public List<PhotonRealtimePlayer> GetPartyMembers()
    {
        return partyMembers;
    }

    public PhotonRealtimePlayer GetPartyLeader()
    {
        return partyLeader;
    }

    public void UpdateInfo(PartyInfo info)
    {
        currentPartyInfo = info;
        partyMembers.Clear();

        foreach (int actorNumber in info.currentMemberActorNumber)
        {
            PhotonRealtimePlayer player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
            if (player != null)
            {
                partyMembers.Add(player);

                if (actorNumber == info.currentLeaderActorNumber)
                {
                    partyLeader = player;
                }
            }
        }
    }

    public PlayerStats GetPartyMemberStats(PhotonRealtimePlayer player)
    {
        if (UnitManager.Instance.HasPlayer(player.ActorNumber))
        {
            GameObject playerObj = UnitManager.Instance.GetPlayer(player.ActorNumber);
            if (playerObj.TryGetComponent(out Player playerComponent))
            {
                return playerComponent.Stats;
            }
        }
        return null;
    }

    public string GetPartyMemberClass(PhotonRealtimePlayer player)
    {
        if (UnitManager.Instance.HasPlayer(player.ActorNumber))
        {
            GameObject playerObj = UnitManager.Instance.GetPlayer(player.ActorNumber);
            if (playerObj.TryGetComponent(out Warrior _)) return "Warrior";
            if (playerObj.TryGetComponent(out Destroyer _)) return "Destroyer";
            if (playerObj.TryGetComponent(out Healer _)) return "Healer";
            if (playerObj.TryGetComponent(out Mage _)) return "Mage";
        }
        return "Unknown";
    }

}
