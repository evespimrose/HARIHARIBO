using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterMove : BaseState<BossMonster>
{
    public BossMonsterMove(StateHandler<BossMonster> handler) : base(handler) { }
    
    public override void Enter(BossMonster monster)
    {
        Debug.Log("Move진입");
        monster.animator.SetBool("Move", true);
    }

    public override void Update(BossMonster monster)
    {
        if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            //공격으로 이동
            monster.bMHandler.ChangeState(typeof(BossMonsterAtk));
        }
        else
        {
            monster.Move();
        }
    }

    public override void Exit(BossMonster monster)
    {
        monster.animator.SetBool("Move", false);
    }
}
