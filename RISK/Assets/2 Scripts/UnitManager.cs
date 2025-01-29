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
        // PhotonView揶쎛 ?醫???롫즲嚥?base.Awake() ?紐꾪뀱 ?袁⑸퓠 ?類ㅼ뵥
        if (Instance != null && Instance != this)
        {
            // 疫꿸퀣??PhotonView??ViewID????덉쨮??揶쏆빘猿쒏에??袁⑤뼎
            if (Instance.photonView != null && photonView != null)
            {
                photonView.ViewID = Instance.photonView.ViewID;
            }
        }
        base.Awake();
    }

    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public GameObject LocalPlayer { get; private set; }
    public List<GameObject> monsters = new List<GameObject>();

    [SerializeField] public int playersCount;
    public List<string> playerNames = new List<string>();

    [PunRPC]
    private void RequestPlayerSync(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (var playerPair in players)
        {
            if (playerPair.Value != null && playerPair.Value.TryGetComponent(out PhotonView photonView))
            {
                photonView.RPC("SyncPlayer", info.Sender,
                    photonView.ViewID,
                    photonView.Owner.ActorNumber,
                    playerPair.Value.name);
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

    [PunRPC]
    private void SyncPlayer(int viewId, int actorNumber, string playerName)
    {
        print("SyncPlayer");
        PhotonView targetView = PhotonView.Find(viewId);
        if (targetView != null)
        {
            GameObject playerObj = targetView.gameObject;
            playerObj.name = playerName;

            if (!HasPlayer(actorNumber))
            {
                RegisterPlayer(playerObj);
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
                    print($"RegisterPlayer - RegisterLocalPlayer : {actorNumber}");
                    LocalPlayer = player;
                }

                // Player??PhotonView?????퉸 RPC ?紐꾪뀱
                photonView.RPC("NotifyPlayerRegistered", RpcTarget.All, actorNumber);
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
        if (photonView != null && target != null)
        {
            PhotonView targetView = target.GetComponent<PhotonView>();
            if (targetView != null)
            {
                int targetID = targetView.ViewID;

                if (PhotonNetwork.IsMasterClient) // ✅ 파티장만 데미지 계산
                {
                    // 파티장에서만 RPC 호출해서 데미지 전파
                    photonView.RPC("ApplyDamageRPC", RpcTarget.All, targetID, damage);
                }
            }
        }
    }

    private void Update()
    {
        playersCount = players.Count;
        if(playerNames.Count != players.Count)
        {
            foreach (var i in players)
            {
                i.Value.TryGetComponent(out Player player);
                playerNames.Add(player.Stats.nickName);
            }
        }
    }

    [PunRPC]
    protected void ApplyDamageRPC(int targetID, float damage)
    {
        // 타겟을 찾아서 데미지 적용
        PhotonView targetView = PhotonView.Find(targetID);
        if (targetView != null)
        {
            targetView.GetComponent<ITakedamage>().Takedamage(damage);
        }
    }
}
