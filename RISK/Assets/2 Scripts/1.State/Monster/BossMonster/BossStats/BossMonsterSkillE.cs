using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillE : BaseState<BossMonster>
{
    public BossMonsterSkillE(StateHandler<BossMonster> handler) : base(handler) { }

    // 스킬 5: 원거리 공격 2
    public GameObject projectilePrefabA; // 투사체 1 (BossSkillEObjectA)

    public float bulletDamage = 10f; // 투사체 데미지
    public float fireDamage = 5f;   // 불 장판 도트 데미지

    private bool hasSpawnedProjectile = false;

    public override void Enter(BossMonster monster)
    {
        projectilePrefabA = monster.skillEPrefab;
        Debug.Log("SkillE 진입");
        monster.animator.SetTrigger("SkillE"); // 애니메이션 실행
        monster.StartSkillCoroutine(SkillECoroutine(monster)); // 스킬 시전 코루틴 시작
    }

    private IEnumerator SkillECoroutine(BossMonster monster)
    {
        // 애니메이션 딜레이 처리 (필드에 애니메이션 시간 설정)
        yield return new WaitForSeconds(1f); // 예시로 1초 딜레이

        // 애니메이션 후 투사체 1 발사
        if (!hasSpawnedProjectile)
        {
            SpawnProjectile1(monster); // 투사체 1 발사
            hasSpawnedProjectile = true;
        }
    }

    private void SpawnProjectile1(BossMonster monster)
    {
        // 투사체 1 생성 (BossSkillEObjectA)
        Vector3 spawnPosition = monster.transform.position + monster.transform.forward; // 예시로 몬스터 앞에 생성
        GameObject projectile = monster.ObjSpwan(projectilePrefabA, spawnPosition);
        projectile.GetComponent<BossSkillEObjectA>().bulletDamage = bulletDamage;
        projectile.GetComponent<BossSkillEObjectA>().fireDamage = fireDamage;
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillE 종료");
    }
}
