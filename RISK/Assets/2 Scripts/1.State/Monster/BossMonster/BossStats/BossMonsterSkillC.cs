using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillC : BaseState<BossMonster>
{
    public BossMonsterSkillC(StateHandler<BossMonster> handler) : base(handler) { }
    //스킬3 범위기 2
    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillC 진입");
        //애니메이션 실행
        SkillCAtk(monster);//애니메이션과 동시실행 살짝텀줄거면 업데이트쪽으로옮겨서 curTime 체크해서 진행
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {

    }

    public void SkillCAtk(BossMonster monster)
    {
        GameObject skillBObj = monster.ObjSpwan(monster.skillCPrefab, monster.transform.position);
        // 현재 몬스터의 forward를 사용하지만, y축 방향을 0으로 설정하여 위아래 각도를 제거
        Vector3 forwardDir = new Vector3(monster.transform.forward.x, 0f, monster.transform.forward.z).normalized;
        // 방향 설정
        skillBObj.transform.forward = forwardDir;
        skillBObj.transform.position = new Vector3(skillBObj.transform.position.x, skillBObj.transform.position.y + 1f, skillBObj.transform.position.z);
        skillBObj.GetComponent<BossSkillCObject>().Seting(monster.atkDamage);
    }
}
