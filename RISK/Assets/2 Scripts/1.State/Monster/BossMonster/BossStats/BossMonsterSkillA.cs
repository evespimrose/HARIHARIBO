using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillA : BaseState<BossMonster>
{
    public BossMonsterSkillA(StateHandler<BossMonster> handler) : base(handler) { }

    public enum CurAction
    {
        Start,
        SkillAtkA,
        SkillAtkB,
        End
    }
    private CurAction curAction = CurAction.Start;
    public float atkRange = 20f;
    //공격 판정 딜레이
    public float startDelay = 0f;
    public float atkADelay = 0.38f;
    public float atkBDelay = 0.38f;
    //애니메이션 지속 시간
    public float atkADuration = 1.34f; // 애니메이션 시간 1.34초
    public float atkBDuration = 1.34f; // 애니메이션 시간 1.34초
    //End로 일찍 들어갈시간
    public float endTime = 0.2f;
    private float startTime;
    private bool isSkillAHit = false;
    private bool isSkillBHit = false;
    private bool isAction = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("공격 시작");
        curAction = CurAction.SkillAtkA;
        startTime = Time.time + startDelay;
        isSkillAHit = false;
        isSkillBHit = false;
        isAction = false;
    }

    public override void Update(BossMonster monster)
    {
        float elapsedTime = Time.time - startTime;
        switch (curAction)
        {
            case CurAction.SkillAtkA:
                if (!isAction && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 1);//SkillAtkA 애니메이션 실행
                    isAction = true;
                }
                if (elapsedTime >= atkADelay && !isSkillAHit)
                {
                    AttackHit(monster, 1);//SkillAtkA 공격 판정
                    isSkillAHit = true;
                }
                // 1.2초 후 SkillAtkB로 전환
                if (elapsedTime >= 1.2f) 
                {
                    ChangeAction(monster, CurAction.SkillAtkB);//SkillAtkA 애니메이션이 끝나기 전에 SkillAtkB로 전환
                }
                break;
            case CurAction.SkillAtkB:
                if (!isAction && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 2);//SkillAtkB 애니메이션 실행
                    isAction = true;
                }
                if (elapsedTime >= atkBDelay && !isSkillBHit)
                {
                    AttackHit(monster, 2);//SkillAtkB 공격 판정
                    isSkillBHit = true;
                }
                // endTime 후 Idle로 전환
                if (elapsedTime >= atkBDuration + endTime)
                {
                    ChangeAction(monster, CurAction.End);
                }
                break;
            case CurAction.End:
                monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
                break;
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("공격 종료");
    }

    private void AttackAnimationPlay(BossMonster monster, int animationNumber)
    {
        switch (animationNumber)
        {
            case 1:
                monster.animator.SetTrigger("SkillA1");
                break;
            case 2:
                monster.animator.SetTrigger("SkillA2");
                break;
        }
        Debug.Log($"{curAction} 애니메이션 재생");
    }

    private void AttackHit(BossMonster monster, int actionNumber)
    {
        Vector3 atkDir;
        switch (actionNumber)
        {
            case 1:
                atkDir = Quaternion.Euler(0, 45f, 0) * monster.transform.forward;
                break;
            case 2:
                atkDir = Quaternion.Euler(0, -45f, 0) * monster.transform.forward;
                break;
            default:
                atkDir = Quaternion.Euler(0, 45f, 0) * monster.transform.forward;
                break;
        }
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                float angle = Vector3.Angle(atkDir, dirToTarget);
                if (angle <= 135f)
                {
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(monster.atkDamage);
                    Debug.Log($"{curAction} 공격 성공");
                }
                else
                {
                    Debug.Log($"{curAction} 공격 실패 - 범위 밖");
                }
            }
        }
    }

    private void ChangeAction(BossMonster monster, CurAction nextPhase)
    {
        curAction = nextPhase;

        if (nextPhase != CurAction.End)
        {
            startTime = Time.time; // 다음 단계로 이동 시 시간 초기화
            isAction = false;   // 애니메이션 실행 준비
        }
    }
}
