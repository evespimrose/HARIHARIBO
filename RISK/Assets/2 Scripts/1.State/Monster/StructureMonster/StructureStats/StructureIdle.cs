using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class StructureIdle : BaseState<StructureMonster>
{
    public StructureIdle(StateHandler<StructureMonster> handler) : base(handler) { }

    public override void Enter(StructureMonster monster)
    {
        Debug.Log("Idle 시작");
    }

    public override void Update(StructureMonster monster)
    {
        monster.sMHandler.ChangeState(typeof(StructureMove));
    }

    public override void Exit(StructureMonster monster)
    {
        Debug.Log("Idle 종료");
    }
}
