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
        // PhotonView媛 ?좎??섎룄濡?base.Awake() ?몄텧 ?꾩뿉 ?뺤씤
        if (Instance != null && Instance != this)
        {
            // 湲곗〈 PhotonView??ViewID瑜??덈줈??媛앹껜濡??꾨떖
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

                // Player??PhotonView瑜??듯빐 RPC ?몄텧
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
}
