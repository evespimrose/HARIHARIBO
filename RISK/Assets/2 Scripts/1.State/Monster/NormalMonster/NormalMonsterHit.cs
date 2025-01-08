using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterHit : BaseState<NormalMonster>
{
    public NormalMonsterHit(StateHandler<NormalMonster> handler) : base(handler) { }

    public float hitDuration;
    private float curTime;
    private bool isHit = false;
    public override void Enter(NormalMonster entity)
    {
        entity.animator.SetTrigger("Hit");
        isHit = true;
        curTime = 0f;
    }

    public override void Update(NormalMonster entity)
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
            //entity.ChangeState(new MonsterIdle());
            //³ª°¡±â
        }
    }

    public override void Exit(NormalMonster entity)
    {

    }
}
