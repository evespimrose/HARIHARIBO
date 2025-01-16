using System.Collections;
using System.Threading;
using UnityEngine;

public class BossMonsterAtk : BaseState<BossMonster>
{
    public BossMonsterAtk(StateHandler<BossMonster> handler) : base(handler) { }

    // 공격 판정 딜레이
    public float atkDelayA = 0.17f;
    public float atkDelayB = 0.43f;
    public float atkDelayC = 0.18f;

    // 애니메이션 지속 시간
    public float atkADuration = 1.4f;
    public float atkBDuration = 1.6f;
    public float atkCDuration = 1.8f;

    // 애니메이션 전환 시점을 설정할 시간
    public float nextBTime = 0.45f; // AtkA -> AtkB로 넘어가는 시간
    public float nextCTime = 1f;    // AtkB -> AtkC로 넘어가는 시간
    public float nextEndTime = 1.3f; // AtkC -> End로 넘어가는 시간

    // 스테이트 들어온 뒤 선딜레이
    public float startDelay = 0.5f;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("Atk 시작");
        monster.isAtk = true;

        // 선딜레이 후 코루틴 시작
        monster.StartSkillCoroutine(AttackCoroutine(monster));
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("Atk 종료");
        monster.isAtk = false;
    }

    private IEnumerator AttackCoroutine(BossMonster monster)
    {
        // 선딜레이
        yield return new WaitForSeconds(startDelay);
        monster.TargetLook(monster.target.position);

        // 첫 번째 공격 - AtkA
        monster.animator.SetTrigger("AtkA");
        yield return new WaitForSeconds(atkDelayA); // 공격 판정 딜레이
        AttackHit(monster, 105f); // 공격 판정

        // AtkA 애니메이션에서 AtkB로 전환
        yield return new WaitForSeconds(nextBTime); // 애니메이션 전환 시점

        // 두 번째 공격 - AtkB
        monster.animator.SetTrigger("AtkB");
        yield return new WaitForSeconds(atkDelayB); // 공격 판정 딜레이
        AttackHit(monster, 45f); // 공격 판정

        // AtkB 애니메이션에서 AtkC로 전환
        yield return new WaitForSeconds(nextCTime); // 애니메이션 전환 시점

        // 세 번째 공격 - AtkC
        monster.animator.SetTrigger("AtkC");
        yield return new WaitForSeconds(atkDelayC); // 공격 판정 딜레이
        AttackHit(monster, 180f); // 공격 판정

        // AtkC 애니메이션이 끝날 때까지 대기
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            // "AtkC" 애니메이션이 끝난 후 다른 상태로 전환되었을 때, "AtkC"가 끝났다고 판단
            return !stateInfo.IsName("AtkC") || stateInfo.normalizedTime >= 1f;
        });

        // 공격 종료 후 상태 전환
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
    }

    private void AttackHit(BossMonster monster, float angleThreshold)
    {
        Vector3 atkDir = monster.transform.forward;
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, monster.atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                float angle = Vector3.Angle(atkDir, dirToTarget);

                if (angle <= angleThreshold)
                {
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(monster.atkDamage);
                    Debug.Log("공격 성공");
                }
                else
                {
                    Debug.Log("공격 실패 - 범위 밖");
                }
            }
        }
    }
}
