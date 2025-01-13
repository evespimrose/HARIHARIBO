using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillC : BaseState<BossMonster>
{
    public BossMonsterSkillC(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkDelay = 0f;//��������
    public float skillCDuration = 1.21f;//�ִϸ��̼� ���� �ð�
    public float skillCAtkTiem = 1f;//�ִϸ��̼� ���� �� SkillCAtk ������� ��ٸ� �ð� (�ʵ尪)
    private float startTime;
    private bool isAction = false;//SkillCAtk�� �� ���� ����ǵ��� ����

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillC ����");
        isAction = false;//��ųC ������ �� ���� ����ǵ��� ����
        startTime = Time.time + atkDelay;//�������̸� ����� ���� �ð�
        //�������̰� 0�� �� ��� �ִϸ��̼� ����
        if (atkDelay <= 0f)
        {
            monster.animator.SetTrigger("SkillC");
        }
    }

    public override void Update(BossMonster monster)
    {
        //�������̰� ���� �� �ִϸ��̼� ����
        if (Time.time >= startTime && !monster.animator.GetCurrentAnimatorStateInfo(0).IsName("SkillC"))
        {
            monster.animator.SetTrigger("SkillC");
        }
        //�ִϸ��̼��� ���۵� �� ���� �ð��� ������ SkillCAtk ����
        if (Time.time - startTime >= skillCAtkTiem && !isAction)
        {
            SkillCAtk(monster);
            isAction = true;
        }
        //�ִϸ��̼��� ���� �� 0.2�� ������ �ΰ� ���� ��ȯ
        if (Time.time - startTime >= skillCDuration + 0.2f)
        {
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillC ����");
    }

    private void SkillCAtk(BossMonster monster)
    {
        GameObject skillCObj = monster.ObjSpwan(monster.skillCPrefab, monster.transform.position);
        //y�� 0 ����
        Vector3 forwardDir = new Vector3(monster.transform.forward.x, 0f, monster.transform.forward.z).normalized;
        //���� ����
        skillCObj.transform.forward = forwardDir;
        skillCObj.transform.position = new Vector3(skillCObj.transform.position.x, skillCObj.transform.position.y + 1f, skillCObj.transform.position.z);
        skillCObj.GetComponent<BossSkillCObject>().Seting(monster.atkDamage);
    }
}
