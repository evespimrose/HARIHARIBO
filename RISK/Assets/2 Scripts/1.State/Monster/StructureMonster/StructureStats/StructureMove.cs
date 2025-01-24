using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureMove : BaseState<StructureMonster>
{
    public StructureMove(StateHandler<StructureMonster> handler) : base(handler) { }

    public override void Enter(StructureMonster monster)
    {
    }

    public override void Update(StructureMonster monster)
    {
        if (Vector3.Distance(monster.target.position, monster.transform.position) > 1f)
        {
            monster.Move();
        }
    }

    public override void Exit(StructureMonster monster)
    {
    }
}
