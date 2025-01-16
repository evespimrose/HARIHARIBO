using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillE : BaseState<BossMonster>
{
    public BossMonsterSkillE(StateHandler<BossMonster> handler) : base(handler) { }
    // 스킬 5: 원거리 공격 2
    public GameObject projectilePrefabA;

    public float skillETime = 2.08f; //애니메이션 총 시간
    public float atkDuration = 1f;
    public float skillEDuration = 0.8f;

    public float bulletDamage = 10f;
    public float fireDamage = 5f;
    public float fireInterval = 1f;
    public float fireDuration = 5f;

    public override void Enter(BossMonster monster)
    {
        projectilePrefabA = monster.skillEPrefab;
        Debug.Log("SkillE 진입");
        monster.StartSkillCoroutine(SkillECoroutine(monster));
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillE 종료");
    }

    private IEnumerator SkillECoroutine(BossMonster monster)
    {
        monster.TargetLook(monster.target.position);

        yield return new WaitForSeconds(atkDuration); // 선딜레이

        // 애니메이션 실행
        monster.animator.SetTrigger("SkillE");

        // 공격 타이밍 대기
        yield return new WaitForSeconds(skillEDuration);

        // 미사일 발사
        SpawnProjectile1(monster);

        // 애니메이션이 끝날 때까지 대기 (애니메이션이 'SkillE'일 때 완료된 상태 확인)
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            // "SkillE" 애니메이션이 끝난 상태인지 확인
            return !stateInfo.IsName("SkillE") || stateInfo.normalizedTime >= 1f;
        });

        // 상태 변경
        monster.bMHandler.ChangeState(typeof(BossMonsterIdle));
    }



    private void SpawnProjectile1(BossMonster monster)
    {
        Vector3 spawnPos = monster.transform.position + monster.transform.forward * 0.2f;
        spawnPos.y = 0.5f;
        GameObject skillEBullet = monster.ObjSpwan(projectilePrefabA, spawnPos);
        BossSkillEObjectA missileScript = skillEBullet.GetComponent<BossSkillEObjectA>();
        missileScript.SetMissileProperties(bulletDamage, fireDamage, fireInterval, fireDuration);
        missileScript.SetMissileType(1);
    }
}
