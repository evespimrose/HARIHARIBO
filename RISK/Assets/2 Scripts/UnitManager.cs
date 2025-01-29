using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
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

    [PunRPC]
    private void RequestPlayerSync(PhotonMessageInfo info)
    {
        print("RequestPlayerSync");
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
                monster.gameObject.GetComponent<ITakedamage>().Takedamage(2000000);
                //CalculateAndSendDamage(monster.gameObject, 2000000);
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

    // 데미지 계산 후 전송하는 메서드
    public void CalculateAndSendDamage(GameObject target, float dmg)
    {
        // 방장에서 데미지 계산 (여기서는 단순히 공격력으로 계산)
        float damage = dmg;

        // 방장만 데미지를 전송
        if (PhotonNetwork.IsMasterClient)
        {
            // PhotonView 컴포넌트를 명시적으로 가져옴
            PhotonView photonView = GetComponent<PhotonView>();

            if (photonView != null)
            {
                // photonView를 통해 RPC 호출
                photonView.RPC("ApplyDamageToClient", RpcTarget.All, target.GetPhotonView().ViewID, damage);
            }
        }
    }

    // RPC로 다른 클라이언트에 데미지 적용
    [PunRPC]
    public void ApplyDamageToClient(int targetPhotonViewID, float damage)
    {
        // PhotonView ID로 대상 객체 찾기
        PhotonView targetView = PhotonView.Find(targetPhotonViewID);

        // 대상 객체가 존재하면, ITakedamage 인터페이스를 통해 데미지를 적용
        if (targetView != null)
        {
            targetView.gameObject.GetComponent<ITakedamage>()?.Takedamage(damage);
        }
    }
}
