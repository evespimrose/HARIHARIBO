using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSpwan : MonoBehaviour
{
    public int wave = 1;
    public int inWave = 1;
    public float SpwanTiem = 10f;

    // ���� ���� ���� ����
    [Header("���� ����")]
    [Tooltip("���� ���� ������ ����")]
    public int meleeMonsterSpwanCount = 5;
    [Tooltip("�ٰŸ� ���� ������")]
    public List<GameObject> meleeMonsterPrefab;

    // ���Ÿ� ���� ���� ����
    [Header("���Ÿ� ����")]
    [Tooltip("���Ÿ� ���� ������ ����")]
    public int rangeMonsterSpwanCount = 2;
    [Tooltip("���Ÿ� ���� ������")]
    public List<GameObject> rangeMonsterPrefab;

    // �߰��� ���� ���� ����
    [Header("����Ʈ ����")]
    [Tooltip("����Ʈ ���� ������ ����")]
    public int eliteMonsterSpwanCount = 1;
    [Tooltip("����Ʈ ���� ������")]
    public List<GameObject> eliteMonsterPrefab;

    [Header("������ ����")]
    [Tooltip("������ ���� ������ ����")]
    public int structureMonsterSpwanCount = 1;
    [Tooltip("������ ���� ������")]
    public List<GameObject> structureMonsterPrefab;

    [Header("���� ����")]
    [Tooltip("���� ���� ������ ����")]
    public int bossMonsterSpwanCount = 1;
    [Tooltip("���� ���� ������")]
    public GameObject bossMonsterPrefab;

    public float meleeMinRadius = 5f;
    public float meleeMaxRadius = 10f;
    public float rnageMinRadius = 10f;
    public float rangeMaxRadius = 15f;
    public Vector3 center;

    private Vector3 bossSpawnPoint = Vector3.zero; // ���� ���� ���� ��ġ

    // ��ųʸ��� �� ���̺� �� ������ ���� ������ ����
    private Dictionary<(int wave, int inWave), (int meleeCount, int rangeCount, int eliteCount, int bossCount, int structureCount)> spawnSettings = new Dictionary<(int wave, int inWave), (int, int, int, int, int)>
    {
        // ���̺� 1
        {(1, 1), (8, 0, 0, 0, 0)},
        {(1, 2), (8, 0, 0, 0, 0)},
        {(1, 3), (8, 0, 0, 0, 0)},
        {(1, 4), (8, 0, 0, 0, 0)},
        
        // ���̺� 2
        {(2, 1), (6, 2, 0, 0, 0)},
        {(2, 2), (6, 2, 0, 0, 0)},
        {(2, 3), (6, 2, 0, 0, 0)},
        {(2, 4), (6, 2, 0, 0, 0)},
        
        // ���̺� 3
        {(3, 1), (4, 2, 0, 0, 10)}, // ������ 10����
        {(3, 2), (6, 2, 0, 0, 10)},
        {(3, 3), (0, 0, 0, 0, 0)}, // 3�� ���̺� ����
        
        // ���̺� 4
        {(4, 1), (4, 2, 0, 0, 10)},
        {(4, 2), (6, 2, 0, 0, 10)},
        {(4, 3), (10, 0, 1, 0, 0)}, // ����Ʈ 1����
        {(4, 4), (5, 2, 0, 0, 10)},
        
        // ���̺� 5
        {(5, 1), (6, 2, 0, 0, 0)},
        {(5, 2), (5, 2, 2, 0, 0)}, // ����Ʈ 2����
        {(5, 3), (4, 2, 0, 0, 10)},
        {(5, 4), (5, 2, 0, 0, 0)},
        
        // ���̺� 6
        {(6, 1), (0, 0, 0, 1, 0)}  // ���� 1����
    };

    private void Awake()
    {
        center = transform.position;
        // ���� ���� ���� ��ġ�� �ʱ�ȭ (�ʿ信 ���� ����)
        bossSpawnPoint = new Vector3(0f, 0f, 20f);
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

        // ���̺� �� ���� �ݺ�
        while (true)
        {
            // ���� ���� ����
            while (true)
            {
                quadrant = Random.Range(1, 5); // ���� ���� ���� (1, 2, 3, 4 ��и�)
                if (lastQuadrant != quadrant || lastQuadrant == 0)
                {
                    lastQuadrant = quadrant;
                    break;
                }
            }

            Vector3 spwanPos;

            // ��ųʸ����� ���� ���̺�� ������ �ش��ϴ� ���� �� ��������
            var currentSpawnSetting = spawnSettings[(wave, inWave)];
            Debug.Log($"Wave {wave}, InWave {inWave}: {currentSpawnSetting.meleeCount}, {currentSpawnSetting.rangeCount}, {currentSpawnSetting.eliteCount}, {currentSpawnSetting.bossCount}, {currentSpawnSetting.structureCount}");

            // ���� ���� ����
            for (int i = 0; i < currentSpawnSetting.meleeCount; i++)
            {
                GameObject MeleeMonster = MeleeMonsterCreate();
                spwanPos = GetMeleeSpwanPos(quadrant);
                MeleeMonster.transform.position = spwanPos;
            }

            // ���Ÿ� ���� ����
            for (int i = 0; i < currentSpawnSetting.rangeCount; i++)
            {
                GameObject RangeMonster = RangeMonsterCreate();
                spwanPos = GetRangeSpwanPos(quadrant);
                RangeMonster.transform.position = spwanPos;
            }

            // ����Ʈ ���� ����
            for (int i = 0; i < currentSpawnSetting.eliteCount; i++)
            {
                GameObject EliteMonster = EliteMonsterCreate();
                spwanPos = GetRangeSpwanPos(quadrant); // ����Ʈ ���ʹ� ���Ÿ� ���Ϳ� ���� ��ġ�� ����
                EliteMonster.transform.position = spwanPos;
            }

            // ������ ���� ���� (���� ���Ϳ� ���� ��ġ)
            for (int i = 0; i < currentSpawnSetting.structureCount; i++)
            {
                GameObject StructureMonster = StructureMonsterCreate();
                spwanPos = GetMeleeSpwanPos(quadrant); // ���� ���Ϳ� ���� ��ġ
                StructureMonster.transform.position = spwanPos;
            }

            // ���� ���� ����
            for (int i = 0; i < currentSpawnSetting.bossCount; i++)
            {
                GameObject BossMonster = BossMonsterCreate();
                BossMonster.transform.position = bossSpawnPoint;  // ���� ���ʹ� ������ ��ġ�� ����
            }
            yield return null;
            // ���� �� ��ٸ��� �κ�
            curTime = 0f;

            // ���� Ÿ�� ���� ��ٸ���, �ʵ忡 ���Ͱ� ��� �׾��� ��� ��� ������ �ٽ� ����
            while (curTime < SpwanTiem)
            {
                // �ʵ忡 ���Ͱ� ��� �׾��� ���, ��� ������ �ٽ� ����
                if (UnitManager.Instance.monsters.Count == 0)
                {
                    Debug.Log("All monsters are dead, respawning...");
                    break;
                }

                curTime += Time.deltaTime;
                yield return null;
            }

            // ������ ������ inWave ����
            inWave++;

            // ��� ������ ������ ���̺긦 �Ѿ (���̺� 6����)
            if (inWave > 4)
            {
                inWave = 1;
                wave++;
                if (wave > 6) // ���̺� 6���� ����
                {
                    wave = 1;
                }
            }
        }
    }

    // ���� ������ ���� ��ġ ���
    public Vector3 GetMeleeSpwanPos(int quadrant)
    {
        float angle = 0f;
        float radius = Random.Range(meleeMinRadius, meleeMaxRadius); // 5 ~ 10 ������ ���� �Ÿ�

        // �� ������ �´� ���� ���� ����
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

        // polar ��ǥ�迡�� Cartesian ��ǥ�� ��ȯ
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y�� 0.1�� ����

        return new Vector3(x, y, z);
    }

    // ���Ÿ� ������ ���� ��ġ ���
    public Vector3 GetRangeSpwanPos(int quadrant)
    {
        float angle = 0f;
        float radius = Random.Range(rnageMinRadius, rangeMaxRadius); // 10 ~ 15 ������ ���� �Ÿ�

        // �� ������ �´� ���� ���� ����
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

        // polar ��ǥ�迡�� Cartesian ��ǥ�� ��ȯ
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y�� 0.1�� ����

        return new Vector3(x, y, z);
    }

    // ���� ���� �Լ��� (������ ���� �޼ҵ�)
    public GameObject MeleeMonsterCreate() => Instantiate(meleeMonsterPrefab[Random.Range(0, meleeMonsterPrefab.Count)]);
    public GameObject RangeMonsterCreate() => Instantiate(rangeMonsterPrefab[Random.Range(0, rangeMonsterPrefab.Count)]);
    public GameObject EliteMonsterCreate() => Instantiate(eliteMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)]);
    public GameObject StructureMonsterCreate() => Instantiate(structureMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)]);
    public GameObject BossMonsterCreate() => Instantiate(bossMonsterPrefab);
}
