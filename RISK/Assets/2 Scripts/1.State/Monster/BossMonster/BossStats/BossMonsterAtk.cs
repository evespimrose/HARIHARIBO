using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterAtk : BaseState<BossMonster>
{
    public BossMonsterAtk(StateHandler<BossMonster> handler) : base(handler) { }

    public enum CurAction
    {
        Strat,
        AtkA,
        AtkB,
        AtkC,
        End
    }
    private CurAction curAction = CurAction.Strat;

    public float atkRange = 20f;

    public float attackDelay = 0.5f; // 공격 판정 전 딜레이
    public float atkADuration = 2f;  // AtkA 애니메이션 지속 시간
    public float atkBDuration = 2f;  // AtkB 애니메이션 지속 시간
    public float atkCDuration = 2f;  // AtkC 애니메이션 지속 시간

    private float phaseStartTime;
    private bool isActionEnd = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("공격 시작");
        curAction = CurAction.AtkA;
        phaseStartTime = Time.time + 1f; // 1초 대기 후 AtkA 애니메이션 시작
        isActionEnd = false;
    }

    public override void Update(BossMonster monster)
    {
        if (curAction == CurAction.End) return;

        float elapsedTime = Time.time - phaseStartTime;

        switch (curAction)
        {
            case CurAction.AtkA:
                if (!isActionEnd && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 1);
                    isActionEnd = true;
                }

                if (elapsedTime >= attackDelay && isActionEnd)
                {
                    AttackHit(monster, 105f); // AtkA 공격 판정
                }

                if (elapsedTime >= atkADuration)
                {
                    ChangeAction(monster, CurAction.AtkB);
                }
                break;

            case CurAction.AtkB:
                if (!isActionEnd && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 2);
                    isActionEnd = true;
                }

                if (elapsedTime >= attackDelay && isActionEnd)
                {
                    AttackHit(monster, 45f); // AtkB 공격 판정
                }

                if (elapsedTime >= atkBDuration)
                {
                    ChangeAction(monster, CurAction.AtkC);
                }
                break;

            case CurAction.AtkC:
                if (!isActionEnd && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 3);
                    isActionEnd = true;
                }

                if (elapsedTime >= attackDelay && isActionEnd)
                {
                    AttackHit(monster, 180f); // AtkC 공격 판정
                }

                if (elapsedTime >= atkCDuration)
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

    private void AttackAnimationPlay(BossMonster monster, int animationnumber)
    {
        switch (animationnumber)
        {
            case 1:
                monster.animator.SetTrigger("AtkA");
                break;
            case 2:
                monster.animator.SetTrigger("AtkB");
                break;
            case 3:
                monster.animator.SetTrigger("AtkC");
                break;
        }
        Debug.Log($"{curAction} 애니메이션 재생");
    }

    private void AttackHit(BossMonster monster, float angleThreshold)
    {
        Vector3 atkDir = monster.transform.forward;
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, atkRange);

        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                float angle = Vector3.Angle(atkDir, dirToTarget);

                if (angle <= angleThreshold)
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
            phaseStartTime = Time.time; // 다음 단계로 이동 시 시간 초기화
            isActionEnd = false;   // 애니메이션 실행 준비
        }
    }
}