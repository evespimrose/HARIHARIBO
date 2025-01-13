using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillC : BaseState<BossMonster>
{
    public BossMonsterSkillC(StateHandler<BossMonster> handler) : base(handler) { }

    public float atkDelay = 0f;//선딜레이
    public float skillCDuration = 1.21f;//애니메이션 지속 시간
    public float skillCAtkTiem = 1f;//애니메이션 시작 후 SkillCAtk 실행까지 기다릴 시간 (필드값)
    private float startTime;
    private bool isAction = false;//SkillCAtk가 한 번만 실행되도록 관리

    public override void Enter(BossMonster monster)
    {
        Debug.Log("SkillC 진입");
        isAction = false;//스킬C 어택이 한 번만 실행되도록 관리
        startTime = Time.time + atkDelay;//선딜레이를 고려한 시작 시간
        //선딜레이가 0일 때 즉시 애니메이션 실행
        if (atkDelay <= 0f)
        {
            monster.animator.SetTrigger("SkillC");
        }
    }

    public override void Update(BossMonster monster)
    {
        //선딜레이가 끝난 후 애니메이션 실행
        if (Time.time >= startTime && !monster.animator.GetCurrentAnimatorStateInfo(0).IsName("SkillC"))
        {
            monster.animator.SetTrigger("SkillC");
        }
        //애니메이션이 시작된 후 일정 시간이 지나면 SkillCAtk 실행
        if (Time.time - startTime >= skillCAtkTiem && !isAction)
        {
            SkillCAtk(monster);
            isAction = true;
        }
        //애니메이션이 끝난 후 0.2초 여유를 두고 상태 전환
        if (Time.time - startTime >= skillCDuration + 0.2f)
        {
            monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
        }
    }

    public override void Exit(BossMonster monster)
    {
        Debug.Log("SkillC 종료");
    }

    private void SkillCAtk(BossMonster monster)
    {
        GameObject skillCObj = monster.ObjSpwan(monster.skillCPrefab, monster.transform.position);
        //y축 0 고정
        Vector3 forwardDir = new Vector3(monster.transform.forward.x, 0f, monster.transform.forward.z).normalized;
        //방향 설정
        skillCObj.transform.forward = forwardDir;
        skillCObj.transform.position = new Vector3(skillCObj.transform.position.x, skillCObj.transform.position.y + 1f, skillCObj.transform.position.z);
        skillCObj.GetComponent<BossSkillCObject>().Seting(monster.atkDamage);
    }
}
