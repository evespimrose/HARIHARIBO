using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    public List<GameObject> players;
    public List<GameObject> monsters;

    private void Awake()
    {
        Instance = this;
    }
}
