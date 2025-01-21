using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSpwan : MonoBehaviour
{
    public int wave = 1;
    public int inWave = 1;
    public float SpwanTiem = 10f;

    // 근접 몬스터 관련 변수
    [Header("근접 몬스터")]
    [Tooltip("근접 몬스터 스폰될 갯수")]
    private int meleeMonsterSpwanCount = 5;
    [Tooltip("근거리 몬스터 프리팹")]
    public List<GameObject> meleeMonsterPrefab;

    // 원거리 몬스터 관련 변수
    [Header("원거리 몬스터")]
    [Tooltip("원거리 몬스터 스폰될 갯수")]
    private int rangeMonsterSpwanCount = 2;
    [Tooltip("원거리 몬스터 프리팹")]
    public List<GameObject> rangeMonsterPrefab;

    // 추가된 몬스터 관련 변수
    [Header("구조물 몬스터")]
    [Tooltip("구조물 몬스터 스폰될 갯수")]
    private int structureMonsterSpwanCount = 1;
    [Tooltip("구조물 몬스터 프리팹")]
    public List<GameObject> structureMonsterPrefab;

    [Header("엘리트 몬스터")]
    [Tooltip("엘리트 몬스터 스폰될 갯수")]
    private int eliteMonsterSpwanCount = 1;
    [Tooltip("엘리트 몬스터 프리팹")]
    public List<GameObject> eliteMonsterPrefab;

    [Header("보스 몬스터")]
    [Tooltip("보스 몬스터 스폰될 갯수")]
    private int bossMonsterSpwanCount = 1;
    [Tooltip("보스 몬스터 프리팹")]
    public GameObject bossMonsterPrefab;

    public float meleeMinRadius = 5f;
    public float meleeMaxRadius = 10f;
    public float rnageMinRadius = 10f;
    public float rangeMaxRadius = 15f;
    public Vector3 center;

    private Vector3 bossSpawnPoint = Vector3.zero; // 보스 몬스터 스폰 위치

    // 딕셔너리로 각 웨이브 및 스폰에 대한 설정을 저장
    private Dictionary<(int wave, int inWave), (int meleeCount, int rangeCount, int eliteCount, int bossCount, int structureCount)> spawnSettings = new Dictionary<(int wave, int inWave), (int, int, int, int, int)>
    {
        // 웨이브 1
        {(1, 1), (8, 0, 0, 0, 0)},
        {(1, 2), (8, 0, 0, 0, 0)},
        {(1, 3), (8, 0, 0, 0, 0)},
        {(1, 4), (8, 0, 0, 0, 0)},
        
        // 웨이브 2
        {(2, 1), (6, 2, 0, 0, 0)},
        {(2, 2), (6, 2, 0, 0, 0)},
        {(2, 3), (6, 2, 0, 0, 0)},
        {(2, 4), (6, 2, 0, 0, 0)},
        
        // 웨이브 3
        {(3, 1), (4, 2, 0, 0, 10)}, // 구조물 10마리
        {(3, 2), (6, 2, 0, 0, 10)},
        {(3, 3), (0, 0, 0, 0, 0)}, // 3번 웨이브 종료
        
        // 웨이브 4
        {(4, 1), (4, 2, 0, 0, 10)},
        {(4, 2), (6, 2, 0, 0, 10)},
        {(4, 3), (10, 0, 1, 0, 0)}, // 엘리트 1마리
        {(4, 4), (5, 2, 0, 0, 10)},
        
        // 웨이브 5
        {(5, 1), (6, 2, 0, 0, 0)},
        {(5, 2), (5, 2, 2, 0, 0)}, // 엘리트 2마리
        {(5, 3), (4, 2, 0, 0, 10)},
        {(5, 4), (5, 2, 0, 0, 0)},
        
        // 웨이브 6
        {(6, 1), (0, 0, 0, 1, 0)}  // 보스 1마리
    };

    private void Awake()
    {
        center = transform.position;
        // 보스 몬스터 스폰 위치를 초기화 (필요에 따라 조정)
        bossSpawnPoint = new Vector3(0f, 0f, 20f);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void SpawnStart()
    {
        StartCoroutine(MonsterSpwanCorutine());
    }

    public IEnumerator MonsterSpwanCorutine()
    {
        int lastQuadrant = 0;
        int quadrant = 0;
        float curTime = 0f;

        // 웨이브 및 스폰 반복
        while (true)
        {
            // 스폰 구역 선택
            while (true)
            {
                quadrant = Random.Range(1, 5); // 랜덤 구역 선택 (1, 2, 3, 4 사분면)
                if (lastQuadrant != quadrant || lastQuadrant == 0)
                {
                    lastQuadrant = quadrant;
                    break;
                }
            }

            Vector3 spwanPos;

            // 딕셔너리에서 현재 웨이브와 스폰에 해당하는 몬스터 수 가져오기
            var currentSpawnSetting = spawnSettings[(wave, inWave)];
            Debug.Log($"Wave {wave}, InWave {inWave}: {currentSpawnSetting.meleeCount}, {currentSpawnSetting.rangeCount}, {currentSpawnSetting.eliteCount}, {currentSpawnSetting.bossCount}, {currentSpawnSetting.structureCount}");

            // 근접 몬스터 스폰
            for (int i = 0; i < currentSpawnSetting.meleeCount; i++)
            {
                spwanPos = GetMeleeSpwanPos(quadrant);
                GameObject MeleeMonster = MeleeMonsterCreate(spwanPos);
            }

            // 원거리 몬스터 스폰
            for (int i = 0; i < currentSpawnSetting.rangeCount; i++)
            {
                spwanPos = GetRangeSpwanPos(quadrant);
                GameObject RangeMonster = RangeMonsterCreate(spwanPos);
            }

            // 엘리트 몬스터 스폰
            for (int i = 0; i < currentSpawnSetting.eliteCount; i++)
            {
                spwanPos = GetRangeSpwanPos(quadrant); // 엘리트 몬스터는 원거리 몬스터와 같은 위치로 스폰
                GameObject EliteMonster = EliteMonsterCreate(spwanPos);
            }

            // 구조물 몬스터 스폰 (근접 몬스터와 같은 위치)
            for (int i = 0; i < currentSpawnSetting.structureCount; i++)
            {
                spwanPos = GetMeleeSpwanPos(quadrant); // 근접 몬스터와 동일 위치
                GameObject StructureMonster = StructureMonsterCreate(spwanPos);
            }

            // 보스 몬스터 스폰
            for (int i = 0; i < currentSpawnSetting.bossCount; i++)
            {
                GameObject BossMonster = BossMonsterCreate(bossSpawnPoint);
            }
            yield return null;
            // 스폰 후 기다리는 부분
            curTime = 0f;

            // 스폰 타임 동안 기다리되, 필드에 몬스터가 모두 죽었을 경우 즉시 스폰을 다시 시작
            while (curTime < SpwanTiem)
            {
                // 필드에 몬스터가 모두 죽었을 경우, 즉시 스폰을 다시 시작
                if (UnitManager.Instance.monsters.Count == 0)
                {
                    Debug.Log("All monsters are dead, respawning...");
                    break;
                }

                curTime += Time.deltaTime;
                yield return null;
            }

            // 스폰이 끝나면 inWave 증가
            inWave++;

            // 모든 스폰이 끝나면 웨이브를 넘어감 (웨이브 6까지)
            if (inWave > 4)
            {
                inWave = 1;
                wave++;
                if (wave > 6) // 웨이브 6까지 설정
                {
                    yield break;
                }
            }
        }
    }

    // 근접 몬스터의 스폰 위치 계산
    public Vector3 GetMeleeSpwanPos(int quadrant)
    {
        float angle = 0f;
        float radius = Random.Range(meleeMinRadius, meleeMaxRadius); // 5 ~ 10 사이의 랜덤 거리

        // 각 구역에 맞는 각도 범위 지정
        switch (quadrant)
        {
            case 1:
                angle = Random.Range(0f, Mathf.PI / 2f);
                break;
            case 2:
                angle = Random.Range(Mathf.PI / 2f, Mathf.PI);
                break;
            case 3:
                angle = Random.Range(Mathf.PI, 3 * Mathf.PI / 2f);
                break;
            case 4:
                angle = Random.Range(3 * Mathf.PI / 2f, 2 * Mathf.PI);
                break;
        }

        // polar 좌표계에서 Cartesian 좌표로 변환
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y는 0.1로 고정

        return new Vector3(x, y, z);
    }

    // 원거리 몬스터의 스폰 위치 계산
    public Vector3 GetRangeSpwanPos(int quadrant)
    {
        float angle = 0f;
        float radius = Random.Range(rnageMinRadius, rangeMaxRadius); // 10 ~ 15 사이의 랜덤 거리

        // 각 구역에 맞는 각도 범위 지정
        switch (quadrant)
        {
            case 1:
                angle = Random.Range(0f, Mathf.PI / 2f);
                break;
            case 2:
                angle = Random.Range(Mathf.PI / 2f, Mathf.PI);
                break;
            case 3:
                angle = Random.Range(Mathf.PI, 3 * Mathf.PI / 2f);
                break;
            case 4:
                angle = Random.Range(3 * Mathf.PI / 2f, 2 * Mathf.PI);
                break;
        }

        // polar 좌표계에서 Cartesian 좌표로 변환
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y는 0.1로 고정

        return new Vector3(x, y, z);
    }

    // 몬스터 생성 함수들 (프리팹 생성 메소드)
    public GameObject MeleeMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(meleeMonsterPrefab[Random.Range(0, meleeMonsterPrefab.Count)].name, spwanPos, Quaternion.identity);
    public GameObject RangeMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(rangeMonsterPrefab[Random.Range(0, rangeMonsterPrefab.Count)].name, spwanPos, Quaternion.identity);
    public GameObject EliteMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(eliteMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)].name, spwanPos, Quaternion.identity);
    public GameObject StructureMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(structureMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)].name, spwanPos, structureMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)].transform.rotation);
    public GameObject BossMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(bossMonsterPrefab.name, spwanPos, Quaternion.identity);
}
