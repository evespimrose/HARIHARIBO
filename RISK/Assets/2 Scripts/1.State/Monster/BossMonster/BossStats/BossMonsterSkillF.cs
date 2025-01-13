using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillF : BaseState<BossMonster>
{
    public BossMonsterSkillF(StateHandler<BossMonster> handler) : base(handler) { }
    //��ų6 �뽬 �а�

    public float atkTime = 1f;
    public float curTime = 0f;

    public override void Enter(BossMonster monster)
    {
        //Ÿ�� ���μ���
        monster.Targeting();
        monster.skillFTargetPos = monster.target.position;
        monster.SkillFReset();
        Debug.Log("SkillF ����");
        //�ִϸ��̼� ����
        curTime = 0f;
        monster.isMoving = true;
    }

    public override void Update(BossMonster monster)
    {
        if (curTime < atkTime && monster.isMoving == false)
        {
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        }
        else if (curTime >= atkTime && monster.isMoving == true)
        {
            monster.isMoving = false;
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillF ����");
        monster.SkillFReset();
    }
}