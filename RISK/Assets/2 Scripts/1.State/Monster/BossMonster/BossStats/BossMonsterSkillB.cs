using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillB : BaseState<BossMonster>
{
    public BossMonsterSkillB(StateHandler<BossMonster> handler) : base(handler) { }
    // ��ų2 ������ 1
    public enum AtkType
    {
        Melee, 
        Range  
    }
    public float skillBDuration = 2.09f; 
    public float atkDelay = 1f;//��������
    public float atkTime = 1f;//���� ����
    public float additionalWaitTime = 1f;

    public float atkRange = 15f;         
    public float rangeAtkMinRange = 15f; 
    public float rangeAtkMaxRange = 25f; 

    private AtkType atkType; 

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillB ����");
        monster.isAtk = true;
        atkType = (AtkType)Random.Range(0, 2); 
        monster.StartSkillCoroutine(SkillBAtk(monster));
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillB ����");
        monster.isAtk = false;
        DeactivateSkillBFieldParticle(monster);
    }

    private IEnumerator SkillBAtk(BossMonster monster)
    {
        yield return new WaitForSeconds(atkDelay);

        ActivateSkillBParticle(monster);
        //��������
        yield return new WaitForSeconds(atkDelay);

        monster.animator.SetTrigger("SkillB");
        yield return new WaitForSeconds(atkTime);

        // skillBParticle ��Ȱ��ȭ
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
        // �ִϸ��̼��� ���� �� �߰� ��� �ð�
        yield return new WaitForSeconds(additionalWaitTime);

        monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        Debug.Log("SkillB ���� �� Idle ���·� ��ȯ");
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
        Debug.Log("���� ���� ����");
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<ITakedamage>()?.Takedamage(monster.atkDamage);
                Debug.Log("���� ���� ����");
            }
        }
    }

    public void RangeAtk(BossMonster monster)
    {
        Vector3 atkCenter = monster.transform.position;
        Collider[] cols = Physics.OverlapSphere(atkCenter, rangeAtkMaxRange);
        Debug.Log("���Ÿ� ���� ����");
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - atkCenter).normalized;
                float dirTarget = Vector3.Distance(atkCenter, col.transform.position);
                if (dirTarget >= rangeAtkMinRange && dirTarget <= rangeAtkMaxRange) 
                {
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(monster.atkDamage);
                    Debug.Log("���Ÿ� ���� ����");
                }
            }
        }
    }

    // skillBParticle Ȱ��ȭ
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
