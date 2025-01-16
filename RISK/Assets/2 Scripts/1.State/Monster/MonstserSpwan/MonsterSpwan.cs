using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSpwan : MonoBehaviour
{
    public int wave = 1;
    public int inWave = 1;
    public float SpwanTiem = 5f;
    [Header("근접 몬스터")]
    [Tooltip("근접 몬스터 스폰될 갯수")]
    public int meleeMonsterSpwanCount = 5;
    [Tooltip("근거리 몬스터 프리팹")]
    public List<GameObject> meleeMonsterPrefab;
    [Header("원거리 몬스터")]
    [Tooltip("원거리 몬스터 스폰될 갯수")]
    public int rangeMonsterSpwanCount = 2;
    [Tooltip("원거리 몬스터 프리팹")]
    public List<GameObject> rangeMonsterPrefab;

    public float meleeMinRadius = 5f;
    public float meleeMaxRadius = 10f;
    public float rnageMinRadius = 10f;
    public float rangeMaxRadius = 15f;
    public Vector3 center;

    private void Awake()
    {
        center = transform.position;
    }

    void Start()
    {
        StartCoroutine(MonsterSpwanCorutine());
    }

    void Update()
    {
        
    }

    public IEnumerator MonsterSpwanCorutine()
    {
        int lastQuadrant = 0;
        int quadrant = 0;
        float curTime = 0f;
        while (true)
        {
            //스폰구역 선택
            //랜덤으로 구역을 선택 (1, 2, 3, 4 사분면)
            while (true)
            {
                quadrant = Random.Range(1, 5);
                if (lastQuadrant != quadrant || lastQuadrant == 0)
                {
                    lastQuadrant = quadrant;
                    break; 
                }
            }
            Vector3 spwanPos;
            //근접 몬스터 스폰
            for (int i = 0; i < meleeMonsterSpwanCount; i++)
            {
                GameObject MeleeMonster = MeleeMonsterCreate();
                spwanPos = GetMeleeSpwanPos(quadrant);  // 근접 몬스터의 스폰 위치
                MeleeMonster.transform.position = spwanPos;
            }
            //원거리 몬스터 스폰
            for (int i = 0; i < rangeMonsterSpwanCount; i++)
            {
                GameObject RangeMonster = RangeMonsterCreate();
                spwanPos = GetRangeSpwanPos(quadrant);
                RangeMonster.transform.position = spwanPos;
            }
            while (curTime > SpwanTiem || UnitManager.Instance.monsters.Count == 0)
            {
                curTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    public Vector3 GetMeleeSpwanPos(int quadrant)
    {
        // 근접 몬스터에 맞는 범위 설정 (5 ~ 10 사이의 거리)
        float angle = 0f;
        float radius = Random.Range(meleeMinRadius, meleeMaxRadius); // 5 ~ 10 사이의 랜덤 거리

        // 각 구역에 맞게 각도 범위 지정
        switch (quadrant)
        {
            case 1: // 오른쪽 위 (1사분면)
                angle = Random.Range(0f, Mathf.PI / 2f);
                break;
            case 2: // 왼쪽 위 (2사분면)
                angle = Random.Range(Mathf.PI / 2f, Mathf.PI);
                break;
            case 3: // 왼쪽 아래 (3사분면)
                angle = Random.Range(Mathf.PI, 3 * Mathf.PI / 2f);
                break;
            case 4: // 오른쪽 아래 (4사분면)
                angle = Random.Range(3 * Mathf.PI / 2f, 2 * Mathf.PI);
                break;
        }

        // polar 좌표계에서 Cartesian 좌표로 변환
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y는 0.1로 고정

        return new Vector3(x, y, z);
    }

    public Vector3 GetRangeSpwanPos(int quadrant)
    {
        // 구역에 맞는 범위 설정
        float angle = 0f;
        float radius = Random.Range(rnageMinRadius, rangeMaxRadius); // 10 ~ 15 사이의 랜덤 거리

        // 각 구역에 맞게 각도 범위 지정
        switch (quadrant)
        {
            case 1: // 오른쪽 위 (1사분면)
                angle = Random.Range(0f, Mathf.PI / 2f);
                break;
            case 2: // 왼쪽 위 (2사분면)
                angle = Random.Range(Mathf.PI / 2f, Mathf.PI);
                break;
            case 3: // 왼쪽 아래 (3사분면)
                angle = Random.Range(Mathf.PI, 3 * Mathf.PI / 2f);
                break;
            case 4: // 오른쪽 아래 (4사분면)
                angle = Random.Range(3 * Mathf.PI / 2f, 2 * Mathf.PI);
                break;
        }

        // polar 좌표계에서 Cartesian 좌표로 변환
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y는 0.1으로 고정

        return new Vector3(x, y, z);
    }

    public GameObject MeleeMonsterCreate()
    {
        int r = Random.Range(0, meleeMonsterPrefab.Count);
        GameObject MeleeMonster = Instantiate(meleeMonsterPrefab[r]);
        return MeleeMonster;
    }

    public GameObject RangeMonsterCreate()
    {
        int r = Random.Range(0, rangeMonsterPrefab.Count);
        GameObject RangeMonster = Instantiate(rangeMonsterPrefab[r]);
        return RangeMonster;
    }
}
