using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossMonsterSkillE : BaseState<BossMonster>
{
    public BossMonsterSkillE(StateHandler<BossMonster> handler) : base(handler) { }
    // ��ų 5: ���Ÿ� ���� 2
    public GameObject projectilePrefabA;

    public float skillETime = 2.08f; //�ִϸ��̼� �� �ð�
    public float atkDuration = 1f;
    public float skillEDuration = 0.8f;

    public float bulletDamage = 10f;
    public float fireDamage = 5f;
    public float fireInterval = 1f;
    public float fireDuration = 5f;

    public override void Enter(BossMonster monster)
    {
        projectilePrefabA = monster.skillEPrefab;
        Debug.Log("SkillE ����");
        monster.StartSkillCoroutine(SkillECoroutine(monster));
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillE ����");
    }

    private IEnumerator SkillECoroutine(BossMonster monster)
    {
        monster.TargetLook(monster.target.position);

        yield return new WaitForSeconds(atkDuration); // ��������

        // �ִϸ��̼� ����
        monster.animator.SetTrigger("SkillE");

        // ���� Ÿ�̹� ���
        yield return new WaitForSeconds(skillEDuration);

        // �̻��� �߻�
        SpawnProjectile1(monster);

        // �ִϸ��̼��� ���� ������ ��� (�ִϸ��̼��� 'SkillE'�� �� �Ϸ�� ���� Ȯ��)
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
            // "SkillE" �ִϸ��̼��� ���� �������� Ȯ��
            return !stateInfo.IsName("SkillE") || stateInfo.normalizedTime >= 1f;
        });

        // ���� ����
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
