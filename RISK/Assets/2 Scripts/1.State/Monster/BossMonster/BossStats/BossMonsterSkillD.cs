using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillD : BaseState<BossMonster>
{
    public BossMonsterSkillD(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkDelay = 0f; // 선딜레이
    public float skillDDuration = 2.4f; // 애니메이션 지속 시간
    public float skillFAtkTime = 1f; // 애니메이션 시작 후 SkillDAtk 실행까지 기다릴 시간

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillD 진입");
        monster.StartSkillCoroutine(SkillDCoroutine(monster)); // 코루틴 시작
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillD 종료");
    }

    private IEnumerator SkillDCoroutine(BossMonster monster)
    {
        // 선딜레이 후 애니메이션 실행
        yield return new WaitForSeconds(atkDelay);
        monster.animator.SetTrigger("SkillD");
        Debug.Log("SkillD 애니메이션 시작");

        // 애니메이션 시작 후 일정 시간이 지나면 SkillDAtk 실행
        yield return new WaitForSeconds(skillFAtkTime);
        SkillDAtk(monster);

        // 애니메이션 지속 시간 후 0.2초 여유를 두고 상태 전환
        yield return new WaitForSeconds(skillDDuration + 0.2f);
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // 상태 전환
        Debug.Log("SkillD 종료 후 Idle 상태로 전환");
    }

    private void SkillDAtk(BossMonster monster)
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

            // 스킬 객체 생성
            GameObject skillDObj = monster.ObjSpwan(monster.skillDPrefab, monster.transform.position);
            // 소환된 오브젝트의 전방이 회전 방향을 가리키도록 설정
            skillDObj.transform.rotation = Quaternion.LookRotation(rotDir, Vector3.up);
            skillDObj.GetComponent<BossSkillDObject>().Seting(monster.atkDamage);
        }
    }
}
