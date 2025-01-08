using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterDie : BaseState<NormalMonster>
{
    public NormalMonsterDie(StateHandler<NormalMonster> handler) : base(handler) { }

    public override void Enter(NormalMonster monster)
    {
        //monster.animator.SetBool("Die", true);
    }

    public override void Update(NormalMonster monster)
    {
        
    }

    public override void Exit(NormalMonster monster)
    {
        
    }
}
