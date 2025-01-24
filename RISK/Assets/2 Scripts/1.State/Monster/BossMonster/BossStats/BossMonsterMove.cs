using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        if (monster.isChase)
        {
            if (Vector3.Distance(monster.target.position, monster.transform.position) > 2f)
            {
                monster.Move();
            }
        }
        else
        {
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("Move퇴장");
        monster.animator.SetBool("Move", false);
    }
}
