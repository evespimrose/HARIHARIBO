using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : SingletonManager<UnitManager>
{
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public GameObject LocalPlayer { get; private set; }
    public List<GameObject> monsters = new List<GameObject>();

    public void RegisterPlayer(GameObject player)
    {
        if (player.TryGetComponent(out PhotonView photonView))
        {
            int actorNumber = photonView.Owner.ActorNumber;
            print($"RegisterPlayer : {actorNumber}, {player.GetComponent<PhotonView>().IsMine}");
            if (!players.ContainsKey(actorNumber))
            {
                players.Add(actorNumber, player);

                if (player.GetComponent<PhotonView>().IsMine)
                {
                    print($"RegisterPlayer - RegisterLocalPlayer : {actorNumber}");
                    LocalPlayer = player;
                }
            }
        }
        else
            print("GameObject don't has PhotonView!!");
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
