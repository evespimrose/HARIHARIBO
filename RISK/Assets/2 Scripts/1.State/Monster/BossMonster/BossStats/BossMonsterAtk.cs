using UnityEngine;

public class BossMonsterAtk : BaseState<BossMonster>
{
    public BossMonsterAtk(StateHandler<BossMonster> handler) : base(handler) { }

    public enum CurAction
    {
        Start,
        AtkA,
        AtkB,
        AtkC,
        End
    }
    private CurAction curAction = CurAction.Start;
    public float atkRange = 20f;
    //공격 판정 딜레이
    public float atkDelayA = 0.17f;
    public float atkDelayB = 0.43f;
    public float atkDelayC = 0.18f;
    //애니메이션 지속 시간
    public float atkADuration = 1.4f;
    public float atkBDuration = 1.6f;
    public float atkCDuration = 1.8f;
    //애니메이션 전환 시점을 설정할 시간
    public float nextBTime = 0.45f;//AtkA -> AtkB로 넘어가는 시간
    public float nextCTime = 1f;//AtkB -> AtkC로 넘어가는 시간
    public float nextEndTime = 1.3f;//AtkC -> End로 넘어가는 시간

    //스테이트 들어온 뒤 선딜레이
    public float startDelay = 0.5f;

    private float phaseStartTime;
    private float startTime = 0f;

    private bool isAtkAHit = false;
    private bool isAtkBHit = false;
    private bool isAtkCHit = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("공격 시작");
        curAction = CurAction.AtkA;
        phaseStartTime = Time.time + startDelay;//선딜레이 포함하여 공격 시작 시간 기록
        startTime = 0f;//애니메이션 시작 이후 시간 초기화
        isAtkAHit = false;
        isAtkBHit = false;
        isAtkCHit = false;
        AttackAnimationPlay(monster, 1);
    }

    public override void Update(BossMonster monster)
    {
        startTime = Time.time - phaseStartTime;//경과 시간 계산

        if (curAction == CurAction.End)
        {
            //End 상태에서는 공격이 종료된 후 바로 Idle 상태로 전환
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
            return;//End 상태에서 바로 전환하므로 더 이상 진행할 필요 없음
        }
        switch (curAction)
        {
            case CurAction.AtkA:
                //AtkA 공격 판정 딜레이
                if (!isAtkAHit && startTime >= atkDelayA)
                {
                    AttackHit(monster, 105f);//AtkA 공격 판정
                    isAtkAHit = true;//공격 판정이 한 번만 실행되도록 함
                }
                //AtkA 애니메이션이 nextBTime 시점에 도달하면 AtkB로 전환
                if (startTime >= nextBTime && startTime < atkADuration)
                {
                    ChangeAction(monster, CurAction.AtkB);
                }
                break;
            case CurAction.AtkB:
                //AtkB 애니메이션 실행 후 atkDelayB 만큼 기다린 후 공격 판정 실행
                if (!isAtkBHit && startTime >= atkDelayB)
                {
                    AttackHit(monster, 45f);//AtkB 공격 판정
                    isAtkBHit = true;//공격 판정이 한 번만 실행되도록 함
                }
                //AtkB 애니메이션이 nextCTime 시점에 도달하면 AtkC로 전환
                if (startTime >= nextCTime && startTime < atkBDuration)
                {
                    ChangeAction(monster, CurAction.AtkC);
                }
                break;
            case CurAction.AtkC:
                //AtkC 애니메이션 실행 후 atkDelayC 만큼 기다린 후 공격 판정 실행
                if (!isAtkCHit && startTime >= atkDelayC)
                {
                    AttackHit(monster, 180f);//AtkC 공격 판정
                    isAtkCHit = true;//공격 판정이 한 번만 실행되도록 함
                }
                //AtkC 애니메이션이 nextEndTime 시점에 도달하면 End로 전환
                if (startTime >= nextEndTime && startTime < atkCDuration)
                {
                    ChangeAction(monster, CurAction.End);
                }
                break;
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("공격 종료");
    }

    private void AttackAnimationPlay(BossMonster monster, int animationNumber)
    {
        //현재 실행 중인 애니메이션이 무엇인지 확인하고 겹치지 않게 실행
        if (monster.animator.GetCurrentAnimatorStateInfo(0).IsName("AtkA") && animationNumber == 1 ||
            monster.animator.GetCurrentAnimatorStateInfo(0).IsName("AtkB") && animationNumber == 2 ||
            monster.animator.GetCurrentAnimatorStateInfo(0).IsName("AtkC") && animationNumber == 3)
        {
            return;//애니메이션이 재생중이면 리턴
        }
        switch (animationNumber)
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
            phaseStartTime = Time.time;//시간 초기화
            //애니메이션을 시작하기 전에 이미 트리거가 됐으므로, 바로 다음 애니메이션을 실행
            switch (nextPhase)
            {
                case CurAction.AtkB:
                    AttackAnimationPlay(monster, 2);
                    break;
                case CurAction.AtkC:
                    AttackAnimationPlay(monster, 3);
                    break;
            }
        }
    }
}
