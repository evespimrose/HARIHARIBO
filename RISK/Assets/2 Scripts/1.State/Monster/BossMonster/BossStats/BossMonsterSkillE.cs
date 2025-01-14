using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterSkillE : BaseState<BossMonster>
{
    public BossMonsterSkillE(StateHandler<BossMonster> handler) : base(handler) { }

    // ��ų 5: ���Ÿ� ���� 2
    public GameObject projectilePrefabA; // ����ü 1 (BossSkillEObjectA)

    public float bulletDamage = 10f; // ����ü ������
    public float fireDamage = 5f;   // �� ���� ��Ʈ ������

    private bool hasSpawnedProjectile = false;

    public override void Enter(BossMonster monster)
    {
        projectilePrefabA = monster.skillEPrefab;
        Debug.Log("SkillE ����");
        monster.animator.SetTrigger("SkillE"); // �ִϸ��̼� ����
        monster.StartSkillCoroutine(SkillECoroutine(monster)); // ��ų ���� �ڷ�ƾ ����
    }

    private IEnumerator SkillECoroutine(BossMonster monster)
    {
        // �ִϸ��̼� ������ ó�� (�ʵ忡 �ִϸ��̼� �ð� ����)
        yield return new WaitForSeconds(1f); // ���÷� 1�� ������

        // �ִϸ��̼� �� ����ü 1 �߻�
        if (!hasSpawnedProjectile)
        {
            SpawnProjectile1(monster); // ����ü 1 �߻�
            hasSpawnedProjectile = true;
        }
    }

    private void SpawnProjectile1(BossMonster monster)
    {
        // ����ü 1 ���� (BossSkillEObjectA)
        Vector3 spawnPosition = monster.transform.position + monster.transform.forward; // ���÷� ���� �տ� ����
        GameObject projectile = monster.ObjSpwan(projectilePrefabA, spawnPosition);
        projectile.GetComponent<BossSkillEObjectA>().bulletDamage = bulletDamage;
        projectile.GetComponent<BossSkillEObjectA>().fireDamage = fireDamage;
    }

    public override void Exit(BossMonster monster)
    {
        monster.AtkEnd();
        Debug.Log("SkillE ����");
    }
}
