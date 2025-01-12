using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillG : BaseState<BossMonster>
{
    public BossMonsterSkillG(StateHandler<BossMonster> handler) : base(handler) { }

    // 스킬7 대쉬 + 공격
    public float atkDelay = 1f;
    private float curTime = 0f;
    private float endTime = 0.5f; // 텔레포트 후 대기 시간
    private bool isAction = false; // 텔레포트 여부를 추적

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillG 진입");
        curTime = 0f;
        isAction = false;
    }

    public override void Update(BossMonster monster)
    {
        curTime += Time.deltaTime;

        if (isAction == false && curTime >= atkDelay)
        {
            // 텔레포트 수행
            SkillGAtk(monster);
            isAction = true; // 텔레포트 완료
            curTime = 0f; // 텔레포트 후 대기를 위해 시간 초기화
        }
        else if (isAction == true && curTime >= endTime)
        {
            // 텔레포트 후 다음 상태로 전환
            monster.bMHandler.ChangeState(typeof(BossMonsterSkillB));
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillG 종료");
    }

    public void SkillGAtk(BossMonster monster)
    {
        // 타겟 방향 벡터 계산
        Vector3 TargetDir = (monster.target.position - monster.transform.position).normalized;

        // 타겟의 뒤쪽 위치 계산
        Vector3 teleportPos = monster.target.position - TargetDir * 2f; // 2f는 거리 (필요시 조정 가능)

        // Y축은 기존 몬스터 높이를 유지
        teleportPos.y = monster.transform.position.y;

        // 몬스터 위치 설정
        monster.transform.position = teleportPos;

        Debug.Log($"몬스터가 타겟 뒤로 이동: {teleportPos}");
    }
}

