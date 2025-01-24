using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterAirborne : BaseState<NormalMonster>
{
    public NormalMonsterAirborne(StateHandler<NormalMonster> handler) : base(handler) { }

    public override void Enter(NormalMonster monster)
    {
        monster.animator.SetTrigger("Airborne");
        monster.StartAirborne();
    }

    public override void Update(NormalMonster monster)
    {
        if (monster.isStun == true)
        {
            monster.isStun = false;
        }
        if (!monster.isAirborne)
        {
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
        }
    }

    public override void Exit(NormalMonster monster)
    {

    }
}

