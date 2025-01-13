using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillB : BaseState<BossMonster>
{
    public BossMonsterSkillB(StateHandler<BossMonster> handler) : base(handler) { }

    //스킬2 범위기 1
    public float skillBDuration = 2.09f;
    public float atkDelay = 1f;//선딜레이

    private float startTime;
    private bool isAction = false;

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillB 진입");
        // 애니메이션 실행
        monster.animator.SetTrigger("SkillB");
        // 선딜레이 후 공격 실행
        startTime = Time.time + atkDelay;
        isAction = false;
    }

    public override void Update(BossMonster monster)
    {
        float elapsedTime = Time.time - startTime;
        // 선딜레이 이후 공격 실행
        if (!isAction && elapsedTime >= 0f)
        {
            SkellBAtk(monster);  
            isAction = true;     
        }
        // 스킬 지속 시간 후 상태 변경
        if (elapsedTime >= skillBDuration)
        {
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillB 종료");
    }

    public void SkellBAtk(BossMonster monster)
    {
        GameObject skillBObj = monster.ObjSpwan(monster.skillBPrefab, monster.transform.position);
        skillBObj.GetComponent<BossSkillBObject>().Seting(monster.transform.position, monster.atkDamage);
    }
}
