using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillF : BaseState<BossMonster>
{
    public BossMonsterSkillF(StateHandler<BossMonster> handler) : base(handler) { }

    // 스킬6 대쉬 밀격
    public Vector3 dashDirection; // 대쉬 방향
    public float atkTime = 1f; // 대쉬 지속 시간
    public float skillFMoveSpeed = 20f; // 대쉬 속도
    public float dashDistance = 5f; // 대쉬 이동 거리
    public float slowSpeed = 0.1f; // 벽에 충돌할 때 느려지는 속도 비율

    public override void Enter(BossMonster monster)
    {
        monster.StartSkillCoroutine(SkillFCoroutine(monster)); // 코루틴 시작
        monster.Targeting(); // 타겟 새로 설정

        // 대쉬 시작 방향을 설정 (현재 바라보는 방향)
        dashDirection = monster.transform.forward;

        // 초기 회전 설정 (대쉬 방향으로 회전)
        monster.TargetLook(monster.transform.position + dashDirection);

        monster.SkillFReset(); // 스킬 초기화
        Debug.Log("SkillF 진입");
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillF 종료");
        monster.SkillFReset();
        monster.animator.SetBool("SkillF", false); // 애니메이션 리셋
    }

    private IEnumerator SkillFCoroutine(BossMonster monster)
    {
        // 애니메이션 실행 (대쉬 시작)
        monster.animator.SetBool("SkillF", true);

        // 대쉬 시작
        monster.isMoving = true;
        Debug.Log("돌진 시작");

        float elapsedTime = 0f; // 경과 시간

        // 대쉬 지속 시간 동안 이동
        while (elapsedTime < atkTime)
        {
            if (monster.isMoving)
            {
                // 대쉬 방향으로 계속 이동
                Vector3 targetPosition = monster.transform.position + dashDirection * skillFMoveSpeed * Time.deltaTime;

                // 벽에 충돌했는지 체크
                if (monster.isWall)
                {
                    Debug.Log("벽에 충돌하여 속도 감소");
                    // 벽에 충돌하면 속도를 낮추기: 벽에 가까워지면 이동 속도를 더 낮추기
                    targetPosition = monster.transform.position + dashDirection * skillFMoveSpeed * slowSpeed * Time.deltaTime;
                }

                targetPosition.y = monster.transform.position.y;  // y축 고정
                monster.transform.position = targetPosition;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // 한 프레임 대기
        }

        // 대쉬 종료
        monster.isMoving = false;
        monster.SkillFReset();
        Debug.Log("돌진 종료");

        // 대쉬 종료 후 애니메이션을 끄고 0.2초 후 Idle로 전환
        yield return new WaitForSeconds(0.2f);
        monster.animator.SetBool("SkillF", false);

        // 상태 전환
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // 상태 전환
    }
}
