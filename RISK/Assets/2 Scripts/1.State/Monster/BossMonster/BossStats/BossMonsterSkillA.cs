using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillA : BaseState<BossMonster>
{
    public BossMonsterSkillA(StateHandler<BossMonster> handler) : base(handler) { }
    //스킬1 3연속 공격
    // 공격 판정 딜레이
    public float startDelay = 0f;
    public float atkADelay = 0.38f;
    public float atkBDelay = 0.38f;

    // 애니메이션 지속 시간
    public float atkBStartTime = 1.04f;
    public float atkADuration = 1.34f;
    public float atkBDuration = 1.34f;

    // End로 일찍 들어갈 시간
    public float endTime = 0.2f;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillA 시작");
        monster.isAtk = true;
        monster.StartSkillCoroutine(SkillACoroutine(monster));
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("공격 종료");
        monster.isAtk = false;
    }

    private IEnumerator SkillACoroutine(BossMonster monster)
    {
        // 선딜레이
        yield return new WaitForSeconds(startDelay);
        monster.TargetLook(monster.target.position);

        monster.animator.SetTrigger("SkillA1");
        yield return new WaitForSeconds(atkADelay); 
        AttackHit(monster, 1); 
        yield return new WaitForSeconds((startDelay + atkADelay) - atkBStartTime); // 애니메이션 전환 시점

        monster.animator.SetTrigger("SkillA2");
        yield return new WaitForSeconds(atkBDelay);

        AttackHit(monster, 2); 
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            // "SkillA2" 애니메이션이 끝날 때까지 대기
            return !stateInfo.IsName("SkillA2") || stateInfo.normalizedTime >= 1f;
        });
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
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
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, monster.atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                float angle = Vector3.Angle(atkDir, dirToTarget);
                if (angle <= 135f)
                {
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(monster.atkDamage);
                    Debug.Log($"{actionNumber} 공격 성공");
                }
                else
                {
                    Debug.Log($"{actionNumber} 공격 실패 - 범위 밖");
                }
            }
        }
    }
}
