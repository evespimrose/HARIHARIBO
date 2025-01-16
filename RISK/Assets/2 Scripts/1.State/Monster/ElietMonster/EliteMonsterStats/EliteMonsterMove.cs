using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterMove : BaseState<EliteMonster>
{
    public EliteMonsterMove(StateHandler<EliteMonster> handler) : base(handler) { }
    
    public override void Enter(EliteMonster monster)
    {
        Debug.Log("Move진입");
        monster.animator.SetBool("Move", true);
    }

    public override void Update(EliteMonster monster)
    {
        if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            monster.eMHandler.ChangeState(typeof(EliteMonsterIdle));
        }
        else
        {
            monster.Move();
        }
    }

    public override void Exit(EliteMonster monster)
    {
        monster.animator.SetBool("Move", false);
    }
}
