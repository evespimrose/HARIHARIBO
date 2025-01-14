using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillB : BaseState<BossMonster>
{
    public BossMonsterSkillB(StateHandler<BossMonster> handler) : base(handler) { }

    // 스킬2 범위기 1
    public float skillBDuration = 2.09f;  // 스킬 지속 시간
    public float atkDelay = 1f;            // 선딜레이

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillB 진입");
        monster.isAtk = true;
        monster.StartSkillCoroutine(SkillBCoroutine(monster));
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillB 종료");
        monster.isAtk = false;
    }

    private IEnumerator SkillBCoroutine(BossMonster monster)
    {
        monster.animator.SetTrigger("SkillB");
        yield return new WaitForSeconds(atkDelay);

        // 스킬 공격 실행
        SkellBAtk(monster);

        // 스킬 지속 시간을 기다리고 상태 전환
        yield return new WaitForSeconds(skillBDuration);

        // 스킬 지속 시간 후 0.2초 여유를 두고 상태 전환
        yield return new WaitForSeconds(0.2f);
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // 상태 전환
        Debug.Log("SkillB 종료 후 Idle 상태로 전환");
    }

    public void SkellBAtk(BossMonster monster)
    {
        // 스킬 공격 실행
        GameObject skillBObj = monster.ObjSpwan(monster.skillBPrefab, monster.transform.position);
        skillBObj.GetComponent<BossSkillBObject>().Seting(monster.transform.position, monster.atkDamage);
    }
}
