using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : SingletonManager<UnitManager>
{
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public GameObject LocalPlayer { get; private set; }
    public List<GameObject> monsters = new List<GameObject>();

    public void RegisterPlayer(GameObject player, int actorNumber)
    {
        if (!players.ContainsKey(actorNumber))
        {
            players.Add(actorNumber, player);

            if (player.GetComponent<PhotonView>().IsMine)
            {
                LocalPlayer = player;
            }
        }
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
