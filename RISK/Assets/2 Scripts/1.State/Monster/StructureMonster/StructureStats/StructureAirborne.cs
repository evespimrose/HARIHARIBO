using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureAirborne : BaseState<StructureMonster>
{
    public StructureAirborne(StateHandler<StructureMonster> handler) : base(handler) { }

    public override void Enter(StructureMonster monster)
    {
        Debug.Log("에어본 시작");
        monster.StartAirborne();
    }

    public override void Update(StructureMonster monster)
    {
        if (monster.isStun == true)
        {
            monster.isStun = false;
        }
        if (monster.isAirborne == false)
        {
            monster.sMHandler.ChangeState(typeof(StructureIdle));
        }
    }

    public override void Exit(StructureMonster monster)
    {
        Debug.Log("에어본 종료");
    }
}
