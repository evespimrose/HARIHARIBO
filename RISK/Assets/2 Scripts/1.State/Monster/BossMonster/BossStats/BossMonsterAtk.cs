using System.Collections;
using System.Threading;
using UnityEngine;

public class BossMonsterAtk : BaseState<BossMonster>
{
    public BossMonsterAtk(StateHandler<BossMonster> handler) : base(handler) { }

    // ���� ���� ������
    public float atkDelayA = 0.17f;
    public float atkDelayB = 0.43f;
    public float atkDelayC = 0.18f;

    // �ִϸ��̼� ���� �ð�
    public float atkADuration = 1.4f;
    public float atkBDuration = 1.6f;
    public float atkCDuration = 1.8f;

    // �ִϸ��̼� ��ȯ ������ ������ �ð�
    public float nextBTime = 0.45f; // AtkA -> AtkB�� �Ѿ�� �ð�
    public float nextCTime = 1f;    // AtkB -> AtkC�� �Ѿ�� �ð�
    public float nextEndTime = 1.3f; // AtkC -> End�� �Ѿ�� �ð�

    // ������Ʈ ���� �� ��������
    public float startDelay = 0.5f;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("Atk ����");
        monster.isAtk = true;

        // �������� �� �ڷ�ƾ ����
        monster.StartSkillCoroutine(AttackCoroutine(monster));
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("Atk ����");
        monster.isAtk = false;
    }

    private IEnumerator AttackCoroutine(BossMonster monster)
    {
        // ��������
        yield return new WaitForSeconds(startDelay);
        monster.TargetLook(monster.target.position);

        // ù ��° ���� - AtkA
        monster.animator.SetTrigger("AtkA");
        yield return new WaitForSeconds(atkDelayA); // ���� ���� ������
        AttackHit(monster, 105f); // ���� ����

        // AtkA �ִϸ��̼ǿ��� AtkB�� ��ȯ
        yield return new WaitForSeconds(nextBTime); // �ִϸ��̼� ��ȯ ����

        // �� ��° ���� - AtkB
        monster.animator.SetTrigger("AtkB");
        yield return new WaitForSeconds(atkDelayB); // ���� ���� ������
        AttackHit(monster, 45f); // ���� ����

        // AtkB �ִϸ��̼ǿ��� AtkC�� ��ȯ
        yield return new WaitForSeconds(nextCTime); // �ִϸ��̼� ��ȯ ����

        // �� ��° ���� - AtkC
        monster.animator.SetTrigger("AtkC");
        yield return new WaitForSeconds(atkDelayC); // ���� ���� ������
        AttackHit(monster, 180f); // ���� ����

        // AtkC �ִϸ��̼��� ���� ������ ���
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            // "AtkC" �ִϸ��̼��� ���� �� �ٸ� ���·� ��ȯ�Ǿ��� ��, "AtkC"�� �����ٰ� �Ǵ�
            return !stateInfo.IsName("AtkC") || stateInfo.normalizedTime >= 1f;
        });

        // ���� ���� �� ���� ��ȯ
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
                    Debug.Log("���� ����");
                }
                else
                {
                    Debug.Log("���� ���� - ���� ��");
                }
            }
        }
    }
}
