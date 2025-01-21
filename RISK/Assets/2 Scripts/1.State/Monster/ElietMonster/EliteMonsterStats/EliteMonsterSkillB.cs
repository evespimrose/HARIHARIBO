using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterSkillB : BaseState<EliteMonster>
{
    public EliteMonsterSkillB(StateHandler<EliteMonster> handler) : base(handler) { }

    public float skillBDamage;
    public float atkDelay = 0f;
    public float skillBDuration = 1.21f;
    public float skillBAtkTime = 0.8f;
    public float additionalWaitTime = 0.8f;

    private Coroutine action;

    public override void Enter(EliteMonster monster)
    {
        skillBDamage = monster.atkDamage * 1.2f;
        Debug.Log("SkillB 진입");
        action = monster.StartCoroutine(SkillCCoroutine(monster));
    }

    public override void Exit(EliteMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillB 종료");
        monster.StopCoroutine(action);
    }

    private IEnumerator SkillCCoroutine(EliteMonster monster)
    {
        // 선딜레이
        yield return new WaitForSeconds(atkDelay);

        monster.TargetLook(monster.target.position);
        monster.animator.SetTrigger("SkillB");
        yield return new WaitForSeconds(skillBAtkTime);

        SkillCAtk(monster);  // SkillCAtk 실행
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsName("SkillB") || stateInfo.normalizedTime >= 1f;
        });

        monster.animator.SetTrigger("Idle");
        yield return new WaitForSeconds(additionalWaitTime);

        monster.eMHandler.ChangeState(typeof(EliteMonsterIdle));
        Debug.Log("SkillB 종료 후 Idle 상태로 전환");
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
