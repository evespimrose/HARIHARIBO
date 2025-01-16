using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillA : BaseState<BossMonster>
{
    public BossMonsterSkillA(StateHandler<BossMonster> handler) : base(handler) { }
    //��ų1 3���� ����
    // ���� ���� ������
    public float startDelay = 0f;
    public float atkADelay = 0.38f;
    public float atkBDelay = 0.38f;

    // �ִϸ��̼� ���� �ð�
    public float atkBStartTime = 1.04f;
    public float atkADuration = 1.34f;
    public float atkBDuration = 1.34f;

    // End�� ���� �� �ð�
    public float endTime = 0.2f;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillA ����");
        monster.isAtk = true;
        monster.StartSkillCoroutine(SkillACoroutine(monster));
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("���� ����");
        monster.isAtk = false;
    }

    private IEnumerator SkillACoroutine(BossMonster monster)
    {
        // ��������
        yield return new WaitForSeconds(startDelay);
        monster.TargetLook(monster.target.position);

        monster.animator.SetTrigger("SkillA1");
        yield return new WaitForSeconds(atkADelay); 
        AttackHit(monster, 1); 
        yield return new WaitForSeconds((startDelay + atkADelay) - atkBStartTime); // �ִϸ��̼� ��ȯ ����

        monster.animator.SetTrigger("SkillA2");
        yield return new WaitForSeconds(atkBDelay);

        AttackHit(monster, 2); 
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            // "SkillA2" �ִϸ��̼��� ���� ������ ���
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
                    Debug.Log($"{actionNumber} ���� ����");
                }
                else
                {
                    Debug.Log($"{actionNumber} ���� ���� - ���� ��");
                }
            }
        }
    }
}
