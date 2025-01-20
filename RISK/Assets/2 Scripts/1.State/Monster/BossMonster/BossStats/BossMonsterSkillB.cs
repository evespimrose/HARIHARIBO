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
    public float damage = 1f;
    public float skillBDuration = 2.09f; 
    public float atkDelay = 1f;//선딜레이
    public float atkTime = 1f;//물체 떨어지는시간
    public float hitTime = 1.2f;//타격타이밍
    public float endTime = 0.5f;//타격후 딜레이시간
    public float additionalWaitTime = 1f;

    public float atkRange = 10f;
    public float rangeAtkMinRange = 10f;
    public float rangeAtkMaxRange = 15f;

    private AtkType atkType;

    private bool Action = false;

    public override void Enter(BossMonster monster)
    {
        damage = monster.atkDamage * 3f;
        Action = true;
        monster.isAtk = true;
        Debug.Log("SkillB 진입");
        atkType = (AtkType)Random.Range(0, 2); 
        monster.StartCoroutine(SkillBAtk(monster));
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
        Debug.Log("SkillB 종료");
        DeactivateSkillBFieldParticle(monster);
    }

    private IEnumerator SkillBAtk(BossMonster monster)
    {
        yield return new WaitForSeconds(atkDelay);
        monster.TargetLook(monster.target.position);

        ActivateSkillBParticle(monster);
        //선딜레이
        yield return new WaitForSeconds(atkDelay);

        monster.animator.SetTrigger("SkillB");
        yield return new WaitForSeconds(atkTime);

        // skillBParticle 비활성화
        DeactivateSkillBParticle(monster);
        ActivateSkillBFieldParticle(monster);
        yield return new WaitForSeconds(hitTime);

        Atk(monster);
        DeactivateSkillBFieldParticle(monster);
        yield return new WaitForSeconds(endTime);

        Action = false;
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
                col.gameObject.GetComponent<ITakedamage>()?.Takedamage(damage);
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
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(damage);
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
