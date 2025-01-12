using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillB : BaseState<BossMonster>
{
    public BossMonsterSkillB(StateHandler<BossMonster> handler) : base(handler) { }
    //스킬2 범위기 1
    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillB 진입");
        //애니메이션 실행
        SkellBAtk(monster);//애니메이션과 동시실행 살짝텀줄거면 업데이트쪽으로옮겨서 curTime 체크해서 진행
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {

    }

    public void SkellBAtk(BossMonster monster)
    {
        GameObject skillBObj = monster.ObjSpwan(monster.skillBPrefab, monster.target.position);
        skillBObj.GetComponent<BossSkillBObject>().Seting(monster.target.position, monster.atkDamage);
    }
}
