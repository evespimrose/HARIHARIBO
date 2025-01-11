using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonRealtimePlayer = Photon.Realtime.Player;

public class PartyManager : PhotonSingletonManager<PartyManager>
{
    private List<PhotonRealtimePlayer> partyMembers = new List<PhotonRealtimePlayer>();
    private PhotonRealtimePlayer partyLeader;

    public void JoinParty(PhotonRealtimePlayer player)
    {
        if (!partyMembers.Contains(player))
        {
            partyMembers.Add(player);
            Debug.Log($"{player.NickName} joined the party!");
        }
    }

    public void LeaveParty(PhotonRealtimePlayer player)
    {
        if (partyMembers.Contains(player))
        {
            partyMembers.Remove(player);
            Debug.Log($"{player.NickName} left the party!");
        }
    }

    public void SetPartyLeader(PhotonRealtimePlayer leader)
    {
        partyLeader = leader;
        Debug.Log($"{leader.NickName} is now the party leader!");
    }

    public bool IsPartyLeader(PhotonRealtimePlayer player)
    {
        return player == partyLeader;
    }

    public List<PhotonRealtimePlayer> GetPartyMembers()
    {
        return partyMembers;
    }
}
