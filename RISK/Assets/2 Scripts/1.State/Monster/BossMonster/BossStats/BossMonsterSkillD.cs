using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillD : BaseState<BossMonster>
{
    public BossMonsterSkillD(StateHandler<BossMonster> handler) : base(handler) { }

    public float damage = 1f;
    public float atkDelay = 0f; // 선딜레이
    public float skillDDuration = 2.4f; // 애니메이션 지속 시간
    public float skillFAtkTime = 1f; // 애니메이션 시작 후 SkillDAtk 실행까지 기다릴 시간
    public float additionalWaitTime = 0.2f; // 애니메이션 종료 후 추가 대기 시간

    private bool Action = false;

    public override void Enter(BossMonster monster)
    {
        damage = monster.atkDamage * 1f;
        Action = true;
        monster.isAtk = true;
        Debug.Log("SkillD 진입");
        monster.StartCoroutine(SkillDCoroutine(monster)); // 코루틴 시작
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
        Debug.Log("SkillD 종료");
    }

    private IEnumerator SkillDCoroutine(BossMonster monster)
    {
        // 선딜레이 후 애니메이션 실행
        yield return new WaitForSeconds(atkDelay);
        monster.TargetLook(monster.target.position);
        monster.animator.SetTrigger("SkillD");
        Debug.Log("SkillD 애니메이션 시작");

        // 애니메이션 시작 후 일정 시간이 지나면 SkillDAtk 실행
        yield return new WaitForSeconds(skillFAtkTime);
        SkillDAtk(monster);

        // 애니메이션이 끝날 때까지 대기
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            return !stateInfo.IsName("SkillD") || stateInfo.normalizedTime >= 1f;
        });

        monster.animator.SetTrigger("Idle");
        yield return null;
        yield return new WaitForSeconds(additionalWaitTime);

        Action = false;
        Debug.Log("SkillD 종료 후 Idle 상태로 전환");
    }

    private void SkillDAtk(BossMonster monster)
    {
        int atkCount = 8;
        float angleStep = 360f / atkCount;
        for (int i = 0; i < atkCount; i++)
        {
            float angle = i * angleStep;
            Vector3 rotDir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            rotDir = new Vector3(rotDir.x, 0f, rotDir.z).normalized;
            GameObject skillDObj = monster.ObjSpwan(monster.skillDPrefab, new Vector3(monster.transform.position.x, 1f, monster.transform.position.z));
            skillDObj.transform.rotation = Quaternion.LookRotation(rotDir, Vector3.up);
            skillDObj.GetComponent<BossSkillDObject>().Seting(damage);
            Rigidbody skillRigidbody = skillDObj.GetComponent<Rigidbody>();
            if (skillRigidbody != null)
            {
                skillRigidbody.velocity = rotDir * 10f; 
            }
        }
    }

}
