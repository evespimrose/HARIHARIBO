using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillA : BaseState<BossMonster>
{
    public BossMonsterSkillA(StateHandler<BossMonster> handler) : base(handler) { }

    public enum CurAction
    {
        Start,
        SkillAtkA,
        SkillAtkB,
        End
    }
    private CurAction curAction = CurAction.Start;
    public float atkRange = 20f;
    //���� ���� ������
    public float startDelay = 0f;
    public float atkADelay = 0.38f;
    public float atkBDelay = 0.38f;
    //�ִϸ��̼� ���� �ð�
    public float atkADuration = 1.34f; // �ִϸ��̼� �ð� 1.34��
    public float atkBDuration = 1.34f; // �ִϸ��̼� �ð� 1.34��
    //End�� ���� ���ð�
    public float endTime = 0.2f;
    private float startTime;
    private bool isSkillAHit = false;
    private bool isSkillBHit = false;
    private bool isAction = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("���� ����");
        curAction = CurAction.SkillAtkA;
        startTime = Time.time + startDelay;
        isSkillAHit = false;
        isSkillBHit = false;
        isAction = false;
    }

    public override void Update(BossMonster monster)
    {
        float elapsedTime = Time.time - startTime;
        switch (curAction)
        {
            case CurAction.SkillAtkA:
                if (!isAction && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 1);//SkillAtkA �ִϸ��̼� ����
                    isAction = true;
                }
                if (elapsedTime >= atkADelay && !isSkillAHit)
                {
                    AttackHit(monster, 1);//SkillAtkA ���� ����
                    isSkillAHit = true;
                }
                // 1.2�� �� SkillAtkB�� ��ȯ
                if (elapsedTime >= 1.2f) 
                {
                    ChangeAction(monster, CurAction.SkillAtkB);//SkillAtkA �ִϸ��̼��� ������ ���� SkillAtkB�� ��ȯ
                }
                break;
            case CurAction.SkillAtkB:
                if (!isAction && elapsedTime >= 0f)
                {
                    AttackAnimationPlay(monster, 2);//SkillAtkB �ִϸ��̼� ����
                    isAction = true;
                }
                if (elapsedTime >= atkBDelay && !isSkillBHit)
                {
                    AttackHit(monster, 2);//SkillAtkB ���� ����
                    isSkillBHit = true;
                }
                // endTime �� Idle�� ��ȯ
                if (elapsedTime >= atkBDuration + endTime)
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

    private void AttackAnimationPlay(BossMonster monster, int animationNumber)
    {
        switch (animationNumber)
        {
            case 1:
                monster.animator.SetTrigger("SkillA1");
                break;
            case 2:
                monster.animator.SetTrigger("SkillA2");
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
            startTime = Time.time; // ���� �ܰ�� �̵� �� �ð� �ʱ�ȭ
            isAction = false;   // �ִϸ��̼� ���� �غ�
        }
    }
}
