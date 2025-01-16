using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterHit : BaseState<NormalMonster>
{
    public NormalMonsterHit(StateHandler<NormalMonster> handler) : base(handler) { }

    public float hitDuration = 0.5f;
    private float curTime;
    private bool isHit = false;

    public override void Enter(NormalMonster monster)
    {
        Debug.Log("Hit In");
        monster.animator.SetTrigger("Hit");
        isHit = true;
        curTime = 0f;
    }

    public override void Update(NormalMonster monster)
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
            monster.isHit = false;
            monster.isHitAction = false;
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
            //나가기
        }
    }

    public override void Exit(NormalMonster monster)
    {
        Debug.Log("Hit Out");
    }
}
