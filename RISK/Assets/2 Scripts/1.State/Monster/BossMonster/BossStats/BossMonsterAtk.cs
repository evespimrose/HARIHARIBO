using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterAtk : BaseState<BossMonster>
{
    public BossMonsterAtk(StateHandler<BossMonster> handler) : base(handler) { }

    public enum CurAction
    {
        Strat,
        AtkA,
        AtkB,
        AtkC,
        End
    }
    private CurAction curAction = CurAction.Strat;

    public float atkRange = 20f;

    public float attackDelay = 0.5f; // ���� ���� �� ������
    public float atkADuration = 2f;  // AtkA �ִϸ��̼� ���� �ð�
    public float atkBDuration = 2f;  // AtkB �ִϸ��̼� ���� �ð�
    public float atkCDuration = 2f;  // AtkC �ִϸ��̼� ���� �ð�

    private float phaseStartTime;
    private bool isActionEnd = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("���� ����");
        curAction = CurAction.AtkA;
        phaseStartTime = Time.time + 1f; // 1�� ��� �� AtkA �ִϸ��̼� ����
        isActionEnd = false;
    }

    public override void Update(BossMonster monster)
    {
        if (curAction == CurAction.End) return;

        float elapsedTime = Time.time - phaseStartTime;

        switch (curAction)
        {
            case CurAction.AtkA:
                if (!isActionEnd && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 1);
                    isActionEnd = true;
                }

                if (elapsedTime >= attackDelay && isActionEnd)
                {
                    AttackHit(monster, 105f); // AtkA ���� ����
                }

                if (elapsedTime >= atkADuration)
                {
                    ChangeAction(monster, CurAction.AtkB);
                }
                break;

            case CurAction.AtkB:
                if (!isActionEnd && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 2);
                    isActionEnd = true;
                }

                if (elapsedTime >= attackDelay && isActionEnd)
                {
                    AttackHit(monster, 45f); // AtkB ���� ����
                }

                if (elapsedTime >= atkBDuration)
                {
                    ChangeAction(monster, CurAction.AtkC);
                }
                break;

            case CurAction.AtkC:
                if (!isActionEnd && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 3);
                    isActionEnd = true;
                }

                if (elapsedTime >= attackDelay && isActionEnd)
                {
                    AttackHit(monster, 180f); // AtkC ���� ����
                }

                if (elapsedTime >= atkCDuration)
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
                monster.animator.SetTrigger("AtkA");
                break;
            case 2:
                monster.animator.SetTrigger("AtkB");
                break;
            case 3:
                monster.animator.SetTrigger("AtkC");
                break;
        }
        Debug.Log($"{curAction} �ִϸ��̼� ���");
    }

    private void AttackHit(BossMonster monster, float angleThreshold)
    {
        Vector3 atkDir = monster.transform.forward;
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, atkRange);

        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                float angle = Vector3.Angle(atkDir, dirToTarget);

                if (angle <= angleThreshold)
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