using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillD : BaseState<BossMonster>
{
    public BossMonsterSkillD(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkDelay = 0f; // ��������
    public float skillDDuration = 2.4f; // �ִϸ��̼� ���� �ð�
    public float skillFAtkTime = 1f; // �ִϸ��̼� ���� �� SkillDAtk ������� ��ٸ� �ð�

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillD ����");
        monster.StartSkillCoroutine(SkillDCoroutine(monster)); // �ڷ�ƾ ����
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillD ����");
    }

    private IEnumerator SkillDCoroutine(BossMonster monster)
    {
        // �������� �� �ִϸ��̼� ����
        yield return new WaitForSeconds(atkDelay);
        monster.animator.SetTrigger("SkillD");
        Debug.Log("SkillD �ִϸ��̼� ����");

        // �ִϸ��̼� ���� �� ���� �ð��� ������ SkillDAtk ����
        yield return new WaitForSeconds(skillFAtkTime);
        SkillDAtk(monster);

        // �ִϸ��̼� ���� �ð� �� 0.2�� ������ �ΰ� ���� ��ȯ
        yield return new WaitForSeconds(skillDDuration + 0.2f);
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // ���� ��ȯ
        Debug.Log("SkillD ���� �� Idle ���·� ��ȯ");
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

            // ��ų ��ü ����
            GameObject skillDObj = monster.ObjSpwan(monster.skillDPrefab, monster.transform.position);
            // ��ȯ�� ������Ʈ�� ������ ȸ�� ������ ����Ű���� ����
            skillDObj.transform.rotation = Quaternion.LookRotation(rotDir, Vector3.up);
            skillDObj.GetComponent<BossSkillDObject>().Seting(monster.atkDamage);
        }
    }
}
