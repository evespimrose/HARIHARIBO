using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureMove : BaseState<StructureMonster>
{
    public StructureMove(StateHandler<StructureMonster> handler) : base(handler) { }

    public override void Enter(StructureMonster monster)
    {
        Debug.Log("Move 시작");
    }

    public override void Update(StructureMonster monster)
    {
        monster.Move();
    }

    public override void Exit(StructureMonster monster)
    {
        Debug.Log("Move 종료");
    }
}
