using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillB : BaseState<BossMonster>
{
    public BossMonsterSkillB(StateHandler<BossMonster> handler) : base(handler) { }
    //��ų2 ������ 1
    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillB ����");
        //�ִϸ��̼� ����
        SkellBAtk(monster);//�ִϸ��̼ǰ� ���ý��� ��¦���ٰŸ� ������Ʈ�����οŰܼ� curTime üũ�ؼ� ����
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {

    }

    public void SkellBAtk(BossMonster monster)
    {
        GameObject skillBObj = monster.ObjSpwan(monster.skillBPrefab, monster.target.position);
        skillBObj.GetComponent<BossSkillBObject>().Seting(monster.target.position, monster.atkDamage);
    }
}
