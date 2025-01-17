using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillC : BaseState<BossMonster>
{
    public BossMonsterSkillC(StateHandler<BossMonster> handler) : base(handler) { }

    public float damage = 1f;
    public float atkDelay = 0f;         
    public float skillCDuration = 1.21f;
    public float skillCAtkTime = 1f;    
    public float additionalWaitTime = 0.4f;

    private bool Action = false;

    public override void Enter(BossMonster monster)
    {
        damage = monster.atkDamage * 2f;
        Action = true;
        monster.isAtk = true;
        Debug.Log("SkillC 진입");
        monster.StartCoroutine(SkillCCoroutine(monster)); 
    }

    public override void Update(BossMonster monster)
    {
        if (Action == false)
        {
            monster.isAtk = false;
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillC 종료");
    }

    private IEnumerator SkillCCoroutine(BossMonster monster)
    {
        // 선딜레이
        yield return new WaitForSeconds(atkDelay);

        monster.TargetLook(monster.target.position);
        monster.animator.SetTrigger("SkillC");
        yield return new WaitForSeconds(skillCAtkTime);

        SkillCAtk(monster);  // SkillCAtk 실행
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsName("SkillC") || stateInfo.normalizedTime >= 1f;
        });

        monster.animator.SetTrigger("Idle");
        yield return null;
        yield return new WaitForSeconds(additionalWaitTime);

        Action = false;
        Debug.Log("SkillC 종료 후 Idle 상태로 전환");
    }

    private void SkillCAtk(BossMonster monster)
    {
        GameObject skillCObj = monster.ObjSpwan(monster.skillCPrefab, monster.transform.position);
        Vector3 forwardDir = new Vector3(monster.transform.forward.x, 0f, monster.transform.forward.z).normalized;
        skillCObj.transform.forward = forwardDir;
        skillCObj.transform.position = new Vector3(skillCObj.transform.position.x, skillCObj.transform.position.y + 1f, skillCObj.transform.position.z);
        skillCObj.GetComponent<BossSkillCObject>().Seting(damage);
    }
}
