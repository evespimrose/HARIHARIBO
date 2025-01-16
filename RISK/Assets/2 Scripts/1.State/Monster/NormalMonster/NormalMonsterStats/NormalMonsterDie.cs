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
        Debug.Log("DieÁøÀÔ");
        monster.animator.SetTrigger("Die");
        monster.DieParticle();
    }

    public override void Update(NormalMonster monster)
    {
        if (curTime > dieDuration)
        {
            monster.Die();
        }
        //Debug.Log("Å©¾Æ¾Æ¾Ç Á×´ÂÁß");
        curTime = curTime + Time.deltaTime;
    }

    public override void Exit(NormalMonster monster)
    {
        Debug.Log("»ç¸Á");
    }
}
