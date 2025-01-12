using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillG : BaseState<BossMonster>
{
    public BossMonsterSkillG(StateHandler<BossMonster> handler) : base(handler) { }

    // ��ų7 �뽬 + ����
    public float atkDelay = 1f;
    private float curTime = 0f;
    private float endTime = 0.5f; // �ڷ���Ʈ �� ��� �ð�
    private bool isAction = false; // �ڷ���Ʈ ���θ� ����

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillG ����");
        curTime = 0f;
        isAction = false;
    }

    public override void Update(BossMonster monster)
    {
        curTime += Time.deltaTime;

        if (isAction == false && curTime >= atkDelay)
        {
            // �ڷ���Ʈ ����
            SkillGAtk(monster);
            isAction = true; // �ڷ���Ʈ �Ϸ�
            curTime = 0f; // �ڷ���Ʈ �� ��⸦ ���� �ð� �ʱ�ȭ
        }
        else if (isAction == true && curTime >= endTime)
        {
            // �ڷ���Ʈ �� ���� ���·� ��ȯ
            monster.bMHandler.ChangeState(typeof(BossMonsterSkillB));
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillG ����");
    }

    public void SkillGAtk(BossMonster monster)
    {
        // Ÿ�� ���� ���� ���
        Vector3 TargetDir = (monster.target.position - monster.transform.position).normalized;

        // Ÿ���� ���� ��ġ ���
        Vector3 teleportPos = monster.target.position - TargetDir * 2f; // 2f�� �Ÿ� (�ʿ�� ���� ����)

        // Y���� ���� ���� ���̸� ����
        teleportPos.y = monster.transform.position.y;

        // ���� ��ġ ����
        monster.transform.position = teleportPos;

        Debug.Log($"���Ͱ� Ÿ�� �ڷ� �̵�: {teleportPos}");
    }
}

