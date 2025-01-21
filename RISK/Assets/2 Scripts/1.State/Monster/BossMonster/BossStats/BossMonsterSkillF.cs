using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillF : BaseState<BossMonster>
{
    public BossMonsterSkillF(StateHandler<BossMonster> handler) : base(handler) { }

    // 스킬6 대쉬 밀격
    public Vector3 dashDirection; // 대쉬 방향
    public float atkDelay = 2f;//선딜
    public float atkTime = 1f; // 대쉬 지속 시간
    public float skillFMoveSpeed = 20f; // 대쉬 속도
    public float dashDistance = 5f; // 대쉬 이동 거리
    public float slowSpeed = 0.1f; // 벽에 충돌할 때 느려지는 속도 비율

    public float additionalWaitTime = 0.5f;

    private Coroutine action;

    public override void Enter(BossMonster monster)
    {
        monster.skillFDamage = monster.atkDamage * 1.33f;
        monster.isAtk = true;
        action = monster.StartCoroutine(SkillFCoroutine(monster)); // 코루틴 시작
        monster.Targeting(); // 타겟 새로 설정

        // 대쉬 시작 방향을 설정 (현재 바라보는 방향)
        dashDirection = monster.transform.forward;

        // 초기 회전 설정 (대쉬 방향으로 회전)
        monster.TargetLook(monster.transform.position + dashDirection);

        monster.SkillFReset(); // 스킬 초기화
        Debug.Log("SkillF 진입");
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillF 종료");
        monster.StopCoroutine(action);
        monster.isAtk = false;
    }

    private IEnumerator SkillFCoroutine(BossMonster monster)
    {
        // 선딜레이
        monster.skillFPrefabA.SetActive(true);
        yield return new WaitForSeconds(atkDelay);

        // 애니메이션 실행
        monster.skillFPrefabA.SetActive(false);
        monster.skillFPrefabB.SetActive(true);
        monster.animator.SetBool("SkillF", true);
        // 대쉬 시작
        monster.isMoving = true;
        Debug.Log("돌진 시작");

        Rigidbody rb = monster.GetComponent<Rigidbody>();
        Vector3 dashDirection = monster.transform.forward;
        Vector3 moveDirection = dashDirection * skillFMoveSpeed;
        // 대쉬 지속 시간 동안 이동
        float elapsedTime = 0f; 
        while (elapsedTime < atkTime)
        {
            if (monster.isMoving)
            {
                if (monster.isWall)
                {
                    moveDirection *= slowSpeed;
                }
                rb.velocity = moveDirection;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        monster.skillFPrefabB.SetActive(false);
        monster.isMoving = false;
        monster.SkillFReset();
        monster.RBStop();
        Debug.Log("돌진 종료");
        monster.animator.SetBool("SkillF", false);
        yield return null;

        yield return new WaitForSeconds(additionalWaitTime);

        yield return null;
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
    }


}
