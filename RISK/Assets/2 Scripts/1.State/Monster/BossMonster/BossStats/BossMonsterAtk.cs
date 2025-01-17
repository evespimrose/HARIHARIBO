using System.Collections;
using System.Threading;
using UnityEngine;

public class BossMonsterAtk : BaseState<BossMonster>
{
    public BossMonsterAtk(StateHandler<BossMonster> handler) : base(handler) { }

    public float damageA = 1f;
    public float damageB = 1f;
    public float damageC = 1f;

    // 공격 판정 딜레이
    public float atkAHitTime = 0.25f;//들어와서 이시간후 1타 히트
    public float atkBHitTime = 0.95f;//1타이후 이시간이후 2타 히트
    public float atkCHitTime = 1.2f;//2타이후 이시간후 3타히트

    // 애니메이션 지속 시간
    public float endTime = 1.5f; //공격후 나가는시간

    // 스테이트 들어온 뒤 선딜레이
    public float startDelay = 0.5f;

    private bool Action = false;

    public override void Enter(BossMonster monster)
    {
        damageA = monster.atkDamage * 1f;
        damageB = monster.atkDamage * 1.1f;
        damageC = monster.atkDamage * 1.2f;

        Action = true;
        monster.isAtk = true;
        Debug.Log("Atk 시작");

        // 선딜레이 후 코루틴 시작
        monster.StartCoroutine(AttackCoroutine(monster));
    }

    public override void Update(BossMonster monster)
    {
        if (Action == false)
        {
            monster.isAtk = false;
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("Atk 종료");
    }

    private IEnumerator AttackCoroutine(BossMonster monster)
    {
        // 선딜레이
        yield return new WaitForSeconds(startDelay);
        monster.TargetLook(monster.target.position);

        // 첫 번째 공격 - AtkA
        monster.animator.SetTrigger("AtkA");
        yield return new WaitForSeconds(atkAHitTime); // 공격 판정 딜레이
        AttackHit(monster, 105f, damageA); // 공격 판정

        // 두 번째 공격 - AtkB
        yield return new WaitForSeconds(atkBHitTime); // 공격 판정 딜레이
        AttackHit(monster, 45f, damageB); // 공격 판정

        // 세 번째 공격 - AtkC
        yield return new WaitForSeconds(atkCHitTime); // 공격 판정 딜레이
        AttackHit(monster, 180f, damageC); // 공격 판정

        //전체애니메이션 종료
        yield return new WaitForSeconds(endTime);

        Action = false;
        // 공격 종료 후 상태 전환
    }

    private void AttackHit(BossMonster monster, float angleThreshold, float damage)
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
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(damage);
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
