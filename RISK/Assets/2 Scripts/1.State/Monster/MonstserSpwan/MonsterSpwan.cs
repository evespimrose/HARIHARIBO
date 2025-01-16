using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSpwan : MonoBehaviour
{
    public int wave = 1;
    public int inWave = 1;
    public float SpwanTiem = 5f;
    [Header("���� ����")]
    [Tooltip("���� ���� ������ ����")]
    public int meleeMonsterSpwanCount = 5;
    [Tooltip("�ٰŸ� ���� ������")]
    public List<GameObject> meleeMonsterPrefab;
    [Header("���Ÿ� ����")]
    [Tooltip("���Ÿ� ���� ������ ����")]
    public int rangeMonsterSpwanCount = 2;
    [Tooltip("���Ÿ� ���� ������")]
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
            //�������� ����
            //�������� ������ ���� (1, 2, 3, 4 ��и�)
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
            //���� ���� ����
            for (int i = 0; i < meleeMonsterSpwanCount; i++)
            {
                GameObject MeleeMonster = MeleeMonsterCreate();
                spwanPos = GetMeleeSpwanPos(quadrant);  // ���� ������ ���� ��ġ
                MeleeMonster.transform.position = spwanPos;
            }
            //���Ÿ� ���� ����
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
        // ���� ���Ϳ� �´� ���� ���� (5 ~ 10 ������ �Ÿ�)
        float angle = 0f;
        float radius = Random.Range(meleeMinRadius, meleeMaxRadius); // 5 ~ 10 ������ ���� �Ÿ�

        // �� ������ �°� ���� ���� ����
        switch (quadrant)
        {
            case 1: // ������ �� (1��и�)
                angle = Random.Range(0f, Mathf.PI / 2f);
                break;
            case 2: // ���� �� (2��и�)
                angle = Random.Range(Mathf.PI / 2f, Mathf.PI);
                break;
            case 3: // ���� �Ʒ� (3��и�)
                angle = Random.Range(Mathf.PI, 3 * Mathf.PI / 2f);
                break;
            case 4: // ������ �Ʒ� (4��и�)
                angle = Random.Range(3 * Mathf.PI / 2f, 2 * Mathf.PI);
                break;
        }

        // polar ��ǥ�迡�� Cartesian ��ǥ�� ��ȯ
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y�� 0.1�� ����

        return new Vector3(x, y, z);
    }

    public Vector3 GetRangeSpwanPos(int quadrant)
    {
        // ������ �´� ���� ����
        float angle = 0f;
        float radius = Random.Range(rnageMinRadius, rangeMaxRadius); // 10 ~ 15 ������ ���� �Ÿ�

        // �� ������ �°� ���� ���� ����
        switch (quadrant)
        {
            case 1: // ������ �� (1��и�)
                angle = Random.Range(0f, Mathf.PI / 2f);
                break;
            case 2: // ���� �� (2��и�)
                angle = Random.Range(Mathf.PI / 2f, Mathf.PI);
                break;
            case 3: // ���� �Ʒ� (3��и�)
                angle = Random.Range(Mathf.PI, 3 * Mathf.PI / 2f);
                break;
            case 4: // ������ �Ʒ� (4��и�)
                angle = Random.Range(3 * Mathf.PI / 2f, 2 * Mathf.PI);
                break;
        }

        // polar ��ǥ�迡�� Cartesian ��ǥ�� ��ȯ
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y�� 0.1���� ����

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
