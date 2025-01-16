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
        Debug.Log("Move진입");
        monster.animator.SetBool("Move", true);
    }

    public override void Update(NormalMonster monster)
    {
        if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
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
