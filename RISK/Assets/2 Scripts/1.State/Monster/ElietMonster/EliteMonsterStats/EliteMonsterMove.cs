using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NormalMonster;

public class EliteMonsterMove : BaseState<EliteMonster>
{
    public EliteMonsterMove(StateHandler<EliteMonster> handler) : base(handler) { }
    
    public override void Enter(EliteMonster monster)
    {
        Debug.Log("Move����");
        monster.animator.SetBool("Move", true);
    }

    public override void Update(EliteMonster monster)
    {
        if (Vector3.Distance(monster.target.position, monster.transform.position) < monster.atkRange && monster.isAtk == false)
        {
            //�������� �̵�
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
