using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillD : BaseState<BossMonster>
{
    public BossMonsterSkillD(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkDelay = 0f; // 선딜레이
    public float skillDDuration = 2.4f; // 애니메이션 지속 시간
    public float skillFAtkTiem = 1f; // 애니메이션 시작 후 SkillDAtk 실행까지 기다릴 시간 (필드값)
    private float startTime;
    private bool isAction = false; // SkillDAtk가 한 번만 실행되도록 관리

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillD 진입");

        // 선딜레이가 있을 경우 선딜레이 후 애니메이션 실행
        startTime = Time.time + atkDelay; // 선딜레이를 고려한 시작 시간

        // 선딜레이가 0일 때 즉시 애니메이션 실행
        if (atkDelay <= 0f)
        {
            // 애니메이션이 이미 실행되고 있지 않은 경우에만 트리거 설정
            if (!monster.animator.GetCurrentAnimatorStateInfo(0).IsName("SkillD"))
            {
                monster.animator.SetTrigger("SkillD");
                Debug.Log("SkillD 애니메이션 시작");
            }
        }
    }

    public override void Update(BossMonster monster)
    {
        // 선딜레이가 끝난 후 애니메이션 실행 (한 번만 실행되도록)
        if (Time.time >= startTime && !monster.animator.GetCurrentAnimatorStateInfo(0).IsName("SkillD"))
        {
            monster.animator.SetTrigger("SkillD"); // 애니메이션 실행
            Debug.Log("SkillD 애니메이션 시작");
        }

        // 애니메이션이 시작된 후 일정 시간이 지나면 SkillDAtk 실행
        if (Time.time - startTime >= skillFAtkTiem && !isAction)
        {
            SkillDAtk(monster); // SkillDAtk 실행
            isAction = true; // SkillDAtk는 한 번만 실행되도록 플래그 설정
        }

        // 애니메이션이 끝난 후 0.2초 여유를 두고 상태 전환
        if (Time.time - startTime >= skillDDuration + 0.2f)
        {
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // 상태 전환
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillD 종료");
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

            // 오브젝트 생성 및 위치 설정
            GameObject skillDObj = monster.ObjSpwan(monster.skillDPrefab, monster.transform.position);

            // 방향 설정 (소환된 오브젝트의 전방이 방향을 가리키게 설정)
            skillDObj.transform.rotation = Quaternion.LookRotation(rotDir, Vector3.up);

            // 데미지 및 기타 설정 초기화
            skillDObj.GetComponent<BossSkillDObject>().Seting(monster.atkDamage);
        }
    }
}
