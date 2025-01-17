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
        Debug.Log("Die진입");
        monster.animator.SetBool("Die", true);
        monster.DieParticle();
    }

    public override void Update(NormalMonster monster)
    {
        if (curTime > dieDuration)
        {
            monster.Die();
        }
        //Debug.Log("크아아악 죽는중");
        curTime = curTime + Time.deltaTime;
    }

    public override void Exit(NormalMonster monster)
    {
        Debug.Log("사망");
    }
}
