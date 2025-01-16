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

    public void JoinParty(PhotonRealtimePlayer player, PartyInfo info)
    {
        UpdateInfo(info);

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
                // 파티 멤버 목록에서 자신 제거
                var memberList = new List<int>(currentPartyInfo.currentMemberActorNumber);
                memberList.Remove(player.ActorNumber);
                currentPartyInfo.currentMemberActorNumber = memberList.ToArray();
                currentPartyInfo.currentMemberCount--;
                
                // 파티 정보 업데이트
                PhotonManager.Instance.UpdatePartyInfo(currentPartyInfo);
            }
            
            currentPartyInfo = null;
        }
        else if (partyMembers.Contains(player))
        {
            // 다른 멤버가 나가는 경우
            partyMembers.Remove(player);
            
            // 파티 멤버 목록 업데이트
            var memberList = new List<int>(currentPartyInfo.currentMemberActorNumber);
            memberList.Remove(player.ActorNumber);
            currentPartyInfo.currentMemberActorNumber = memberList.ToArray();
            currentPartyInfo.currentMemberCount = partyMembers.Count;
            
            // 나간 멤버가 파티장이었다면 다음 멤버를 파티장으로 설정
            if (IsPartyLeader(player) && partyMembers.Count > 0)
            {
                var nextLeader = partyMembers[0];
                SetPartyLeader(nextLeader);
                currentPartyInfo.currentLeaderActorNumber = nextLeader.ActorNumber;
            }
            
            // 파티 정보 업데이트
            PhotonManager.Instance.UpdatePartyInfo(currentPartyInfo);
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

}
