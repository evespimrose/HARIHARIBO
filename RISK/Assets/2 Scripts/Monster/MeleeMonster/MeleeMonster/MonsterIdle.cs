using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdle : BaseState<BaseCharacter>
{
    private NormalMonster normalMonster;

    public override void Enter(BaseCharacter entity)
    {
        Debug.Log("Idle¡¯¿‘");
        normalMonster = entity.GetComponent<NormalMonster>();
        entity.animator?.SetTrigger("Idle");
    }

    public override void Update(BaseCharacter entity)
    {
        if (normalMonster.isAtk == true) return;
        else if (normalMonster.target == null)
        {
            normalMonster.Targeting();
        }
        else if (Vector3.Distance(normalMonster.target.position, normalMonster.transform.position) < normalMonster.atkRange && normalMonster.isAtk == false)
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
        else
        {
            normalMonster.ChangeState(new MonsterMove());
        }
    }

    public override void Exit(BaseCharacter entity) 
    {
        
    }
}
