using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class NormalMonsterAirborne : BaseState<NormalMonster>
{
    public NormalMonsterAirborne(StateHandler<NormalMonster> handler) : base(handler) { }

    public override void Enter(NormalMonster monster)
    {
        monster.animator?.SetTrigger("Airborne");
        Debug.Log("에어본 시작");
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
        Debug.Log("에어본 종료");
    }
}

