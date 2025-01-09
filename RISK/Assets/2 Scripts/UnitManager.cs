using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> monsters = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
