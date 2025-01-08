using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterMove : BaseState<NormalMonster>
{
    public NormalMonsterMove(StateHandler<NormalMonster> handler) : base(handler) { }

    public override void Enter(NormalMonster entity)
    {
        Debug.Log("Move¡¯¿‘");
        entity.animator.SetBool("Move", true);
    }

    public override void Update(NormalMonster entity)
    {
        if (Vector3.Distance(entity.target.position, entity.transform.position) < entity.atkRange)
        {
            if (entity.monsterType == NormalMonster.MonsterType.Melee)
            {
                //entity.ChangeState(new MonsterMeleeAtk());
            }
            else if (entity.monsterType == NormalMonster.MonsterType.Range)
            {
                //entity.ChangeState(new NormalMonsterRangeAtk());
            }
        }
        entity.Move();
    }

    public override void Exit(NormalMonster entity)
    {
        entity.animator.SetBool("Move", false);
    }
}
