using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterDie : BaseState<NormalMonster>
{
    public NormalMonsterDie(StateHandler<NormalMonster> handler) : base(handler) { }

    public float dieDuration = 0.8f;
    public float curTime;

    public override void Enter(NormalMonster monster)
    {
        monster.monsterDebuff.DebuffAllOff();
        monster.animator.SetTrigger("Die");
        monster.DieParticle();
    }

    public override void Update(NormalMonster monster)
    {
        if (curTime > dieDuration)
        {
            monster.Die();
        }
        //Debug.Log("?ъ븘?꾩븙 二쎈뒗以?);
        curTime = curTime + Time.deltaTime;
    }

    public override void Exit(NormalMonster monster)
    {
        ;
    }
}
