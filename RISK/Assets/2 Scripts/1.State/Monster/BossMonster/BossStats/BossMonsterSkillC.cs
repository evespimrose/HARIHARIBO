using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillC : BaseState<BossMonster>
{
    public BossMonsterSkillC(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkDelay = 0f;          // 선딜레이
    public float skillCDuration = 1.21f; // 애니메이션 지속 시간
    public float skillCAtkTime = 1f;     // 애니메이션 시작 후 SkillCAtk 실행까지 기다릴 시간

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillC 진입");
        monster.StartSkillCoroutine(SkillCCoroutine(monster));  // 코루틴 시작
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillC 종료");
    }

    private IEnumerator SkillCCoroutine(BossMonster monster)
    {
        // 선딜레이 후 애니메이션 실행
        yield return new WaitForSeconds(atkDelay);
        monster.animator.SetTrigger("SkillC");

        // 애니메이션 시작 후 skillCAtk 실행까지 대기
        yield return new WaitForSeconds(skillCAtkTime);
        SkillCAtk(monster);  // SkillCAtk 실행

        // 애니메이션 종료 후 0.2초 여유를 두고 상태 전환
        yield return new WaitForSeconds(skillCDuration + 0.2f);
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle)); // 상태 전환
        Debug.Log("SkillC 종료 후 Idle 상태로 전환");
    }

    private void SkillCAtk(BossMonster monster)
    {
        // 스킬 C 공격 객체 생성
        GameObject skillCObj = monster.ObjSpwan(monster.skillCPrefab, monster.transform.position);

        // y축 고정, 방향 설정
        Vector3 forwardDir = new Vector3(monster.transform.forward.x, 0f, monster.transform.forward.z).normalized;
        skillCObj.transform.forward = forwardDir;

        // 높이 조정 (기본 y 위치를 1로 설정)
        skillCObj.transform.position = new Vector3(skillCObj.transform.position.x, skillCObj.transform.position.y + 1f, skillCObj.transform.position.z);

        // 스킬 C 객체에 데미지 설정
        skillCObj.GetComponent<BossSkillCObject>().Seting(monster.atkDamage);
    }
}
