using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillB : BaseState<BossMonster>
{
    public BossMonsterSkillB(StateHandler<BossMonster> handler) : base(handler) { }
    // 스킬2 범위기 1
    public enum AtkType
    {
        Melee, 
        Range  
    }
    public float skillBDuration = 2.09f; 
    public float atkDelay = 1f;//선딜레이
    public float atkTime = 1f;//번개 시작
    public float additionalWaitTime = 1f;

    public float atkRange = 15f;         
    public float rangeAtkMinRange = 15f; 
    public float rangeAtkMaxRange = 25f; 

    private AtkType atkType; 

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillB 진입");
        monster.isAtk = true;
        atkType = (AtkType)Random.Range(0, 2); 
        monster.StartSkillCoroutine(SkillBAtk(monster));
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillB 종료");
        monster.isAtk = false;
        DeactivateSkillBFieldParticle(monster);
    }

    private IEnumerator SkillBAtk(BossMonster monster)
    {
        yield return new WaitForSeconds(atkDelay);

        ActivateSkillBParticle(monster);
        //선딜레이
        yield return new WaitForSeconds(atkDelay);

        monster.animator.SetTrigger("SkillB");
        yield return new WaitForSeconds(atkTime);

        // skillBParticle 비활성화
        DeactivateSkillBParticle(monster);
        ActivateSkillBFieldParticle(monster);
        yield return new WaitForSeconds(atkTime);

        Atk(monster);
        DeactivateSkillBFieldParticle(monster);
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsName("SkillB") || stateInfo.normalizedTime >= 1f;
        });
        // 애니메이션이 끝난 후 추가 대기 시간
        yield return new WaitForSeconds(additionalWaitTime);

        monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        Debug.Log("SkillB 종료 후 Idle 상태로 전환");
    }

    public void Atk(BossMonster monster)
    {
        switch (atkType)
        {
            case AtkType.Melee:
                MeleeAtk(monster);
                break;
            case AtkType.Range:
                RangeAtk(monster);
                break;
        }
    }

    public void MeleeAtk(BossMonster monster)
    {
        Vector3 atkCenter = monster.transform.position; 
        Collider[] cols = Physics.OverlapSphere(atkCenter, atkRange); 
        Debug.Log("근접 공격 진입");
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<ITakedamage>()?.Takedamage(monster.atkDamage);
                Debug.Log("근접 공격 성공");
            }
        }
    }

    public void RangeAtk(BossMonster monster)
    {
        Vector3 atkCenter = monster.transform.position;
        Collider[] cols = Physics.OverlapSphere(atkCenter, rangeAtkMaxRange);
        Debug.Log("원거리 공격 진입");
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - atkCenter).normalized;
                float dirTarget = Vector3.Distance(atkCenter, col.transform.position);
                if (dirTarget >= rangeAtkMinRange && dirTarget <= rangeAtkMaxRange) 
                {
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(monster.atkDamage);
                    Debug.Log("원거리 공격 성공");
                }
            }
        }
    }

    // skillBParticle 활성화
    private void ActivateSkillBParticle(BossMonster monster)
    {
        if (atkType == AtkType.Melee)
        {
            monster.skillBParticle[0].SetActive(true);
        }
        else if (atkType == AtkType.Range)
        {
            monster.skillBParticle[1].SetActive(true);
        }
    }

    private void DeactivateSkillBParticle(BossMonster monster)
    {
        if (atkType == AtkType.Melee)
        {
            monster.skillBParticle[0].SetActive(false);
        }
        else if (atkType == AtkType.Range)
        {
            monster.skillBParticle[1].SetActive(false);
        }
    }

    private void ActivateSkillBFieldParticle(BossMonster monster)
    {
        monster.skillBFieldParticle[atkType == AtkType.Melee ? 0 : 1].SetActive(true);
    }

    private void DeactivateSkillBFieldParticle(BossMonster monster)
    {
        monster.skillBFieldParticle[atkType == AtkType.Melee ? 0 : 1].SetActive(false);
    }
}
