using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class BossMonsterIdle : BaseState<BossMonster>
{
    public BossMonsterIdle(StateHandler<BossMonster> handler) : base(handler) { }

    public override void Enter(BossMonster monster)
    {
        Debug.Log("Idle����");
        monster.animator?.SetTrigger("Idle");
    }

    public override void Update(BossMonster monster)
    {
        if (monster.isAtk == true) return;
        if (monster.target == null)
        {
            monster.Targeting();
        }
        else if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            //�������� �̵�
            monster.bMHandler.ChangeState(typeof(BossMonsterSkillD));
        }
        else
        {
            monster.bMHandler.ChangeState(typeof(BossMonsterMove));
        }
    }

    public override void Exit(BossMonster monster)
    {

    }
}
