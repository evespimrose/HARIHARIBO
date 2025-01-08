using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHit : NormalMonsterBaseState
{
    private NormalMonster normalMonster;

    public float hitDuration;
    private float curTime;
    private bool isHit = false;
    public override void Enter(BaseCharacter entity)
    {
        normalMonster = entity as NormalMonster;
        normalMonster.animator.SetTrigger("Hit");
        isHit = true;
        curTime = 0f;
    }

    public override void Update(BaseCharacter entity)
    {
        if (curTime < hitDuration)
        {
            curTime += Time.deltaTime;
        }
        else
        {
            isHit = false;
        }
        if (isHit == false)
        {
            normalMonster.ChangeState(new MonsterIdle());
            //³ª°¡±â
        }
    }

    public override void Exit(BaseCharacter entity)
    {

    }
}
