using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillE : BaseState<BossMonster>
{
    public BossMonsterSkillE(StateHandler<BossMonster> handler) : base(handler) { }
    //��ų5 ���Ÿ� ���� 2
    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillE ����");
        //�ִϸ��̼� ����
    }

    public override void Update(BossMonster monster)
    {

    }

    public override void Exit(BossMonster monster)
    {

    }

    public void SkillEAtk(BossMonster monster)
    {

    }
}