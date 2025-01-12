using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillD : BaseState<BossMonster>
{
    public BossMonsterSkillD(StateHandler<BossMonster> handler) : base(handler) { }
    //��ų4 ���Ÿ� ���� 1
    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillC ����");
        //�ִϸ��̼� ����
        SkillDAtk(monster);//�ִϸ��̼ǰ� ���ý��� ��¦���ٰŸ� ������Ʈ�����οŰܼ� curTime üũ�ؼ� ����
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {

    }

    public void SkillDAtk(BossMonster monster)
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
