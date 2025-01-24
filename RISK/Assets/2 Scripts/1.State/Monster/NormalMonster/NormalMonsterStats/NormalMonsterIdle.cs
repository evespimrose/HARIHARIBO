using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NormalMonster;

public class NormalMonsterIdle : BaseState<NormalMonster>
{
    public NormalMonsterIdle(StateHandler<NormalMonster> handler) : base(handler) { }

    public override void Enter(NormalMonster monster)
    {
        monster.animator?.SetTrigger("Idle");
    }

    public override void Update(NormalMonster monster)
    {
        if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            monster.StartCoroutine(monster.AtkCoolTime());
            switch (monster.monsterType)
            {
                case Monster.MonsterType.Melee:
                    monster.nMHandler.ChangeState(typeof(NormalMonsterMeleeAtk));
                    break;
                case Monster.MonsterType.Range:
                    monster.nMHandler.ChangeState(typeof(NormalMonsterRangeAtk));
                    break;
            }
        }
        else if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == true)
        {
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
