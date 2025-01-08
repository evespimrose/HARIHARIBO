using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMeleeAtk : NormalMonsterBaseState
{
    private NormalMonster normalMonster;

    public float atkDuration = 5f;
    public float atkDelay = 0.4f;
    private float curTime = 0;
    private bool isAtk = false;
    public override void Enter(BaseCharacter entity)
    {
        Debug.Log("MeleeAtk진입");
        normalMonster = entity as NormalMonster;
        normalMonster.animator.SetTrigger("Atk");
        curTime = 0;
        isAtk = false;
    }

    public override void Update(BaseCharacter entity)
    {
        if (atkDuration - curTime < 0.1f)
        {
            //공격종료
            normalMonster.ChangeState(new MonsterIdle());
        }
        if (atkDelay - curTime < 0.1f && isAtk == false)
        {
            Atk(normalMonster);
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(BaseCharacter entity)
    {
        normalMonster.StartCoroutine(normalMonster.AtkCoolTime());
    }

    private void Atk(NormalMonster entity)
    {
        Debug.Log("MeleeAtk공격");
        entity.isAtk = true;
        isAtk = true;
        Collider[] cols = Physics.OverlapSphere(entity.transform.position, entity.atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<ITakedamage>().Takedamage(entity.AtkDamage);
            }
        }
    }
}
