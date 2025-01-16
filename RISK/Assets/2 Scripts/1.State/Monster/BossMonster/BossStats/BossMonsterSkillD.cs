using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillD : BaseState<BossMonster>
{
    public BossMonsterSkillD(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkDelay = 0f; // ��������
    public float skillDDuration = 2.4f; // �ִϸ��̼� ���� �ð�
    public float skillFAtkTime = 1f; // �ִϸ��̼� ���� �� SkillDAtk ������� ��ٸ� �ð�
    public float additionalWaitTime = 0.2f; // �ִϸ��̼� ���� �� �߰� ��� �ð�

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
        monster.TargetLook(monster.target.position);
        monster.animator.SetTrigger("SkillD");
        Debug.Log("SkillD �ִϸ��̼� ����");

        // �ִϸ��̼� ���� �� ���� �ð��� ������ SkillDAtk ����
        yield return new WaitForSeconds(skillFAtkTime);
        SkillDAtk(monster);

        // �ִϸ��̼��� ���� ������ ���
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsName("SkillD") || stateInfo.normalizedTime >= 1f;
        });

        monster.animator.SetTrigger("Idle");
        yield return new WaitForSeconds(additionalWaitTime);

        // ���� ��ȯ
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // ���� ��ȯ
        Debug.Log("SkillD ���� �� Idle ���·� ��ȯ");
    }

    private void SkillDAtk(BossMonster monster)
    {
        int atkCount = 8;
        float angleStep = 360f / atkCount;
        for (int i = 0; i < atkCount; i++)
        {
            float angle = i * angleStep;
            Vector3 rotDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            rotDir = new Vector3(rotDir.x, 0f, rotDir.z).normalized;
            GameObject skillDObj = monster.ObjSpwan(monster.skillDPrefab, new Vector3(monster.transform.position.x, 1f, monster.transform.position.z));
            skillDObj.transform.rotation = Quaternion.LookRotation(rotDir, Vector3.up);
            skillDObj.GetComponent<BossSkillDObject>().Seting(monster.atkDamage);
            Rigidbody skillRigidbody = skillDObj.GetComponent<Rigidbody>();
            if (skillRigidbody != null)
            {
                skillRigidbody.velocity = rotDir * 10f; 
            }
        }
    }

}
