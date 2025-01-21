using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillDObject : MonoBehaviour
{
    public float moveSpeed = 5f;       // 이동 속도
    public float moveDistance = 30f;  // 이동할 거리
    public float atkDamage;           // 공격 데미지

    private Vector3 startPos;    // 시작 위치
    private bool isSeting = false;

    private void Start()
    {
        moveSpeed = 5f;
        moveDistance = 30f;
    }

    void Update()
    {
        if (!isSeting) return;

        // 오브젝트의 로컬 전방으로 이동
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * 2);

        // 이동 거리 확인
        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            Destroy(gameObject); // 이동 거리 도달 시 오브젝트 삭제
        }
    }

    public void Seting(float damage)
    {
        this.atkDamage = damage;
        startPos = transform.position; // 초기 위치 저장
        isSeting = true;               // 이동 활성화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<ITakedamage>()?.Takedamage(atkDamage);
            Destroy(gameObject);
        }
    }
}
