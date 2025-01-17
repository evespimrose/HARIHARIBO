using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NormalMonster;

public class NormalMonsterStun : BaseState<NormalMonster>
{
    public NormalMonsterStun(StateHandler<NormalMonster> handler) : base(handler) { }

    public float stunTime = 5f;

    public override void Enter(NormalMonster monster)
    {
        Debug.Log("스턴진입");
        monster.animator.SetBool("Stun", true);
        monster.StartCoroutine(StartStun(monster));
    }

    public override void Update(NormalMonster monster)
    {
        if (monster.isAirborne == true)
        {
            Debug.Log("스턴도중에 에어본들어와서 스턴캔슬");
            monster.StopCoroutine(StartStun(monster));
            monster.isStun = false;
            monster.isHitAction = false;
            monster.nMHandler.ChangeState(typeof(NormalMonsterAirborne));
        }
        else if (!monster.isStun && !monster.isStunAction)
        {
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
        }
    }

    public override void Exit(NormalMonster monster)
    {
        monster.isStunAction = false;
        monster.isStun = false;
        Debug.Log("스턴종료");
        monster.animator.SetBool("Stun", false);
    }

    private IEnumerator StartStun(NormalMonster monster)
    {
        yield return new WaitForSeconds(stunTime);
        monster.isStun = false;
        monster.isHitAction = false;
    }
}
