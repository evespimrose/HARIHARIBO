using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDebuff : MonoBehaviour
{
    NormalMonster monster;
    private Coroutine bleeding;
    private Coroutine poison;
    private Coroutine slow;

    private void Awake()
    {
        monster = GetComponent<NormalMonster>();
    }

    private void Update()
    {
        if(monster.isBleeding == false)
        {
            bleeding = null;
        }
        if (monster.isPoison == false)
        {
            poison = null;
        }
        if (monster.isSlow == false)
        {
            slow = null;
        }
    }

    public void Bleeding()
    {
        if (bleeding != null) return;
        bleeding = StartCoroutine(BleedingOff());
        while (monster.isBleeding == true)
        {
            monster.curHp -= 1f;
        }
    }

    public void Poison()
    {
        if (poison != null) return;
        poison = StartCoroutine(PoisonOff());
        while (monster.isPoison == true)
        {
            monster.curHp -= 1f;
        }
    }

    public void Slow()
    {
        if (slow != null) return;
        slow = StartCoroutine(SlowOff());        
        monster.moveSpeed -= 1f;
    }

    private IEnumerator BleedingOff()
    {
        yield return new WaitForSeconds(1f);
        monster.isBleeding = false;
    }

    private IEnumerator PoisonOff()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.GetComponent<NormalMonster>().isPoison = false;
    }


    private IEnumerator SlowOff()
    {
        yield return new WaitForSeconds(1f);
        monster.moveSpeed += 1f;
        monster.isSlow = false;
    }
}
