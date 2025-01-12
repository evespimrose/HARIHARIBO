using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillCObject : MonoBehaviour
{
    public float moveSpeed = 5f;       // 이동 속도
    public float moveDistance = 20f;  // 이동할 거리
    public int maxAtkCount = 1;    // 유닛별 최대 공격 횟수 (-1: 무제한)
    public float atkDamage;           // 공격 데미지

    private Vector3 startPos;    // 시작 위치
    private Dictionary<GameObject, int> atkCounts = new Dictionary<GameObject, int>(); // 유닛별 공격 횟수 기록
    private bool isSeting = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (isSeting == false) return;
        // 정면으로 이동
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // 이동 거리 확인
        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            Destroy(gameObject); // 이동 거리 도달 시 오브젝트 삭제
        }
    }

    public void Seting(float damage)
    {
        this.atkDamage = damage;
        isSeting = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 공격 기록 초기화
            if (!atkCounts.ContainsKey(other.gameObject))
            {
                atkCounts[other.gameObject] = 0;
            }

            // 유닛별 최대 공격 횟수 검사
            if (maxAtkCount == -1 || atkCounts[other.gameObject] < maxAtkCount)
            {
                // 공격 실행
                var target = other.GetComponent<ITakedamage>();
                if (target != null)
                {
                    target.Takedamage(atkDamage);
                    atkCounts[other.gameObject]++; // 해당 유닛의 공격 횟수 증가
                    Debug.Log($"Player {other.name} attacked {atkCounts[other.gameObject]} times.");
                }
            }
            else
            {
                Debug.Log($"Player {other.name} already attacked {atkCounts[other.gameObject]} times (max: {maxAtkCount}).");
            }
        }
    }
}
