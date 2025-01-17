using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterSkillB : BaseState<EliteMonster>
{
    public EliteMonsterSkillB(StateHandler<EliteMonster> handler) : base(handler) { }

    public float skillBDamage = 10f;
    public float atkDelay = 0f;
    public float skillCDuration = 1.21f;
    public float skillCAtkTime = 1f;
    public float additionalWaitTime = 0.4f;

    public override void Enter(EliteMonster monster)
    {
        Debug.Log("SkillC 진입");
        monster.StartCoroutine(SkillCCoroutine(monster));
    }

    public override void Exit(EliteMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillC 종료");
    }

    private IEnumerator SkillCCoroutine(EliteMonster monster)
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
        yield return new WaitForSeconds(additionalWaitTime);

        monster.eMHandler.ChangeState(typeof(EliteMonsterIdle));
        Debug.Log("SkillC 종료 후 Idle 상태로 전환");
    }

    private void SkillCAtk(EliteMonster monster)
    {
        GameObject skillCObj = monster.ObjSpwan(monster.skillBPrefab, monster.transform.position);
        Vector3 forwardDir = new Vector3(monster.transform.forward.x, 0f, monster.transform.forward.z).normalized;
        skillCObj.transform.forward = forwardDir;
        skillCObj.transform.position = new Vector3(skillCObj.transform.position.x, skillCObj.transform.position.y + 1f, skillCObj.transform.position.z);
        skillCObj.GetComponent<EliteSkillBObjcect>().Seting(skillBDamage);
    }
}
