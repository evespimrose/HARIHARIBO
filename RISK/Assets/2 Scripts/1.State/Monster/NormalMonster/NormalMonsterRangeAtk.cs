using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterRangeAtk : BaseState<NormalMonster>
{
    public NormalMonsterRangeAtk(StateHandler<NormalMonster> handler) : base(handler) { }

    public float atkDuration = 1f;
    public float atkDelay = 0.4f;
    private float curTime = 0;
    private bool isAtk = false;

    public override void Enter(NormalMonster entity)
    {
        entity.animator.SetTrigger("Atk");
        curTime = 0;
        isAtk = false;
    }

    public override void Update(NormalMonster entity)
    {
        if (curTime - atkDuration < 0.1f)
        {
            //공격종료
            //entity.ChangeState(new MonsterIdle());
        }
        if (curTime - atkDelay < 0.1f && isAtk == false)
        {
            Atk(entity);
            entity.isAtk = true;
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(NormalMonster entity)
    {
         
    }

    private void Atk(NormalMonster entity)
    {

    }
}
