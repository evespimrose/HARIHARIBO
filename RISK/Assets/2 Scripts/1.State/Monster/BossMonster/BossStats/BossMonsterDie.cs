using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterDie : BaseState<BossMonster>
{
    public BossMonsterDie(StateHandler<BossMonster> handler) : base(handler) { }

    public float dieDuration = 0.8f;
    public float curTime;

    public override void Enter(BossMonster monster)
    {
        monster.StopCoroutine(monster.action);
        Debug.Log("Die진입");
        monster.animator.SetTrigger("Die");
        monster.DieParticle();
    }

    public override void Update(BossMonster monster)
    {
        if (curTime > dieDuration)
        {
            monster.Die();
        }
        //Debug.Log("크아아악 죽는중");
        curTime = curTime + Time.deltaTime;
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("사망");
    }
}
