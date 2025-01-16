using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour, ITakedamage
{
    public float hp = 1000f;
    // Start is called before the first frame update
    void Start()
    {
        UnitManager.Instance.players.Add(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Takedamage(float damage)
    {
        hp -= damage;   
        print("¾Æ¾ß");
    }
}
