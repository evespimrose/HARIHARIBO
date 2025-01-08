using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : NormalMonsterBaseState
{
    private NormalMonster normalMonster;

    public override void Enter(BaseCharacter entity)
    {
        normalMonster = entity as NormalMonster;
        Debug.Log("Move¡¯¿‘");
        entity.animator.SetBool("Move", true);
    }

    public override void Update(BaseCharacter entity)
    {
        if (Vector3.Distance(normalMonster.target.position, normalMonster.transform.position) < normalMonster.atkRange)
        {
            if (normalMonster.monsterType == NormalMonster.MonsterType.Melee)
            {
                normalMonster.ChangeState(new MonsterMeleeAtk());
            }
            else if (normalMonster.monsterType == NormalMonster.MonsterType.Range)
            {
                normalMonster.ChangeState(new MonsterRangeAtk());
            }
        }
        normalMonster.transform.LookAt(normalMonster.target);
        Vector3 dir = (normalMonster.target.position - normalMonster.transform.position).normalized;
        Vector3 moveDir = normalMonster.transform.position + dir * normalMonster.MoveSpeed * Time.fixedDeltaTime;
        normalMonster.rb.MovePosition(moveDir);
    }

    public override void Exit(BaseCharacter entity)
    {
        entity.animator.SetBool("Move", false);
    }
}
