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
        monster.StartCoroutine(StartStun(monster));
    }

    public override void Update(StructureMonster monster)
    {
        if (monster.isHit == true)
        {
            monster.isHit = false;
        }
        if (monster.isAirborne == true)
        {
            Debug.Log("스턴도중에 에어본들어와서 스턴캔슬");
            monster.StopCoroutine(StartStun(monster));
            monster.isStun = false;
            monster.isHitAction = false;
            monster.sMHandler.ChangeState(typeof(StructureAirborne));
        }
        else if (!monster.isStun && !monster.isStunAction)
        {
            monster.sMHandler.ChangeState(typeof(StructureIdle));
        }
    }

    public override void Exit(StructureMonster monster)
    {
        monster.isStunAction = false;
        monster.isStun = false;
        Debug.Log("스턴종료");
    }

    private IEnumerator StartStun(StructureMonster monster)
    {
        yield return new WaitForSeconds(stunTime);
        monster.isStun = false;
        monster.isHitAction = false;
    }
}
