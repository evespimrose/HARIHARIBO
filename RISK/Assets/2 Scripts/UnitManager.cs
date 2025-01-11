using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : SingletonManager<UnitManager>
{

    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> monsters = new List<GameObject>();

    public void RegisterPlayer(GameObject player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    public void UnregisterPlayer(GameObject player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }

    // 추가적인 관리 기능을 여기에 구현
}
