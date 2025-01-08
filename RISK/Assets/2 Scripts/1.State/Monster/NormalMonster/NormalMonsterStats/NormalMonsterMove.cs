using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static NormalMonster;

public class NormalMonsterMove : BaseState<NormalMonster>
{
    public NormalMonsterMove(StateHandler<NormalMonster> handler) : base(handler) { }

    public override void Enter(NormalMonster monster)
    {
        Debug.Log("Move¡¯¿‘");
        monster.animator.SetBool("Move", true);
    }

    public override void Update(NormalMonster monster)
    {
        if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
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
            monster.Move();
        }
    }

    public override void Exit(NormalMonster monster)
    {
        monster.animator.SetBool("Move", false);
    }
}
