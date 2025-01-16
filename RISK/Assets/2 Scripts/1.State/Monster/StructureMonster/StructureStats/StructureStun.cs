using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureStun : BaseState<StructureMonster>
{
    public StructureStun(StateHandler<StructureMonster> handler) : base(handler) { }

    public float stunTime = 5f;
    private float curTime = 0;

    public override void Enter(StructureMonster monster)
    {
        Debug.Log("스턴진입");
        monster.isStunAction = true;
        curTime = 0;
    }

    public override void Update(StructureMonster monster)
    {
        if (monster.isAirborne == true)
        {
            Debug.Log("스턴도중에 에어본들어와서 스턴캔슬");
            monster.sMHandler.ChangeState(typeof(StructureAirborne));
        }
        if (curTime >= stunTime - 0.1f)
        {
            monster.sMHandler.ChangeState(typeof(StructureIdle));
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(StructureMonster monster)
    {
        monster.isStunAction = false;
        monster.isStun = false;
        Debug.Log("스턴종료");
    }
}
