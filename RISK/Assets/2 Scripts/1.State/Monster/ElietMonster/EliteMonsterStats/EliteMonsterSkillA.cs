using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterSkillA : BaseState<EliteMonster>
{
    public EliteMonsterSkillA(StateHandler<EliteMonster> handler) : base(handler) { }

    //스킬1 3연속 공격

    public float aDamage;
    public float bDamage;

    // 공격 판정 딜레이
    public float startDelay = 0f;
    public float atkADelay = 0.38f;

    public float aHitTime = 0.5f;
    public float bHitTime = 1.45f;

    // 애니메이션 지속 시간
    public float atkDuration = 1.35f;

    private Coroutine action;

    public override void Enter(EliteMonster monster)
    {
        aDamage = monster.atkDamage * 1.38f;
        bDamage = monster.atkDamage * 1.38f;
        Debug.Log("SkillA 시작");
        action = monster.StartCoroutine(SkillACoroutine(monster));
    }

    public override void Exit(EliteMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("공격 종료");
        monster.StopCoroutine(action);
    }

    private IEnumerator SkillACoroutine(EliteMonster monster)
    {
        // 선딜레이
        yield return new WaitForSeconds(startDelay);
        monster.TargetLook(monster.target.position);

        monster.animator.SetTrigger("SkillA1");
        yield return new WaitForSeconds(aHitTime);
        AttackHit(monster, 1);

        yield return new WaitForSeconds(bHitTime - aHitTime);

        AttackHit(monster, 2);
        yield return new WaitForSeconds(atkDuration);
        monster.eMHandler.ChangeState(typeof(EliteMonsterIdle));
    }


    private void AttackHit(EliteMonster monster, int actionNumber)
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
            if (col.gameObject.CompareTag("LocalPlayer") || col.gameObject.CompareTag("RemotePlayer"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                float angle = Vector3.Angle(atkDir, dirToTarget);
                if (angle <= 135f)
                {
                    //col.gameObject.GetComponent<ITakedamage>().Takedamage(aDamage);
                    monster.Atk(col.gameObject, aDamage);
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
