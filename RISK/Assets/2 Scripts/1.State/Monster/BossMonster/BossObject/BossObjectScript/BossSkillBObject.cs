using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossSkillBObject : MonoBehaviour
{
    public enum AtkType
    {
        Melee,
        Range
    }

    public float atkDamage;          // 공격 데미지
    public float moveDelay = 1f;  // 초기 대기 시간
    public float moveDuration = 2f;  // 이동 시간
    public float atkRange = 20f;     // 공격 범위
    public float innerRadius = 15f;  // 내부 반경 (도넛형 공격)

    public GameObject model;         // 모델 객체

    private Vector3 modelStartPos;   // 모델 시작 위치
    private Vector3 modelTargetPos;  // 모델 목표 위치
    private float elapsedTime = 0f;  // 경과 시간
    private bool isMoving = false;   // 모델 이동 중 여부
    private bool isAtk = false; // 공격 실행 여부

    void Start()
    {
        // 모델 시작 위치와 목표 위치 설정
        modelStartPos = transform.position + new Vector3(0, 10f, 0); // 시작 위치 (y=10)
        modelTargetPos = transform.position;                         // 목표 위치 (y=0)
        model.transform.position = modelStartPos;                    // 모델을 시작 위치로 설정
        StartCoroutine(MoveDelay());
    }

    private IEnumerator MoveDelay()
    {
        // 초기 대기 시간
        yield return new WaitForSeconds(moveDelay);
        isMoving = true; // 모델 이동 시작
    }

    void Update()
    {
        if (!isMoving || isAtk) return; // 이동 중이 아니거나 이미 공격이 실행된 경우 중지

        // 모델이 y=10에서 y=0으로 부드럽게 이동
        elapsedTime += Time.deltaTime;
        if (elapsedTime <= moveDuration)
        {
            float t = elapsedTime / moveDuration; // 0~1의 비율
            model.transform.position = Vector3.Lerp(modelStartPos, modelTargetPos, t);
        }
        else
        {
            // 이동이 완료되면 Atk 실행
            isMoving = false;
            isAtk = true;
            SkillAAtk();
        }
    }

    public void Seting(Vector3 spwanPos, float damage)
    {
        this.transform.position = spwanPos; // 오브젝트의 위치 고정
        this.atkDamage = damage;
    }

    private void SkillAAtk()
    {
        // 공격 선택
        int num = Random.Range(0, 2);
        switch (num)
        {
            case 0:
                MeleeAtk();
                break;
            case 1:
                RangeAtk();
                break;
        }
    }

    public void MeleeAtk()
    {
        Vector3 atkCenter = transform.position; // 공격 중심점
        Collider[] cols = Physics.OverlapSphere(atkCenter, atkRange); // 외부 반경 내 객체 탐지
        Debug.Log("공격 진입");

        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<ITakedamage>()?.Takedamage(atkDamage);
                Debug.Log("근접 공격 성공");
            }
        }
        Destroy(this.gameObject);
    }

    public void RangeAtk()
    {
        Vector3 atkCenter = transform.position; // 공격 중심점
        Collider[] cols = Physics.OverlapSphere(atkCenter, atkRange); // 외부 반경 내 객체 탐지
        Debug.Log("공격 진입");

        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - atkCenter).normalized;
                float dirTarget = Vector3.Distance(atkCenter, col.transform.position);

                // 내부 반경과 외부 반경 사이의 객체만 검사
                if (dirTarget >= innerRadius && dirTarget <= atkRange)
                {
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(atkDamage);
                    Debug.Log("원형 공격 성공");
                }
            }
        }
        Destroy(this.gameObject);
    }
}
