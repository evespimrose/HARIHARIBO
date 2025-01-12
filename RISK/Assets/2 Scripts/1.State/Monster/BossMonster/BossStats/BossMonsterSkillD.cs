using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillD : BaseState<BossMonster>
{
    public BossMonsterSkillD(StateHandler<BossMonster> handler) : base(handler) { }
    //스킬4 원거리 공격 1
    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillC 진입");
        //애니메이션 실행
        SkillDAtk(monster);//애니메이션과 동시실행 살짝텀줄거면 업데이트쪽으로옮겨서 curTime 체크해서 진행
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {

    }

    public void SkillDAtk(BossMonster monster)
    {
        // 360도에서 8방향으로 나누기
        int atkCount = 8;
        float angleStep = 360f / atkCount;

        for (int i = 0; i < atkCount; i++)
        {
            // 각 방향의 회전각 계산
            float angle = i * angleStep;

            // 월드 좌표계에서 회전각도를 구하고 회전 방향 벡터 계산
            Vector3 rotDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Y축 방향 제거하여 가로로 제한
            rotDir = new Vector3(rotDir.x, 0f, rotDir.z).normalized;

            // 오브젝트 생성 및 위치 설정
            GameObject skillDObj = monster.ObjSpwan(monster.skillDPrefab, monster.transform.position);

            // 방향 설정 (소환된 오브젝트의 전방이 방향을 가리키게 설정)
            skillDObj.transform.rotation = Quaternion.LookRotation(rotDir, Vector3.up);

            // 데미지 및 기타 설정 초기화
            skillDObj.GetComponent<BossSkillDObject>().Seting(monster.atkDamage);
        }
    }
}
