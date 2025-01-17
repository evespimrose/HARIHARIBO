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

    public float damage = 10f;
    public float fireDamage = 5f;
    public float fireInterval = 1f;
    public float fireDuration = 5f;

    public float additionalWaitTime = 0.5f;

    private bool Action = false;

    public override void Enter(BossMonster monster)
    {
        damage = monster.atkDamage * 1f;
        fireDamage = monster.atkDamage * 0.2f;
        Action = true;
        monster.isAtk = true;
        projectilePrefabA = monster.skillEPrefab;
        Debug.Log("SkillE 진입");
        monster.StartCoroutine(SkillECoroutine(monster));
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
        Debug.Log("SkillE 종료");
    }

    private IEnumerator SkillECoroutine(BossMonster monster)
    {
        monster.TargetLook(Vector3.zero);

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
        yield return null;
        yield return new WaitForSeconds(additionalWaitTime);

        Action = false;
    }

    private void SpawnProjectile1(BossMonster monster)
    {
        Vector3 spawnPos = monster.transform.position + monster.transform.forward * 0.2f;
        spawnPos.y = 1f;
        GameObject skillEBullet = monster.ObjSpwan(projectilePrefabA, spawnPos);
        BossSkillEObject missileScript = skillEBullet.GetComponent<BossSkillEObject>();
        missileScript.SetMissileProperties(damage, fireDamage, fireInterval, fireDuration);
        missileScript.SetMissileType(1);
    }
}
