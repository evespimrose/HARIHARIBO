using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillB : BaseState<BossMonster>
{
    public BossMonsterSkillB(StateHandler<BossMonster> handler) : base(handler) { }

    // ��ų2 ������ 1
    public float skillBDuration = 2.09f;  // ��ų ���� �ð�
    public float atkDelay = 1f;            // ��������

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillB ����");
        monster.isAtk = true;
        monster.StartSkillCoroutine(SkillBCoroutine(monster));
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillB ����");
        monster.isAtk = false;
    }

    private IEnumerator SkillBCoroutine(BossMonster monster)
    {
        monster.animator.SetTrigger("SkillB");
        yield return new WaitForSeconds(atkDelay);

        // ��ų ���� ����
        SkellBAtk(monster);

        // ��ų ���� �ð��� ��ٸ��� ���� ��ȯ
        yield return new WaitForSeconds(skillBDuration);

        // ��ų ���� �ð� �� 0.2�� ������ �ΰ� ���� ��ȯ
        yield return new WaitForSeconds(0.2f);
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // ���� ��ȯ
        Debug.Log("SkillB ���� �� Idle ���·� ��ȯ");
    }

    public void SkellBAtk(BossMonster monster)
    {
        // ��ų ���� ����
        GameObject skillBObj = monster.ObjSpwan(monster.skillBPrefab, monster.transform.position);
        skillBObj.GetComponent<BossSkillBObject>().Seting(monster.transform.position, monster.atkDamage);
    }
}
