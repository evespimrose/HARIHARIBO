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
            currentPartyInfo.currentMemberCount = partyMembers.Count;
            UpdatePartyInfo();
            Debug.Log($"{player.NickName} joined the party!");
        }
    }

    public void LeaveParty(PhotonRealtimePlayer player)
    {
        if (PhotonNetwork.LocalPlayer == player)
        {
            partyLeader = null;
            partyMembers = null;
            currentPartyInfo = null;
        }
        else if (partyMembers.Contains(player))
        {
            // have to Test
            partyMembers.Remove(player);
            currentPartyInfo.currentMemberCount = partyMembers.Count;
            UpdatePartyInfo();
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

}
