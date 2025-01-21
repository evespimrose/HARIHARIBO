using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillG : BaseState<BossMonster>
{
    public BossMonsterSkillG(StateHandler<BossMonster> handler) : base(handler) { }

    // 스킬7 대쉬 + 공격
    public float atkDelay = 2.5f; // 텔레포트 전 대기 시간
    public float endTime = 1f;
    public float additionalWaitTime = 0.5f;

    private Coroutine action;

    public override void Enter(BossMonster monster)
    {
        monster.isAtk = true;
        Debug.Log("SkillG 진입");
        action = monster.StartCoroutine(SkillGCoroutine(monster)); // 코루틴 시작
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillG 종료");
        monster.StopCoroutine(action);
    }

    private IEnumerator SkillGCoroutine(BossMonster monster)
    {
        monster.skillGPrefab.SetActive(true);
        monster.animator.SetTrigger("SkillG");
        monster.TargetLook(monster.target.position);
        // 첫 번째 대기 시간 후 텔레포트
        yield return new WaitForSeconds(atkDelay);
        monster.skillGPrefab.SetActive(false);
        // 텔레포트 수행
        SkillGAtk(monster);
        yield return new WaitForSeconds(endTime);

        yield return null;
        monster.bMHandler.ChangeState(typeof(BossMonsterSkillB));
    }

    public void SkillGAtk(BossMonster monster)
    {
        // 타겟 방향 벡터 계산
        Vector3 targetDir = (monster.target.position - monster.transform.position).normalized;

        // 타겟의 뒤쪽 위치 계산
        Vector3 teleportPos = monster.target.position - targetDir * 2f; // 2f는 거리 (필요시 조정 가능)

        // Y축은 기존 몬스터 높이를 유지
        teleportPos.y = monster.transform.position.y;

        // 몬스터 위치 설정
        monster.transform.position = teleportPos;

        Debug.Log($"몬스터가 타겟 뒤로 이동: {teleportPos}");
    }
}
