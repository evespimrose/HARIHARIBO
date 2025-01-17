using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff : MonoBehaviour
{
    [Header("출혈")]
    [Tooltip("출혈 데미지 간격")]
    public float bleedingDuration = 1f;
    [Tooltip("출혈 지속시간")]
    public float bleedingTime = 5f;
    [Tooltip("출혈 데미지")]
    public float bleedingDamage = 1f;
    [Header("독")]
    [Tooltip("독 데미지 간격")]
    public float poisonDuration = 1f;
    [Tooltip("독 지속시간")]
    public float poisonTime = 5f;
    [Tooltip("독 데미지")]
    public float poisonDamage = 1f;
    [Header("슬로우")]
    [Tooltip("슬로우 지속시간")]
    public float slowTime = 5f;
    [Tooltip("슬로우 감소 수치")]
    public float slowPower = 1f;

    private bool isDie = false;

    public Coroutine bleeding;
    public Coroutine poison;
    public Coroutine slow;

    public bool ExcludingBoss(Monster monster)
    {
        return monster.monsterType == Monster.MonsterType.Boss;
    }

    public void DebuffAllOff()
    {
        StopAllCoroutines();
        isDie = true;
    }

    public void DebuffCheck(Monster monster)
    {
        if (ExcludingBoss(monster)) return;
        if (monster.isBleeding == false)
        {
            bleeding = null;
        }
        else
        {
            Bleeding(monster);
        }
        if (monster.isPoison == false)
        {
            poison = null;
        }
        else
        {
            Poison(monster);
        }
        if (monster.isSlow == false)
        {
            slow = null;
        }
        else
        {
            Slow(monster);
        }
    }

    public void Bleeding(Monster monster)
    {
        if (ExcludingBoss(monster)) return;
        if (bleeding != null) return;
        bleeding = StartCoroutine(BleedingStart(monster));
    }

    public void Poison(Monster monster)
    {
        if (ExcludingBoss(monster)) return;
        if (poison != null) return;
        poison = StartCoroutine(PoisonStart(monster));
    }

    public void Slow(Monster monster)
    {
        if (ExcludingBoss(monster)) return;
        if (slow != null) return;
        slow = StartCoroutine(SlowStart(monster));        
        monster.moveSpeed -= slowPower;
    }

    private IEnumerator BleedingStart(Monster monster)
    {
        float curBleedingTime = 0f;
        print("출혈 상태이상 시작");
        while (curBleedingTime < bleedingTime)
        {
            monster.curHp -= bleedingDamage;
            print($"출혈 상태이상 Hit {bleedingDamage}");
            yield return new WaitForSeconds(bleedingDuration);
            curBleedingTime += bleedingDuration;
        }
        print("출혈 상태이상 종료");
        monster.isBleeding = false;
    }

    private IEnumerator PoisonStart(Monster monster)
    {
        float curPoisonTime = 0f;
        print("독 상태이상 시작");
        while (curPoisonTime < poisonTime)
        {
            monster.curHp -= poisonDamage;
            print($"독 상태이상 Hit {poisonDamage}");
            yield return new WaitForSeconds(poisonDuration);
            curPoisonTime += poisonDuration;
        }
        print("독 상태이상 종료");
        monster.isBleeding = false;
    }


    private IEnumerator SlowStart(Monster monster)
    {
        print("슬로우 상태이상 시작");
        yield return new WaitForSeconds(slowTime);
        print("슬로우 상태이상 종료");
        monster.moveSpeed += 1f;
        monster.isSlow = false;
    }
}
