using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRangeAtk : NormalMonsterBaseState
{
    private NormalMonster normalMonster;

    public float atkDuration = 1f;
    public float atkDelay = 0.4f;
    private float curTime = 0;
    private bool isAtk = false;

    public override void Enter(BaseCharacter entity)
    {
        normalMonster = entity as NormalMonster;
        entity.animator.SetTrigger("Atk");
        curTime = 0;
        isAtk = false;
    }

    public override void Update(BaseCharacter entity)
    {
        if (curTime - atkDuration < 0.1f)
        {
            //공격종료
            normalMonster.ChangeState(new MonsterIdle());
        }
        if (curTime - atkDelay < 0.1f && isAtk == false)
        {
            Atk(normalMonster);
            normalMonster.isAtk = true;
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(BaseCharacter entity)
    {
         
    }

    private void Atk(NormalMonster entity)
    {

    }
}
