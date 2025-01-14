using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour, ITakedamage
{

    // Start is called before the first frame update
    void Start()
    {
        UnitManager.Instance.players.Add(0, this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Takedamage(float damage)
    {
        print("¾Æ¾ß");
    }
}
