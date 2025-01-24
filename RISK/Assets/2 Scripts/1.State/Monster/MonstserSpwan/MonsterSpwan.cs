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

    public DungeonBossUI bossUI;

    [Header("Melee monster")]
    [Tooltip("Melee Monster Count")]
    private int meleeMonsterSpwanCount = 5;
    [Tooltip("Melee Monster Prefab")]
    public List<GameObject> meleeMonsterPrefab;

    [Header("Range Monster")]
    [Tooltip("RangeMonster")]
    private int rangeMonsterSpwanCount = 2;
    [Tooltip("Range Monster Prefab")]
    public List<GameObject> rangeMonsterPrefab;

    [Header("Structure Monster")]
    [Tooltip("Structure Monster Count")]
    private int structureMonsterSpwanCount = 1;
    [Tooltip("Structure Monster Prefab")]
    public List<GameObject> structureMonsterPrefab;

    [Header("Elite Monster")]
    [Tooltip("Elite Monster Count")]
    private int eliteMonsterSpwanCount = 1;
    [Tooltip("Elite Monster Prefab")]
    public List<GameObject> eliteMonsterPrefab;

    [Header("Boss Monster")]
    [Tooltip("Boss Monster Count")]
    private int bossMonsterSpwanCount = 1;
    [Tooltip("Boss Monster Prefab")]
    public GameObject bossMonsterPrefab;

    public float meleeMinRadius = 5f;
    public float meleeMaxRadius = 10f;
    public float rangeMinRadius = 10f;
    public float rangeMaxRadius = 15f;
    public Vector3 center;

    private Vector3 bossSpawnPoint = Vector3.zero; //Boss Monster Spwan Point

    //Spawn Table Set
    private Dictionary<(int wave, int inWave), (int meleeCount, int rangeCount, int eliteCount, int bossCount, int structureCount)> spawnSettings = new Dictionary<(int wave, int inWave), (int, int, int, int, int)>
    {
        //1Wave
        {(1, 1), (8, 0, 0, 0, 0)},
        {(1, 2), (8, 0, 0, 0, 0)},
        {(1, 3), (8, 0, 0, 0, 0)},
        {(1, 4), (8, 0, 0, 0, 0)},
        
        //2Wave
        {(2, 1), (6, 2, 0, 0, 0)},
        {(2, 2), (6, 2, 0, 0, 0)},
        {(2, 3), (6, 2, 0, 0, 0)},
        {(2, 4), (6, 2, 0, 0, 0)},
        
        //3Wave
        {(3, 1), (4, 2, 0, 0, 10)}, 
        {(3, 2), (6, 2, 0, 0, 10)},
        {(3, 3), (2, 2, 0, 0, 4)}, 
        {(3, 4), (2, 0, 1, 0, 0)}, 
        
        //4Wave
        {(4, 1), (4, 2, 0, 0, 10)},
        {(4, 2), (6, 2, 0, 0, 10)},
        {(4, 3), (10, 0, 1, 0, 0)}, 
        {(4, 4), (5, 2, 0, 0, 10)},
        
        //5Wave
        {(5, 1), (6, 2, 0, 0, 0)},
        {(5, 2), (5, 2, 2, 0, 0)},
        {(5, 3), (4, 2, 0, 0, 10)},
        {(5, 4), (5, 2, 0, 0, 0)},
        
        //6Wave
        {(6, 1), (0, 0, 0, 1, 0)}  
    };

    private void Awake()
    {
        center = transform.position;
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

        while (true)
        {
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

            var currentSpawnSetting = spawnSettings[(wave, inWave)];
            Debug.Log($"Wave {wave}, InWave {inWave}: {currentSpawnSetting.meleeCount}, {currentSpawnSetting.rangeCount}, {currentSpawnSetting.eliteCount}, {currentSpawnSetting.bossCount}, {currentSpawnSetting.structureCount}");

            for (int i = 0; i < currentSpawnSetting.meleeCount; i++)
            {
                spwanPos = GetMeleeSpwanPos(quadrant);
                GameObject MeleeMonster = MeleeMonsterCreate(spwanPos);
            }

            for (int i = 0; i < currentSpawnSetting.rangeCount; i++)
            {
                spwanPos = GetRangeSpwanPos(quadrant);
                GameObject RangeMonster = RangeMonsterCreate(spwanPos);
            }

            for (int i = 0; i < currentSpawnSetting.eliteCount; i++)
            {
                spwanPos = GetRangeSpwanPos(quadrant); 
                GameObject EliteMonster = EliteMonsterCreate(spwanPos);
            }

            for (int i = 0; i < currentSpawnSetting.structureCount; i++)
            {
                spwanPos = GetMeleeSpwanPos(quadrant); 
                GameObject StructureMonster = StructureMonsterCreate(spwanPos);
            }

            for (int i = 0; i < currentSpawnSetting.bossCount; i++)
            {
                GameObject BossMonster = BossMonsterCreate(bossSpawnPoint);
            }
            yield return null;
            curTime = 0f;

            while (curTime < SpwanTiem)
            {
                if (UnitManager.Instance.monsters.Count == 0)
                {
                    Debug.Log("All monsters are dead, respawning...");
                    break;
                }

                curTime += Time.deltaTime;
                yield return null;
            }

            inWave++;

            if (inWave > 4 || wave >= 6)
            {
                if (wave >= 6)
                {
                    wave = 1;
                    inWave = 1;
                    GameManager.Instance.isWaveDone = true;
                    yield break;
                }
                wave++;
                inWave = 1;
            }
        }
    }

    public Vector3 GetMeleeSpwanPos(int quadrant)
    {
        float angle = 0f;
        float radius = Random.Range(meleeMinRadius, meleeMaxRadius); 

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

        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; 

        return new Vector3(x, y, z);
    }

    public Vector3 GetRangeSpwanPos(int quadrant)
    {
        float angle = 0f;
        float radius = Random.Range(rangeMinRadius, rangeMaxRadius); 

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

        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; 

        return new Vector3(x, y, z);
    }

    public GameObject MeleeMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(meleeMonsterPrefab[Random.Range(0, meleeMonsterPrefab.Count)].name, spwanPos, Quaternion.identity);
    public GameObject RangeMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(rangeMonsterPrefab[Random.Range(0, rangeMonsterPrefab.Count)].name, spwanPos, Quaternion.identity);
    public GameObject EliteMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(eliteMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)].name, spwanPos, Quaternion.identity);
    public GameObject StructureMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(structureMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)].name, spwanPos, structureMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)].transform.rotation);
    public GameObject BossMonsterCreate(Vector3 spawnPos)
    {
        GameObject bossMonster = PhotonNetwork.Instantiate(bossMonsterPrefab.name, spawnPos, Quaternion.identity);

        bossUI.gameObject.SetActive(true);
        bossUI.bossMonster = bossMonster.GetComponent<BossMonster>();

        return bossMonster;
    }
}
