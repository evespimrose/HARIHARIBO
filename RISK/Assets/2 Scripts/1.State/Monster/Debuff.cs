using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuff : MonoBehaviour
{
    [Header("����")]
    [Tooltip("���� ������ ����")]
    public float bleedingDuration = 1f;
    [Tooltip("���� ���ӽð�")]
    public float bleedingTime = 5f;
    [Tooltip("���� ������")]
    public float bleedingDamage = 1f;
    [Header("��")]
    [Tooltip("�� ������ ����")]
    public float poisonDuration = 1f;
    [Tooltip("�� ���ӽð�")]
    public float poisonTime = 5f;
    [Tooltip("�� ������")]
    public float poisonDamage = 1f;
    [Header("���ο�")]
    [Tooltip("���ο� ���ӽð�")]
    public float slowTime = 5f;
    [Tooltip("���ο� ���� ��ġ")]
    public float slowPower = 1f;

    private bool isDie = false;

    public Coroutine bleeding;
    public Coroutine poison;
    public Coroutine slow;

    public void DebuffAllOff()
    {
        StopAllCoroutines();
        isDie = true;
    }

    public void DebuffCheck(NormalMonster monster)
    {
        if (isDie == true) { return; }
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

    public void Bleeding(NormalMonster monster)
    {
        if (bleeding != null) return;
        bleeding = StartCoroutine(BleedingStart(monster));
    }

    public void Poison(NormalMonster monster)
    {
        if (poison != null) return;
        poison = StartCoroutine(PoisonStart(monster));
    }

    public void Slow(NormalMonster monster)
    {
        if (slow != null) return;
        slow = StartCoroutine(SlowStart(monster));        
        monster.moveSpeed -= slowPower;
    }

    private IEnumerator BleedingStart(NormalMonster monster)
    {
        float curBleedingTime = 0f;
        print("���� �����̻� ����");
        while (curBleedingTime < bleedingTime)
        {
            monster.curHp -= bleedingDamage;
            print($"���� �����̻� Hit {bleedingDamage}");
            yield return new WaitForSeconds(bleedingDuration);
            curBleedingTime += bleedingDuration;
        }
        print("���� �����̻� ����");
        monster.isBleeding = false;
    }

    private IEnumerator PoisonStart(NormalMonster monster)
    {
        float curPoisonTime = 0f;
        print("�� �����̻� ����");
        while (curPoisonTime < poisonTime)
        {
            monster.curHp -= poisonDamage;
            print($"�� �����̻� Hit {poisonDamage}");
            yield return new WaitForSeconds(poisonDuration);
            curPoisonTime += poisonDuration;
        }
        print("�� �����̻� ����");
        monster.isBleeding = false;
    }


    private IEnumerator SlowStart(NormalMonster monster)
    {
        print("���ο� �����̻� ����");
        yield return new WaitForSeconds(slowTime);
        print("���ο� �����̻� ����");
        monster.moveSpeed += 1f;
        monster.isSlow = false;
    }
}
