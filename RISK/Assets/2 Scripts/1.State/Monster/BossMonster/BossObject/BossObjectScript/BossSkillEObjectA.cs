using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillEObjectA : MonoBehaviour
{
    public float moveSpeed = 10f; // 이동 속도
    public float maxDistance = 15f; // 최대 이동 거리
    public GameObject fireFieldPrefab; // 불 장판 프리팹
    public float fireDamage = 5f; // 불 장판 도트 데미지
    public float bulletDamage = 10f; // 투사체 데미지
    public GameObject projectilePrefabB; // 투사체 2 (BossSkillEObjectB) 프리팹

    private Vector3 startPosition; // 투사체 시작 위치
    private Vector3 targetPosition; // 투사체 목표 위치

    private float distanceTraveled = 0f; // 이동한 거리
    private float fireFieldSpacing = 2f; // 불 장판 소환 간격 (거리 기준)
    private float fireFieldTimer = 0f; // 불 장판 소환 타이머
    private float fireFieldInterval = 0.5f; // 불 장판 소환 간격 (초)

    private bool hasSpawnedProjectileB = false; // 투사체 B가 이미 소환되었는지 여부

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = new Vector3(0, 0, 0); // 목표 위치: 0,0,0
    }

    private void Update()
    {
        // A 오브젝트가 목표 지점(0, 0, 0)에 도달할 때까지 이동
        if (Vector3.Distance(startPosition, transform.position) < maxDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            // 이동한 거리 계산
            distanceTraveled += moveSpeed * Time.deltaTime;

            // 일정 간격마다 불장판을 소환
            fireFieldTimer += Time.deltaTime;

            if (fireFieldTimer >= fireFieldInterval)
            {
                CreateFireField(moveDirection); // 불장판 소환
                fireFieldTimer = 0f; // 타이머 초기화
            }
        }
        else
        {
            // 목표 지점에 도달하면 A 오브젝트 사라짐
            Destroy(gameObject); // A 오브젝트 삭제

            // 투사체 B를 한번만 소환하도록 설정
            if (!hasSpawnedProjectileB)
            {
                SpawnProjectile2();
                hasSpawnedProjectileB = true;
            }
        }
    }

    private void CreateFireField(Vector3 moveDirection)
    {
        // 불장판 소환
        Vector3 spawnPosition = transform.position; // 현재 위치에 불장판 소환

        // 불장판을 이동 방향에 맞춰 회전시키기
        GameObject fireField = Instantiate(fireFieldPrefab, spawnPosition, Quaternion.LookRotation(moveDirection));
        fireField.transform.localScale = new Vector3(1, 1, 1); // 불장판 크기 설정 (1, 1, 1)
        Destroy(fireField, fireDamage); // 일정 시간 후 불 장판 제거
    }

    private void SpawnProjectile2()
    {
        // 투사체 2를 8방향으로 발사
        Vector3[] directions = new Vector3[]
        {
            Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
            new Vector3(1, 0, 1).normalized, new Vector3(-1, 0, 1).normalized,
            new Vector3(1, 0, -1).normalized, new Vector3(-1, 0, -1).normalized
        };

        foreach (Vector3 dir in directions)
        {
            // 투사체 2 소환
            GameObject proj2 = Instantiate(projectilePrefabB, transform.position, Quaternion.identity);
            proj2.GetComponent<BossSkillEObjectB>().Initialize(dir, bulletDamage); // 투사체 2 초기화
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 플레이어와 충돌 시 투사체 1 사라지고 데미지 적용
            other.gameObject.GetComponent<PlayerDebuff>().Fire(bulletDamage);
            Destroy(gameObject); // 투사체 1 삭제

            // 투사체 2 발사
            if (!hasSpawnedProjectileB)
            {
                SpawnProjectile2();
                hasSpawnedProjectileB = true;
            }
        }
    }
}
