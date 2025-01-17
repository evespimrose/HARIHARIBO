using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class BossMonsterIdle : BaseState<BossMonster>
{
    public BossMonsterIdle(StateHandler<BossMonster> handler) : base(handler) { }

    public override void Enter(BossMonster monster)
    {
        Debug.Log("Idle진입");
        monster.animator?.SetTrigger("Idle");
    }

    public override void Update(BossMonster monster)
    {
        if (monster.isAction == false)
        {
            monster.StartCoroutine(monster.AtkSet());
        }
        if (monster.isChase) monster.bMHandler.ChangeState(typeof(BossMonsterMove));
        //monster.bMHandler.ChangeState(typeof(BossMonsterSkillE));
        //분류 1 : BossMonsterSkillA , BossMonsterSkillD , BossMonsterSkillE
        //분류 2 : BossMonsterSkillB , BossMonsterSkillC
        //이동기 : BossMonsterSkillF , BossMonsterSkillG
        //막타 : BossMonsterAtk
        //공격묶음 : 분류1 => 0.5초 대기(이동) => 분류2 => 타겟팅 => 0.5초 대기(이동) => 막타
        //공격묶음 => 0.5초 대기(이동) => 이동기 -> 다시 반복
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("Idle퇴장");
    }
}
