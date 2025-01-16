using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterStun : BaseState<EliteMonster>
{
    public EliteMonsterStun(StateHandler<EliteMonster> handler) : base(handler) { }

    public float stunTime = 5f;
    private float curTime = 0;

    public override void Enter(EliteMonster monster)
    {
        Debug.Log("스턴진입");
        monster.animator.SetBool("Stun", true);
        curTime = 0;
    }

    public override void Update(EliteMonster monster)
    {

        if (curTime >= stunTime - 0.1f)
        {
            monster.eMHandler.ChangeState(typeof(EliteMonsterIdle));
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(EliteMonster monster)
    {
        monster.isStunAction = false;
        monster.isStun = false;
        Debug.Log("스턴종료");
        monster.animator.SetBool("Stun", false);
    }
}
