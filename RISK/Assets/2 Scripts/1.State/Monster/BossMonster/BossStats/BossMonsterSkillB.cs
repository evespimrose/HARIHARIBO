using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillB : BaseState<BossMonster>
{
    public BossMonsterSkillB(StateHandler<BossMonster> handler) : base(handler) { }

    //��ų2 ������ 1
    public float skillBDuration = 2.09f;
    public float atkDelay = 1f;//��������

    private float startTime;
    private bool isAction = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillB ����");
        // �ִϸ��̼� ����
        monster.animator.SetTrigger("SkillB");
        // �������� �� ���� ����
        startTime = Time.time + atkDelay;
        isAction = false;
    }

    public override void Update(BossMonster monster)
    {
        float elapsedTime = Time.time - startTime;
        // �������� ���� ���� ����
        if (!isAction && elapsedTime >= 0f)
        {
            SkellBAtk(monster);  
            isAction = true;     
        }
        // ��ų ���� �ð� �� ���� ����
        if (elapsedTime >= skillBDuration)
        {
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillB ����");
    }

    public void SkellBAtk(BossMonster monster)
    {
        GameObject skillBObj = monster.ObjSpwan(monster.skillBPrefab, monster.transform.position);
        skillBObj.GetComponent<BossSkillBObject>().Seting(monster.transform.position, monster.atkDamage);
    }
}
