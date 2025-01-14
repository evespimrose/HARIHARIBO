using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillEObjectB : MonoBehaviour
{
    private Vector3 direction; // 이동 방향
    private float moveSpeed = 15f; // 속도
    private float maxDistance = 10f; // 최대 이동 거리
    public GameObject fireFieldPrefab; // 불 장판 프리팹
    public float fireFieldDuration = 5f; // 불 장판 지속 시간

    // 초기화 함수 (방향 및 이동 거리 설정)
    public void Initialize(Vector3 dir, float damage)
    {
        direction = dir;
        Destroy(gameObject, maxDistance / moveSpeed); // 최대 이동 거리가 다되면 삭제
    }

    private void Update()
    {
        // 이동 처리
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 불 장판 생성 (투사체가 이동하는 동안)
        CreateFireField();
    }

    // 불 장판 생성 함수
    private void CreateFireField()
    {
        if (fireFieldPrefab != null)
        {
            // 투사체가 이동한 위치에 불 장판 생성
            GameObject fireField = Instantiate(fireFieldPrefab, transform.position, Quaternion.identity);
            Destroy(fireField, fireFieldDuration); // 일정 시간 후 불 장판 제거
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerDebuff>().Fire(10f); // 데미지 처리
            Destroy(gameObject); // 투사체 2 삭제
        }
    }
}
