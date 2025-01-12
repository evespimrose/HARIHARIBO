using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillA : BaseState<BossMonster>
{
    public BossMonsterSkillA(StateHandler<BossMonster> handler) : base(handler) { }
    //��ų1 ��Ÿ
    public enum CurAction
    {
        Strat,
        SkillAtkA,
        SkillAtkB,
        End
    }
    private CurAction curAction = CurAction.Strat;

    public float atkRange = 20f;

    public float attackDelay = 0.5f; // ���� ���� �� ������
    public float atkADuration = 2f;  // AtkA �ִϸ��̼� ���� �ð�
    public float atkBDuration = 2f;  // AtkB �ִϸ��̼� ���� �ð�

    private float phaseStartTime;
    private bool isActionEnd = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("���� ����");
        curAction = CurAction.SkillAtkA;
        phaseStartTime = Time.time + 1f; // 1�� ��� �� AtkA �ִϸ��̼� ����
        isActionEnd = false;
    }

    public override void Update(BossMonster monster)
    {
        if (curAction == CurAction.End) return;

        float elapsedTime = Time.time - phaseStartTime;

        switch (curAction)
        {
            case CurAction.SkillAtkA:
                if (!isActionEnd && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 1);
                    isActionEnd = true;
                }

                if (elapsedTime >= attackDelay && isActionEnd)
                {
                    AttackHit(monster, 1); // SkillAtkA ���� ����
                }

                if (elapsedTime >= atkADuration)
                {
                    ChangeAction(monster, CurAction.SkillAtkB);
                }
                break;

            case CurAction.SkillAtkB:
                if (!isActionEnd && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 2);
                    isActionEnd = true;
                }

                if (elapsedTime >= attackDelay && isActionEnd)
                {
                    AttackHit(monster, 2); // SkillAtkB ���� ����
                }

                if (elapsedTime >= atkBDuration)
                {
                    ChangeAction(monster, CurAction.End);
                }
                break;
            case CurAction.End:
                monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
                break;
        }

    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("���� ����");
    }

    private void AttackAnimationPlay(BossMonster monster, int animationnumber)
    {
        switch (animationnumber)
        {
            case 1:
                monster.animator.SetTrigger("SkillAtkA");
                break;
            case 2:
                monster.animator.SetTrigger("SkillAtkB");
                break;
        }
        Debug.Log($"{curAction} �ִϸ��̼� ���");
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
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, atkRange);

        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                float angle = Vector3.Angle(atkDir, dirToTarget);

                if (angle <= 135f)
                {
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(monster.atkDamage);
                    Debug.Log($"{curAction} ���� ����");
                }
                else
                {
                    Debug.Log($"{curAction} ���� ���� - ���� ��");
                }
            }
        }
    }

    private void ChangeAction(BossMonster monster, CurAction nextPhase)
    {
        curAction = nextPhase;

        if (nextPhase != CurAction.End)
        {
            phaseStartTime = Time.time; // ���� �ܰ�� �̵� �� �ð� �ʱ�ȭ
            isActionEnd = false;   // �ִϸ��̼� ���� �غ�
        }
    }
}