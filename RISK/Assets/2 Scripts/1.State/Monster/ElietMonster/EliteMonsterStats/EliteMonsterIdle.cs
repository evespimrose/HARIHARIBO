using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NormalMonster;

public class EliteMonsterIdle : BaseState<EliteMonster>
{
    public EliteMonsterIdle(StateHandler<EliteMonster> handler) : base(handler) { }

    public override void Enter(EliteMonster monster)
    {
        Debug.Log("Idle진입");
        monster.animator?.SetTrigger("Idle");
    }

    public override void Update(EliteMonster monster)
    {
        if (monster.isAtk == true) return;
        else if (monster.target == null)
        {
            monster.Targeting();
        }
        else if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            //공격으로 이동
        }
        else
        {
            monster.eMHandler.ChangeState(typeof(EliteMonsterMove));
        }
    }

    public override void Exit(EliteMonster monster)
    {

    }
}
