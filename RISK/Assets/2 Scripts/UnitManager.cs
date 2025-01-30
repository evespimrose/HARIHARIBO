using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : PhotonSingletonManager<UnitManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public GameObject LocalPlayer { get; private set; }
    public List<GameObject> monsters = new List<GameObject>();

    [SerializeField] public int playersCount;
    public List<GameObject> playerobjects = new List<GameObject>();
    

    public void RequestPlayerSync()
    {
        var playersCopy = new Dictionary<int, GameObject>(players);
        
        foreach (var playerPair in playersCopy)
        {
            if (playerPair.Value != null && playerPair.Value.TryGetComponent(out PhotonView pView))
            {
                print("RequestPlayerSync()?");
                PhotonRequest.Instance.RequestPlayerSync(pView.ViewID, pView.Owner.ActorNumber, playerPair.Value.name);
            }
        }
    }

    public void RequestPlayerSyncToRoomMembers()
    {
        var playersCopy = new Dictionary<int, GameObject>(players);

        foreach (var playerPair in playersCopy)
        {
            if (playerPair.Value != null && playerPair.Value.TryGetComponent(out PhotonView pView))
            {
                PhotonRequest.Instance.RequestPlayerSync(pView.ViewID, pView.Owner.ActorNumber, playerPair.Value.name);
            }
        }
    }

    public void DoomToMonsters()
    {
        foreach (var monster in monsters)
        {
            if (monster != null && monster.TryGetComponent(out Monster monsterComponent))
            {
                //monster.gameObject.GetComponent<ITakedamage>().Takedamage(2000000);
                Atk(monster.gameObject, 2000000);
            }
        }
    }

    public void RegisterPlayer(GameObject player)
    {
        print("RegisterPlayer");
        if (player.TryGetComponent(out PhotonView photonView))
        {
            int actorNumber = photonView.Owner.ActorNumber;
            print($"RegisterPlayer : {actorNumber}, {photonView.IsMine}");

            if (!players.ContainsKey(actorNumber))
            {
                players.Add(actorNumber, player);

                if (photonView.IsMine)
                {
                    LocalPlayer = player;
                }
            }
        }
        else
            print("GameObject doesn't have PhotonView!!");
    }

    public void UnregisterPlayer(int actorNumber)
    {
        if (players.ContainsKey(actorNumber))
        {
            if (players[actorNumber] == LocalPlayer)
            {
                LocalPlayer = null;
            }
            players.Remove(actorNumber);
        }
    }

    public bool HasPlayer(int actorNumber)
    {
        return players.ContainsKey(actorNumber);
    }

    public GameObject GetPlayer(int actorNumber)
    {
        return players.ContainsKey(actorNumber) ? players[actorNumber] : null;
    }

    // ✅ 공격 처리 함수 (화살 등은 PhotonView 필요 없음)
    public void Atk(GameObject target, float damage)
    {
        // 공격 시 자신이 가진 PhotonView를 사용해서 RPC 호출
        if (target != null)
        {
            PhotonView targetView = target.GetComponent<PhotonView>();
            if (targetView != null)
            {
                int targetID = targetView.ViewID;

                if (PhotonNetwork.IsMasterClient) // ✅ 파티장만 데미지 계산
                {
                    // 파티장에서만 RPC 호출해서 데미지 전파
                    PhotonRequest.Instance.ApplyDamage(targetID, damage);
                }
            }
        }
    }

    private void Update()
    {
        playersCount = players.Count;
        if(playerobjects.Count != players.Count)
        {
            playerobjects.Clear();
            foreach (var i in players)
            {
                i.Value.TryGetComponent(out Player player);
                playerobjects.Add(player.gameObject);
            }
        }
    }
}
