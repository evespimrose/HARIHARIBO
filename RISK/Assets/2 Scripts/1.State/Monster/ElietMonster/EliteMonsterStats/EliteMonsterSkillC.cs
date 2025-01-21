using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterSkillC : BaseState<EliteMonster>
{
    public EliteMonsterSkillC(StateHandler<EliteMonster> handler) : base(handler) { }

    public float skillCTime = 2.08f; //애니메이션 총 시간
    public float atkDuration = 1f;
    public float skillCDuration = 0.8f;

    public float skillCDamage = 10f;

    public override void Enter(EliteMonster monster)
    {
        skillCDamage = monster.atkDamage * 0.75f;
        Debug.Log("SkillC 진입");
        monster.StartCoroutine(SkillECoroutine(monster));
    }

    public override void Exit(EliteMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillC 종료");
    }

    private IEnumerator SkillECoroutine(EliteMonster monster)
    {
        monster.TargetLook(monster.target.position);

        yield return new WaitForSeconds(atkDuration); // 선딜레이

        // 애니메이션 실행
        monster.animator.SetTrigger("SkillC");

        // 공격 타이밍 대기
        yield return new WaitForSeconds(skillCDuration);

        // 미사일 발사
        SpawnProjectile1(monster);

        // 애니메이션이 끝날 때까지 대기 (애니메이션이 'SkillC'일 때 완료된 상태 확인)
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            // "SkillE" 애니메이션이 끝난 상태인지 확인
            return !stateInfo.IsName("SkillC") || stateInfo.normalizedTime >= 1f;
        });

        // 상태 변경
        monster.eMHandler.ChangeState(typeof(EliteMonsterIdle));
    }

    private void SpawnProjectile1(EliteMonster monster)
    {
        // 발사체의 방향을 배열로 설정 (정면, 좌측 45도, 우측 45도)
        Vector3[] directions = new Vector3[]
        {
        monster.transform.forward, // 정면
        monster.transform.forward, // 정면 (두 번째 발사체)
        Quaternion.Euler(0, -45, 0) * monster.transform.forward, // 좌측 45도
        Quaternion.Euler(0, 45, 0) * monster.transform.forward // 우측 45도
        };

        // 4발의 발사체를 저장할 배열
        GameObject[] skillEBullets = new GameObject[4];

        // 각 발사체 생성 및 설정
        for (int i = 0; i < 4; i++)
        {
            // 스폰 위치 계산
            Vector3 spawnPos = monster.transform.position + directions[i] * 0.2f;
            spawnPos.y = 1f;  // Y 위치 고정

            // 발사체 스폰
            skillEBullets[i] = monster.ObjSpwan(monster.skillCPrefab, spawnPos);

            // 발사체의 방향 설정
            skillEBullets[i].transform.rotation = Quaternion.LookRotation(directions[i]);

            // 발사체의 속성 설정 (missileSpeed, missileDistance 등을 직접 설정)
            EliteSkillCObjcect missileScript = skillEBullets[i].GetComponent<EliteSkillCObjcect>();
            missileScript.InitMissile(directions[i], 20f);
            missileScript.SetDamage(skillCDamage);
        }
    }
}
