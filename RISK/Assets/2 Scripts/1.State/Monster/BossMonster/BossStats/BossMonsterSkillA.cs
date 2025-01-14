using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillA : BaseState<BossMonster>
{
    public BossMonsterSkillA(StateHandler<BossMonster> handler) : base(handler) { }

    // ���� ���� ������
    public float startDelay = 0f;
    public float atkADelay = 0.38f;
    public float atkBDelay = 0.38f;

    // �ִϸ��̼� ���� �ð�
    public float atkADuration = 1.34f; // �ִϸ��̼� �ð� 1.34��
    public float atkBDuration = 1.34f; // �ִϸ��̼� �ð� 1.34��

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

        // ù ��° ���� - SkillAtkA
        monster.animator.SetTrigger("SkillA1");
        yield return new WaitForSeconds(atkADelay); // ���� ���� ������
        AttackHit(monster, 1); // ���� ����

        // 1.2�� �� SkillAtkB�� ��ȯ
        yield return new WaitForSeconds(0.7f); // �ִϸ��̼� ��ȯ ����

        // �� ��° ���� - SkillAtkB
        monster.animator.SetTrigger("SkillA2");
        yield return new WaitForSeconds(atkBDelay); // ���� ���� ������
        AttackHit(monster, 2); // ���� ����

        // SkillAtkB �ִϸ��̼��� ���� �� End�� ��ȯ
        yield return new WaitForSeconds(atkBDuration + endTime); // �ִϸ��̼� ��ȯ ����

        // ���� ���� �� ���� ��ȯ
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
