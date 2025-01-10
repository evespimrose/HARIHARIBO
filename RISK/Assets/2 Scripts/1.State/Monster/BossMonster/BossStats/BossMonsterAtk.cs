using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterAtk : BaseState<BossMonster>
{
    public BossMonsterAtk(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkRange = 20f;

    public float atkADuration = 3f;
    public float atkADamage = 1f;
    private bool isAtkA = false;

    public float atkBDuration = 3f;
    public float atkBDamage = 1f;
    private bool isAtkB = false;

    public float atkCDuration = 3f;
    public float atkCDamage = 1f;
    private bool isAtkC = false;

    public float outDuration = 3f;
    private float curTime;
    private bool isAction = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("���� ����");
        curTime = 0f;
        isAtkA = false;
        isAtkB = false;
        isAtkC = false;
    }

    public override void Update(BossMonster monster)
    {
        if (isAction == false)
        {
            if (curTime > atkADuration && !isAtkA && !isAtkB && !isAtkC)
            {
                AtkA(monster);
            }
            else if (curTime > atkBDuration && isAtkA && !isAtkB && !isAtkC)
            {
                AtkB(monster);
            }
            else if (curTime > atkCDuration && isAtkA && isAtkB && !isAtkC)
            {
                AtkC(monster);
            }
            else if (curTime > outDuration && isAtkA && isAtkB && isAtkC)
            {
                //��������
                monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
            }
            curTime += Time.deltaTime;
        }
    }

    public override void Exit(BossMonster monster)
    {

    }

    public void AtkA(BossMonster monster)
    {
        isAction = true;
        //�ִϸ��̼� ���
        monster.animator.SetTrigger("Atk");
        Debug.Log("AtkA����");
        isAtkA = true;
        Vector3 atkDir = monster.transform.forward;
        //monster.transform.position = ������������ �߽�
        //monster.atkRange = ���������� ����(����)
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                //����������� �ݿ��������� �ִ��� Ȯ��
                float angle = Vector3.Angle(atkDir, dirToTarget);
                if (angle <= 105f)
                {
                    col.gameObject.GetComponent<ITakedamage>().Takedamage(monster.atkDamage);
                }
                else
                {
                    Debug.Log("�������� ����");
                }
            }
        }
        curTime = 0f;
        isAction = false;
    }

    public void AtkB(BossMonster monster)
    {
        isAction = true;
        //�ִϸ��̼� ���
        monster.animator.SetTrigger("Atk");
        Debug.Log("AtkB����");
        isAtkB = true;
        Vector3 atkDir = monster.transform.forward;
        //monster.transform.position = ������������ �߽�
        //monster.atkRange = ���������� ����(����)
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                //����������� �ݿ��������� �ִ��� Ȯ��
                float angle = Vector3.Angle(atkDir, dirToTarget);
                if (angle <= 45f)
                {
                    col.gameObject.GetComponent<ITakedamage>().Takedamage(monster.atkDamage);
                }
                else
                {
                    Debug.Log("�������� ����");
                }
            }
        }
        curTime = 0f;
        isAction = false;
    }

    public void AtkC(BossMonster monster)
    {
        isAction = true;
        //�ִϸ��̼� ���
        monster.animator.SetTrigger("Atk");
        Debug.Log("AtkC����");
        isAtkC = true;
        //monster.transform.position = ������������ �߽�
        //monster.atkRange = ���������� ����(����)
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - monster.transform.position).normalized;
                //����������� �ݿ��������� �ִ��� Ȯ��
                col.gameObject.GetComponent<ITakedamage>().Takedamage(monster.atkDamage);
            }
        }
        curTime = 0f;
        isAction = false;
    }
}
