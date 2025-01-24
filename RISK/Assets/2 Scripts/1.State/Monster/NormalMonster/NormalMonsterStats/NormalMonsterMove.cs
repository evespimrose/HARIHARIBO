using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterMove : BaseState<NormalMonster>
{
    public NormalMonsterMove(StateHandler<NormalMonster> handler) : base(handler) { }

    public override void Enter(NormalMonster monster)
    {
        if (monster.monsterType != Monster.MonsterType.Range)
        {
            monster.animator.SetBool("Move", true);
        }
        else
        {
            
        }
    }

    public override void Update(NormalMonster monster)
    {
        if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
        }
        else
        {
            if (Vector3.Distance(monster.target.position, monster.transform.position) > 1f)
            {
                monster.Move();
            }
        }
    }

    public override void Exit(NormalMonster monster)
    {
        if (monster.monsterType != Monster.MonsterType.Range)
        {
            monster.animator.SetBool("Move", false);
        }
        else
        {

        }
    }
}
