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

    // 洹쇱젒 紐ъ뒪??愿??蹂??
    [Header("洹쇱젒 紐ъ뒪??")]
    [Tooltip("洹쇱젒 紐ъ뒪???ㅽ룿??媛?닔")]
    private int meleeMonsterSpwanCount = 5;
    [Tooltip("洹쇨굅由?紐ъ뒪???꾨━??")]
    public List<GameObject> meleeMonsterPrefab;

    // ?먭굅由?紐ъ뒪??愿??蹂??
    [Header("?먭굅由?紐ъ뒪??")]
    [Tooltip("?먭굅由?紐ъ뒪???ㅽ룿??媛?닔")]
    private int rangeMonsterSpwanCount = 2;
    [Tooltip("?먭굅由?紐ъ뒪???꾨━??")]
    public List<GameObject> rangeMonsterPrefab;

    // 異붽???紐ъ뒪??愿??蹂??
    [Header("援ъ“臾?紐ъ뒪??")]
    [Tooltip("援ъ“臾?紐ъ뒪???ㅽ룿??媛?닔")]
    private int structureMonsterSpwanCount = 1;
    [Tooltip("援ъ“臾?紐ъ뒪???꾨━??")]
    public List<GameObject> structureMonsterPrefab;

    [Header("?섎━??紐ъ뒪??")]
    [Tooltip("?섎━??紐ъ뒪???ㅽ룿??媛?닔")]
    private int eliteMonsterSpwanCount = 1;
    [Tooltip("?섎━??紐ъ뒪???꾨━??")]
    public List<GameObject> eliteMonsterPrefab;

    [Header("蹂댁뒪 紐ъ뒪??")]
    [Tooltip("蹂댁뒪 紐ъ뒪???ㅽ룿??媛?닔")]
    private int bossMonsterSpwanCount = 1;
    [Tooltip("蹂댁뒪 紐ъ뒪???꾨━??")]
    public GameObject bossMonsterPrefab;

    public float meleeMinRadius = 5f;
    public float meleeMaxRadius = 10f;
    public float rnageMinRadius = 10f;
    public float rangeMaxRadius = 15f;
    public Vector3 center;

    private Vector3 bossSpawnPoint = Vector3.zero; // 蹂댁뒪 紐ъ뒪???ㅽ룿 ?꾩튂

    // ?뺤뀛?덈━濡?媛??⑥씠釉?諛??ㅽ룿??????ㅼ젙?????
    private Dictionary<(int wave, int inWave), (int meleeCount, int rangeCount, int eliteCount, int bossCount, int structureCount)> spawnSettings = new Dictionary<(int wave, int inWave), (int, int, int, int, int)>
    {
        // ?⑥씠釉?1
        {(1, 1), (8, 0, 0, 0, 0)},
        {(1, 2), (8, 0, 0, 0, 0)},
        {(1, 3), (8, 0, 0, 0, 0)},
        {(1, 4), (8, 0, 0, 0, 0)},
        
        // ?⑥씠釉?2
        {(2, 1), (6, 2, 0, 0, 0)},
        {(2, 2), (6, 2, 0, 0, 0)},
        {(2, 3), (6, 2, 0, 0, 0)},
        {(2, 4), (6, 2, 0, 0, 0)},
        
        // ?⑥씠釉?3
        {(3, 1), (4, 2, 0, 0, 10)}, // 援ъ“臾?10留덈━
        {(3, 2), (6, 2, 0, 0, 10)},
        {(3, 3), (0, 0, 0, 0, 0)}, // 3踰??⑥씠釉?醫낅즺
        {(3, 3), (0, 0, 0, 0, 0)}, // 3甕???μ뵠???ル굝利?
        {(3, 4), (0, 0, 0, 0, 0)},
        
        // ?⑥씠釉?4
        {(4, 1), (4, 2, 0, 0, 10)},
        {(4, 2), (6, 2, 0, 0, 10)},
        {(4, 3), (10, 0, 1, 0, 0)}, // ?섎━??1留덈━
        {(4, 4), (5, 2, 0, 0, 10)},
        
        // ?⑥씠釉?5
        {(5, 1), (6, 2, 0, 0, 0)},
        {(5, 2), (5, 2, 2, 0, 0)}, // ?섎━??2留덈━
        {(5, 3), (4, 2, 0, 0, 10)},
        {(5, 4), (5, 2, 0, 0, 0)},
        
        // ?⑥씠釉?6
        {(6, 1), (0, 0, 0, 1, 0)}  // 蹂댁뒪 1留덈━
    };

    private void Awake()
    {
        center = transform.position;
        // 蹂댁뒪 紐ъ뒪???ㅽ룿 ?꾩튂瑜?珥덇린??(?꾩슂???곕씪 議곗젙)
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

        // ?⑥씠釉?諛??ㅽ룿 諛섎났
        while (true)
        {
            // ?ㅽ룿 援ъ뿭 ?좏깮
            while (true)
            {
                quadrant = Random.Range(1, 5); // ?쒕뜡 援ъ뿭 ?좏깮 (1, 2, 3, 4 ?щ텇硫?
                if (lastQuadrant != quadrant || lastQuadrant == 0)
                {
                    lastQuadrant = quadrant;
                    break;
                }
            }

            Vector3 spwanPos;

            // ?뺤뀛?덈━?먯꽌 ?꾩옱 ?⑥씠釉뚯? ?ㅽ룿???대떦?섎뒗 紐ъ뒪????媛?몄삤湲?

            var currentSpawnSetting = spawnSettings[(wave, inWave)];
            Debug.Log($"Wave {wave}, InWave {inWave}: {currentSpawnSetting.meleeCount}, {currentSpawnSetting.rangeCount}, {currentSpawnSetting.eliteCount}, {currentSpawnSetting.bossCount}, {currentSpawnSetting.structureCount}");

            // 洹쇱젒 紐ъ뒪???ㅽ룿
            for (int i = 0; i < currentSpawnSetting.meleeCount; i++)
            {
                spwanPos = GetMeleeSpwanPos(quadrant);
                GameObject MeleeMonster = MeleeMonsterCreate(spwanPos);
            }

            // ?먭굅由?紐ъ뒪???ㅽ룿
            for (int i = 0; i < currentSpawnSetting.rangeCount; i++)
            {
                spwanPos = GetRangeSpwanPos(quadrant);
                GameObject RangeMonster = RangeMonsterCreate(spwanPos);
            }

            // ?섎━??紐ъ뒪???ㅽ룿
            for (int i = 0; i < currentSpawnSetting.eliteCount; i++)
            {
                spwanPos = GetRangeSpwanPos(quadrant); // ?섎━??紐ъ뒪?곕뒗 ?먭굅由?紐ъ뒪?곗? 媛숈? ?꾩튂濡??ㅽ룿
                GameObject EliteMonster = EliteMonsterCreate(spwanPos);
            }

            // 援ъ“臾?紐ъ뒪???ㅽ룿 (洹쇱젒 紐ъ뒪?곗? 媛숈? ?꾩튂)
            for (int i = 0; i < currentSpawnSetting.structureCount; i++)
            {
                spwanPos = GetMeleeSpwanPos(quadrant); // 洹쇱젒 紐ъ뒪?곗? ?숈씪 ?꾩튂
                GameObject StructureMonster = StructureMonsterCreate(spwanPos);
            }

            // 蹂댁뒪 紐ъ뒪???ㅽ룿
            for (int i = 0; i < currentSpawnSetting.bossCount; i++)
            {
                GameObject BossMonster = BossMonsterCreate(bossSpawnPoint);
            }
            yield return null;
            // ?ㅽ룿 ??湲곕떎由щ뒗 遺遺?
            curTime = 0f;

            // ?ㅽ룿 ????숈븞 湲곕떎由щ릺, ?꾨뱶??紐ъ뒪?곌? 紐⑤몢 二쎌뿀??寃쎌슦 利됱떆 ?ㅽ룿???ㅼ떆 ?쒖옉
            while (curTime < SpwanTiem)
            {
                // ?꾨뱶??紐ъ뒪?곌? 紐⑤몢 二쎌뿀??寃쎌슦, 利됱떆 ?ㅽ룿???ㅼ떆 ?쒖옉
                if (UnitManager.Instance.monsters.Count == 0)
                {
                    Debug.Log("All monsters are dead, respawning...");
                    break;
                }

                curTime += Time.deltaTime;
                yield return null;
            }

            // ?ㅽ룿???앸굹硫?inWave 利앷?
            inWave++;

            // 紐⑤뱺 ?ㅽ룿???앸굹硫??⑥씠釉뚮? ?섏뼱媛?(?⑥씠釉?6源뚯?)
            if (inWave > 4)
            {
                inWave = 1;
                wave++;
                if (wave >= 6) // ?⑥씠釉?6源뚯? ?ㅼ젙
                {
                    yield break;
                }
            }
        }
    }

    // 洹쇱젒 紐ъ뒪?곗쓽 ?ㅽ룿 ?꾩튂 怨꾩궛
    public Vector3 GetMeleeSpwanPos(int quadrant)
    {
        float angle = 0f;
        float radius = Random.Range(meleeMinRadius, meleeMaxRadius); // 5 ~ 10 ?ъ씠???쒕뜡 嫄곕━

        // 媛?援ъ뿭??留욌뒗 媛곷룄 踰붿쐞 吏??
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

        // polar 醫뚰몴怨꾩뿉??Cartesian 醫뚰몴濡?蹂??
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y??0.1濡?怨좎젙

        return new Vector3(x, y, z);
    }

    // ?먭굅由?紐ъ뒪?곗쓽 ?ㅽ룿 ?꾩튂 怨꾩궛
    public Vector3 GetRangeSpwanPos(int quadrant)
    {
        float angle = 0f;
        float radius = Random.Range(rnageMinRadius, rangeMaxRadius); // 10 ~ 15 ?ъ씠???쒕뜡 嫄곕━

        // 媛?援ъ뿭??留욌뒗 媛곷룄 踰붿쐞 吏??
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

        // polar 醫뚰몴怨꾩뿉??Cartesian 醫뚰몴濡?蹂??
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        float y = 0.1f; // y??0.1濡?怨좎젙

        return new Vector3(x, y, z);
    }

    // 紐ъ뒪???앹꽦 ?⑥닔??(?꾨━???앹꽦 硫붿냼??
    public GameObject MeleeMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(meleeMonsterPrefab[Random.Range(0, meleeMonsterPrefab.Count)].name, spwanPos, Quaternion.identity);
    public GameObject RangeMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(rangeMonsterPrefab[Random.Range(0, rangeMonsterPrefab.Count)].name, spwanPos, Quaternion.identity);
    public GameObject EliteMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(eliteMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)].name, spwanPos, Quaternion.identity);
    public GameObject StructureMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(structureMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)].name, spwanPos, structureMonsterPrefab[Random.Range(0, eliteMonsterPrefab.Count)].transform.rotation);
    public GameObject BossMonsterCreate(Vector3 spwanPos) => PhotonNetwork.Instantiate(bossMonsterPrefab.name, spwanPos, Quaternion.identity);
}
