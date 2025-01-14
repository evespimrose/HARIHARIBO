using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillC : BaseState<BossMonster>
{
    public BossMonsterSkillC(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkDelay = 0f;          // ��������
    public float skillCDuration = 1.21f; // �ִϸ��̼� ���� �ð�
    public float skillCAtkTime = 1f;     // �ִϸ��̼� ���� �� SkillCAtk ������� ��ٸ� �ð�

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillC ����");
        monster.StartSkillCoroutine(SkillCCoroutine(monster));  // �ڷ�ƾ ����
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillC ����");
    }

    private IEnumerator SkillCCoroutine(BossMonster monster)
    {
        // �������� �� �ִϸ��̼� ����
        yield return new WaitForSeconds(atkDelay);
        monster.animator.SetTrigger("SkillC");

        // �ִϸ��̼� ���� �� skillCAtk ������� ���
        yield return new WaitForSeconds(skillCAtkTime);
        SkillCAtk(monster);  // SkillCAtk ����

        // �ִϸ��̼� ���� �� 0.2�� ������ �ΰ� ���� ��ȯ
        yield return new WaitForSeconds(skillCDuration + 0.2f);
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // ���� ��ȯ
        Debug.Log("SkillC ���� �� Idle ���·� ��ȯ");
    }

    private void SkillCAtk(BossMonster monster)
    {
        // ��ų C ���� ��ü ����
        GameObject skillCObj = monster.ObjSpwan(monster.skillCPrefab, monster.transform.position);

        // y�� ����, ���� ����
        Vector3 forwardDir = new Vector3(monster.transform.forward.x, 0f, monster.transform.forward.z).normalized;
        skillCObj.transform.forward = forwardDir;

        // ���� ���� (�⺻ y ��ġ�� 1�� ����)
        skillCObj.transform.position = new Vector3(skillCObj.transform.position.x, skillCObj.transform.position.y + 1f, skillCObj.transform.position.z);

        // ��ų C ��ü�� ������ ����
        skillCObj.GetComponent<BossSkillCObject>().Seting(monster.atkDamage);
    }
}
