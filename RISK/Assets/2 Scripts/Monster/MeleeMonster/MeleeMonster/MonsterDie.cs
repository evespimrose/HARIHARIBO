using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDie : NormalMonsterBaseState
{
    private NormalMonster normalMonster;

    public override void Enter(BaseCharacter entity)
    {
        normalMonster = entity as NormalMonster;
        entity.animator.SetBool("Die", true);
    }

    public override void Update(BaseCharacter entity)
    {
        
    }

    public override void Exit(BaseCharacter entity)
    {
        
    }
}
