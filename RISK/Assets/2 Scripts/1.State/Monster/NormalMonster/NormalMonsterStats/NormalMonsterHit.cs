using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class NormalMonsterHit : BaseState<NormalMonster>
{
    public NormalMonsterHit(StateHandler<NormalMonster> handler) : base(handler) { }

    public float hitDuration = 0.5f;
    private float curTime;

    public override void Enter(NormalMonster monster)
    {
        Debug.Log("Hit In");
        AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName("Hit") || stateInfo.normalizedTime >= 1.0f)
        {
            // 트리거 리셋 후 새로운 트리거 설정
            monster.animator.ResetTrigger("Hit");
        }
        monster.animator.SetTrigger("Hit");
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
            monster.isHit = false;
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
        }
    }

    public override void Exit(NormalMonster monster)
    {
        Debug.Log("Hit Out");
    }
}
