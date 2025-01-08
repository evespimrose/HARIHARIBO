using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NormalMonster;

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
            switch (monster.monsterType)
            {
                case MonsterType.Melee:
                    monster.nMHandler.ChangeState(typeof(NormalMonsterMeleeAtk));
                    break;
                case MonsterType.Range:
                    monster.nMHandler.ChangeState(typeof(NormalMonsterRangeAtk));
                    break;
            }
        }
        else
        {
            monster.nMHandler.ChangeState(typeof(NormalMonsterMove));
        }
    }

    public override void Exit(NormalMonster monster)
    {

    }
}
