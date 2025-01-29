using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillA : BaseState<BossMonster>
{
    public BossMonsterSkillA(StateHandler<BossMonster> handler) : base(handler) { }
    //스킬1 3연속 공격

    public float damageA = 1f;
    public float damageB = 1f;

    // 공격 판정 딜레이
    public float startDelay = 0f;
    public float atkAHitTime = 0.38f;
    public float atkBHitTime = 1.34f;

    //공격후 종료까지 걸리는시간
    public float endTime = 1.5f;

    private Coroutine action;

    public override void Enter(BossMonster monster)
    {
        damageA = monster.atkDamage * 1.52f;
        damageB = monster.atkDamage * 1.52f;
        monster.isAtk = true;
        Debug.Log("SkillA 시작");
        action = monster.StartCoroutine(SkillACoroutine(monster));
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("공격 종료");
        monster.StopCoroutine(action);
        monster.isAtk = false;
    }

    private IEnumerator SkillACoroutine(BossMonster monster)
    {
        // 선딜레이
        yield return new WaitForSeconds(startDelay);
        monster.TargetLook(monster.target.position);

        GameSoundManager.Instance.PlayBossEffectSound(monster.skillASoundClips[0]);
        monster.animator.SetTrigger("SkillA1");
        yield return new WaitForSeconds(atkAHitTime); 
        AttackHit(monster, 1, damageA); 

        yield return new WaitForSeconds(atkBHitTime);
        GameSoundManager.Instance.PlayBossEffectSound(monster.skillASoundClips[1]);

        AttackHit(monster, 2, damageB); 
        yield return new WaitForSeconds(endTime);

        yield return null;
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
    }


    private void AttackHit(BossMonster monster, int actionNumber, float damage)
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
                    //col.gameObject.GetComponent<ITakedamage>().Takedamage(damage);
                    monster.Atk(col.gameObject, damage);
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
