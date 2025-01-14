using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillEObjectA : MonoBehaviour
{
    public float moveSpeed = 10f; // �̵� �ӵ�
    public float maxDistance = 15f; // �ִ� �̵� �Ÿ�
    public GameObject fireFieldPrefab; // �� ���� ������
    public float fireDamage = 5f; // �� ���� ��Ʈ ������
    public float bulletDamage = 10f; // ����ü ������
    public GameObject projectilePrefabB; // ����ü 2 (BossSkillEObjectB) ������

    private Vector3 startPosition; // ����ü ���� ��ġ
    private Vector3 targetPosition; // ����ü ��ǥ ��ġ

    private float distanceTraveled = 0f; // �̵��� �Ÿ�
    private float fireFieldSpacing = 2f; // �� ���� ��ȯ ���� (�Ÿ� ����)
    private float fireFieldTimer = 0f; // �� ���� ��ȯ Ÿ�̸�
    private float fireFieldInterval = 0.5f; // �� ���� ��ȯ ���� (��)

    private bool hasSpawnedProjectileB = false; // ����ü B�� �̹� ��ȯ�Ǿ����� ����

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(0, 0, 0); // ��ǥ ��ġ: 0,0,0
    }

    private void Update()
    {
        // A ������Ʈ�� ��ǥ ����(0, 0, 0)�� ������ ������ �̵�
        if (Vector3.Distance(startPosition, transform.position) < maxDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // �̵��� �Ÿ� ���
            distanceTraveled += moveSpeed * Time.deltaTime;

            // ���� ���ݸ��� �������� ��ȯ
            fireFieldTimer += Time.deltaTime;

            if (fireFieldTimer >= fireFieldInterval)
            {
                CreateFireField(moveDirection); // ������ ��ȯ
                fireFieldTimer = 0f; // Ÿ�̸� �ʱ�ȭ
            }
        }
        else
        {
            // ��ǥ ������ �����ϸ� A ������Ʈ �����
            Destroy(gameObject); // A ������Ʈ ����

            // ����ü B�� �ѹ��� ��ȯ�ϵ��� ����
            if (!hasSpawnedProjectileB)
            {
                SpawnProjectile2();
                hasSpawnedProjectileB = true;
            }
        }
    }

    private void CreateFireField(Vector3 moveDirection)
    {
        // ������ ��ȯ
        Vector3 spawnPosition = transform.position; // ���� ��ġ�� ������ ��ȯ

        // �������� �̵� ���⿡ ���� ȸ����Ű��
        GameObject fireField = Instantiate(fireFieldPrefab, spawnPosition, Quaternion.LookRotation(moveDirection));
        fireField.transform.localScale = new Vector3(1, 1, 1); // ������ ũ�� ���� (1, 1, 1)
        Destroy(fireField, fireDamage); // ���� �ð� �� �� ���� ����
    }

    private void SpawnProjectile2()
    {
        // ����ü 2�� 8�������� �߻�
        Vector3[] directions = new Vector3[]
        {
            Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
            new Vector3(1, 0, 1).normalized, new Vector3(-1, 0, 1).normalized,
            new Vector3(1, 0, -1).normalized, new Vector3(-1, 0, -1).normalized
        };

        foreach (Vector3 dir in directions)
        {
            // ����ü 2 ��ȯ
            GameObject proj2 = Instantiate(projectilePrefabB, transform.position, Quaternion.identity);
            proj2.GetComponent<BossSkillEObjectB>().Initialize(dir, bulletDamage); // ����ü 2 �ʱ�ȭ
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // �÷��̾�� �浹 �� ����ü 1 ������� ������ ����
            other.gameObject.GetComponent<PlayerDebuff>().Fire(bulletDamage);
            Destroy(gameObject); // ����ü 1 ����

            // ����ü 2 �߻�
            if (!hasSpawnedProjectileB)
            {
                SpawnProjectile2();
                hasSpawnedProjectileB = true;
            }
        }
    }
}
