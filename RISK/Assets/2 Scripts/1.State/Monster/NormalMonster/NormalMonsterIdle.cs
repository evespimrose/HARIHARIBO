using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterIdle : BaseState<NormalMonster>
{
    public NormalMonsterIdle(StateHandler<NormalMonster> handler) : base(handler) { }

    public override void Enter(NormalMonster monster)
    {
        Debug.Log("Idle¡¯¿‘");
        monster.animator?.SetTrigger("Idle");
    }

    public override void Update(NormalMonster monster)
    {
        if (monster.isAtk == true) return;
        else if (monster.target == null)
        {
            monster.Targeting();
        }
        else if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            if (monster.monsterType == NormalMonster.MonsterType.Melee)
            {
                //monster.ChangeState(new MonsterMeleeAtk());
            }
            else if (monster.monsterType == NormalMonster.MonsterType.Range)
            {
                //monster.ChangeState(new NormalMonsterRangeAtk());
            }
        }
        else
        {
            //monster.ChangeState(new MonsterMove());
        }
    }

    public override void Exit(NormalMonster monster)
    {

    }
}
