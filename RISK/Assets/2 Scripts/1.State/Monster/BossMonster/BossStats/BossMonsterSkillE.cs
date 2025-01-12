using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillE : BaseState<BossMonster>
{
    public BossMonsterSkillE(StateHandler<BossMonster> handler) : base(handler) { }
    //스킬5 원거리 공격 2
    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillE 진입");
        //애니메이션 실행
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {

    }

    public void SkillEAtk(BossMonster monster)
    {

    }
}