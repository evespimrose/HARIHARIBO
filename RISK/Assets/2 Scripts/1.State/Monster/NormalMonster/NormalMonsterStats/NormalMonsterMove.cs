using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static NormalMonster;

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
        if (Vector3.Distance(entity.target.position, entity.transform.position) < entity.atkRange && entity.isAtk == false)
        {
            switch (entity.monsterType)
            {
                case MonsterType.Melee:
                    entity.nMHandler.ChangeState(typeof(NormalMonsterMeleeAtk));
                    break;
                case MonsterType.Range:
                    entity.nMHandler.ChangeState(typeof(NormalMonsterRangeAtk));
                    break;
            }
        }
        else
        {
            entity.Move();
        }
    }

    public override void Exit(NormalMonster entity)
    {
        entity.animator.SetBool("Move", false);
    }
}
