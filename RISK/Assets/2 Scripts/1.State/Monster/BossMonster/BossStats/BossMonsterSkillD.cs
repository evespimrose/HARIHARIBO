using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillD : BaseState<BossMonster>
{
    public BossMonsterSkillD(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkDelay = 0f; // ��������
    public float skillDDuration = 2.4f; // �ִϸ��̼� ���� �ð�
    public float skillFAtkTiem = 1f; // �ִϸ��̼� ���� �� SkillDAtk ������� ��ٸ� �ð� (�ʵ尪)
    private float startTime;
    private bool isAction = false; // SkillDAtk�� �� ���� ����ǵ��� ����

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillD ����");

        // �������̰� ���� ��� �������� �� �ִϸ��̼� ����
        startTime = Time.time + atkDelay; // �������̸� ����� ���� �ð�

        // �������̰� 0�� �� ��� �ִϸ��̼� ����
        if (atkDelay <= 0f)
        {
            // �ִϸ��̼��� �̹� ����ǰ� ���� ���� ��쿡�� Ʈ���� ����
            if (!monster.animator.GetCurrentAnimatorStateInfo(0).IsName("SkillD"))
            {
                monster.animator.SetTrigger("SkillD");
                Debug.Log("SkillD �ִϸ��̼� ����");
            }
        }
    }

    public override void Update(BossMonster monster)
    {
        // �������̰� ���� �� �ִϸ��̼� ���� (�� ���� ����ǵ���)
        if (Time.time >= startTime && !monster.animator.GetCurrentAnimatorStateInfo(0).IsName("SkillD"))
        {
            monster.animator.SetTrigger("SkillD"); // �ִϸ��̼� ����
            Debug.Log("SkillD �ִϸ��̼� ����");
        }

        // �ִϸ��̼��� ���۵� �� ���� �ð��� ������ SkillDAtk ����
        if (Time.time - startTime >= skillFAtkTiem && !isAction)
        {
            SkillDAtk(monster); // SkillDAtk ����
            isAction = true; // SkillDAtk�� �� ���� ����ǵ��� �÷��� ����
        }

        // �ִϸ��̼��� ���� �� 0.2�� ������ �ΰ� ���� ��ȯ
        if (Time.time - startTime >= skillDDuration + 0.2f)
        {
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // ���� ��ȯ
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillD ����");
    }

    private void SkillDAtk(BossMonster monster)
    {
        // 360������ 8�������� ������
        int atkCount = 8;
        float angleStep = 360f / atkCount;

        for (int i = 0; i < atkCount; i++)
        {
            // �� ������ ȸ���� ���
            float angle = i * angleStep;

            // ���� ��ǥ�迡�� ȸ�������� ���ϰ� ȸ�� ���� ���� ���
            Vector3 rotDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            // Y�� ���� �����Ͽ� ���η� ����
            rotDir = new Vector3(rotDir.x, 0f, rotDir.z).normalized;

            // ������Ʈ ���� �� ��ġ ����
            GameObject skillDObj = monster.ObjSpwan(monster.skillDPrefab, monster.transform.position);

            // ���� ���� (��ȯ�� ������Ʈ�� ������ ������ ����Ű�� ����)
            skillDObj.transform.rotation = Quaternion.LookRotation(rotDir, Vector3.up);

            // ������ �� ��Ÿ ���� �ʱ�ȭ
            skillDObj.GetComponent<BossSkillDObject>().Seting(monster.atkDamage);
        }
    }
}
