using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillC : BaseState<BossMonster>
{
    public BossMonsterSkillC(StateHandler<BossMonster> handler) : base(handler) { }
    //��ų3 ������ 2
    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillC ����");
        //�ִϸ��̼� ����
        SkillCAtk(monster);//�ִϸ��̼ǰ� ���ý��� ��¦���ٰŸ� ������Ʈ�����οŰܼ� curTime üũ�ؼ� ����
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {

    }

    public void SkillCAtk(BossMonster monster)
    {
        GameObject skillBObj = monster.ObjSpwan(monster.skillCPrefab, monster.transform.position);
        // ���� ������ forward�� ���������, y�� ������ 0���� �����Ͽ� ���Ʒ� ������ ����
        Vector3 forwardDir = new Vector3(monster.transform.forward.x, 0f, monster.transform.forward.z).normalized;
        // ���� ����
        skillBObj.transform.forward = forwardDir;
        skillBObj.transform.position = new Vector3(skillBObj.transform.position.x, skillBObj.transform.position.y + 1f, skillBObj.transform.position.z);
        skillBObj.GetComponent<BossSkillCObject>().Seting(monster.atkDamage);
    }
}
