using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterDie : BaseState<EliteMonster>
{
    public EliteMonsterDie(StateHandler<EliteMonster> handler) : base(handler) { }

    public float dieDuration = 0.8f;
    public float curTime;

    public override void Enter(EliteMonster monster)
    {
        monster.monsterDebuff.DebuffAllOff();
        monster.animator.SetTrigger("Die");
        monster.DieParticle();
    }

    public override void Update(EliteMonster monster)
    {
        if (curTime > dieDuration)
        {
            monster.Die();
        }
        //Debug.Log("크아아악 죽는중");
        curTime = curTime + Time.deltaTime;
    }

    public override void Exit(EliteMonster monster)
    {
        Debug.Log("사망");
    }
}
