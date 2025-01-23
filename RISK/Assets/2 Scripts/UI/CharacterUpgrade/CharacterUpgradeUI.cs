using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatsSet
{
    [Tooltip("스탯 이름")]
    public TextMeshProUGUI statsName;  // 스탯의 이름을 표시하는 UI 텍스트
    [Tooltip("현재 스탯 값")]
    public TextMeshProUGUI currentStats;  // 현재 스탯 값을 표시하는 UI 텍스트
    [Tooltip("스탯 업그레이드 값")]
    public TextMeshProUGUI statsUpgradeValue;  // 스탯 업그레이드 값을 표시하는 UI 텍스트
    [Tooltip("성공 확률")]
    public TextMeshProUGUI successChance;  // 업그레이드 성공 확률을 표시하는 UI 텍스트
    [Tooltip("리소스 비용")]
    public TextMeshProUGUI resourceCost;  // 업그레이드에 필요한 비용을 표시하는 UI 텍스트
    [Tooltip("업그레이드 버튼")]
    public Button upgradeButton;  // 업그레이드를 위한 버튼

    // UI를 업데이트하는 함수
    public void UpdateUI(string name, float current, float upgradeValue, float chance, int cost)
    {
        statsName.text = name;  // 스탯 이름 업데이트
        currentStats.text = $"현재 스탯 : {current}";  // 현재 스탯 값 표시
        statsUpgradeValue.text = $"업그레이드 값: +{upgradeValue}";  // 업그레이드 값 표시
        successChance.text = $"성공 확률 : {chance * 100}%";  // 성공 확률 표시
        resourceCost.text = $"강화 비용 : {cost}";  // 리소스 비용 표시
    }
}

[System.Serializable]
public class UpgradeData
{
    public float upProb;  // 업그레이드 성공 확률
    public int useWon;    // 업그레이드 비용 (원)
    public float statIncrease;  // 스탯 증가 값

    // 업그레이드 데이터를 초기화하는 생성자
    public UpgradeData(float upProb, int useWon, float statIncrease)
    {
        this.upProb = upProb;
        this.useWon = useWon;
        this.statIncrease = statIncrease;
    }
}

public class CharacterUpgradeUI : MonoBehaviour
{
    [Header("Character Info"), SerializeField]
    public TextMeshProUGUI characterNameText;  // 캐릭터 이름을 표시하는 UI 텍스트
    public TextMeshProUGUI levelText;  // 캐릭터 레벨을 표시하는 UI 텍스트
    public TextMeshProUGUI curGold;  // 현재 보유한 골드를 표시하는 UI 텍스트

    [Header("Stats Sets"), SerializeField]
    private List<StatsSet> statSets;  // 여러 스탯들을 UI에 업데이트할 리스트

    [Header("Upgrade Data")]
    [SerializeField] private Dictionary<int, UpgradeData> maxHpUpgradeData = new Dictionary<int, UpgradeData>();  // 체력 업그레이드 데이터
    [SerializeField] private Dictionary<int, UpgradeData> atkUpgradeData = new Dictionary<int, UpgradeData>();  // 공격력 업그레이드 데이터
    [SerializeField] private Dictionary<int, UpgradeData> criUpgradeData = new Dictionary<int, UpgradeData>();  // 치명타 확률 업그레이드 데이터
    [SerializeField] private Dictionary<int, UpgradeData> criDmgUpgradeData = new Dictionary<int, UpgradeData>();  // 치명타 데미지 업그레이드 데이터
    [SerializeField] private Dictionary<int, UpgradeData> hpRegUpgradeData = new Dictionary<int, UpgradeData>();  // 체력 회복 업그레이드 데이터
    [SerializeField] private Dictionary<int, UpgradeData> coolRedUpgradeData = new Dictionary<int, UpgradeData>();  // 쿨타임 감소 업그레이드 데이터

    private int currentGold;  // 현재 보유한 골드

    // 업그레이드 데이터를 초기화하는 함수
    private void InitializeUpgradeData()
    {
        //maxHp 스탯 업그레이드 데이터 초기화
        maxHpUpgradeData.Add(0, new UpgradeData(1f, 2000, 5));
        maxHpUpgradeData.Add(1, new UpgradeData(0.99f, 5200, 5));
        maxHpUpgradeData.Add(2, new UpgradeData(0.96f, 10800, 5));
        maxHpUpgradeData.Add(3, new UpgradeData(0.91f, 18800, 5));
        maxHpUpgradeData.Add(4, new UpgradeData(0.9f, 29200, 5));
        maxHpUpgradeData.Add(5, new UpgradeData(0.85f, 42000, 10));
        maxHpUpgradeData.Add(6, new UpgradeData(0.78f, 57200, 10));
        maxHpUpgradeData.Add(7, new UpgradeData(0.71f, 74800, 10));
        maxHpUpgradeData.Add(8, new UpgradeData(0.8f, 368400, 20));
        maxHpUpgradeData.Add(9, new UpgradeData(0.79f, 497200, 20));
        maxHpUpgradeData.Add(10, new UpgradeData(0.76f, 652400, 20));
        maxHpUpgradeData.Add(11, new UpgradeData(0.71f, 836400, 20));
        maxHpUpgradeData.Add(12, new UpgradeData(0.68f, 1051600, 20));
        maxHpUpgradeData.Add(13, new UpgradeData(0.62f, 1300400, 25));
        maxHpUpgradeData.Add(14, new UpgradeData(0.54f, 1585200, 25));
        maxHpUpgradeData.Add(15, new UpgradeData(0.45f, 1908400, 25));
        maxHpUpgradeData.Add(16, new UpgradeData(0.50f, 14091400, 30));
        maxHpUpgradeData.Add(17, new UpgradeData(0.49f, 15164400, 30));
        maxHpUpgradeData.Add(18, new UpgradeData(0.46f, 16290200, 30));
        maxHpUpgradeData.Add(19, new UpgradeData(0.41f, 17470000, 40));
        maxHpUpgradeData.Add(20, new UpgradeData(0.42f, 18705000, 40));
        maxHpUpgradeData.Add(21, new UpgradeData(0.38f, 19996400, 40));
        maxHpUpgradeData.Add(22, new UpgradeData(0.33f, 21345400, 50));
        maxHpUpgradeData.Add(23, new UpgradeData(0.26f, 22753200, 50));
        maxHpUpgradeData.Add(24, new UpgradeData(0.03f, 24221000, 80));
        maxHpUpgradeData.Add(25, new UpgradeData(0.03f, 25750000, 100));

        //atk 스탯 업그레이드 데이터 초기화
        atkUpgradeData.Add(0, new UpgradeData(1f, 2000, 4));
        atkUpgradeData.Add(1, new UpgradeData(0.99f, 5200, 4));
        atkUpgradeData.Add(2, new UpgradeData(0.96f, 10800, 4));
        atkUpgradeData.Add(3, new UpgradeData(0.91f, 18800, 4));
        atkUpgradeData.Add(4, new UpgradeData(0.9f, 29200, 6));
        atkUpgradeData.Add(5, new UpgradeData(0.85f, 42000, 6));
        atkUpgradeData.Add(6, new UpgradeData(0.78f, 57200, 6));
        atkUpgradeData.Add(7, new UpgradeData(0.71f, 74800, 7));
        atkUpgradeData.Add(8, new UpgradeData(0.8f, 368400, 9));
        atkUpgradeData.Add(9, new UpgradeData(0.79f, 497200, 9));
        atkUpgradeData.Add(10, new UpgradeData(0.76f, 652400, 11));
        atkUpgradeData.Add(11, new UpgradeData(0.71f, 836400, 11));
        atkUpgradeData.Add(12, new UpgradeData(0.68f, 1051600, 11));
        atkUpgradeData.Add(13, new UpgradeData(0.62f, 1300400, 11));
        atkUpgradeData.Add(14, new UpgradeData(0.54f, 1585200, 14));
        atkUpgradeData.Add(15, new UpgradeData(0.45f, 1908400, 14));
        atkUpgradeData.Add(16, new UpgradeData(0.50f, 14091400, 17));
        atkUpgradeData.Add(17, new UpgradeData(0.49f, 15164400, 17));
        atkUpgradeData.Add(18, new UpgradeData(0.46f, 16290200, 17));
        atkUpgradeData.Add(19, new UpgradeData(0.41f, 17470000, 18));
        atkUpgradeData.Add(20, new UpgradeData(0.42f, 18705000, 19));
        atkUpgradeData.Add(21, new UpgradeData(0.38f, 19996400, 20));
        atkUpgradeData.Add(22, new UpgradeData(0.33f, 21345400, 22));
        atkUpgradeData.Add(23, new UpgradeData(0.26f, 22753200, 24));
        atkUpgradeData.Add(24, new UpgradeData(0.03f, 24221000, 26));
        atkUpgradeData.Add(25, new UpgradeData(0.03f, 25750000, 29));

        //cri 스탯 업그레이드 데이터 초기화
        criUpgradeData.Add(0, new UpgradeData(1f, 2000, 0.01f));
        criUpgradeData.Add(1, new UpgradeData(0.99f, 5200, 0.01f));
        criUpgradeData.Add(2, new UpgradeData(0.96f, 10800, 0.01f));
        criUpgradeData.Add(3, new UpgradeData(0.91f, 18800, 0.01f));
        criUpgradeData.Add(4, new UpgradeData(0.9f, 29200, 0.01f));
        criUpgradeData.Add(5, new UpgradeData(0.85f, 42000, 0.015f));
        criUpgradeData.Add(6, new UpgradeData(0.78f, 57200, 0.015f));
        criUpgradeData.Add(7, new UpgradeData(0.71f, 74800, 0.015f));
        criUpgradeData.Add(8, new UpgradeData(0.8f, 368400, 0.02f));
        criUpgradeData.Add(9, new UpgradeData(0.79f, 497200, 0.02f));
        criUpgradeData.Add(10, new UpgradeData(0.76f, 652400, 0.02f));
        criUpgradeData.Add(11, new UpgradeData(0.71f, 836400, 0.02f));
        criUpgradeData.Add(12, new UpgradeData(0.68f, 1051600, 0.024f));
        criUpgradeData.Add(13, new UpgradeData(0.62f, 1300400, 0.024f));
        criUpgradeData.Add(14, new UpgradeData(0.54f, 1585200, 0.024f));
        criUpgradeData.Add(15, new UpgradeData(0.45f, 1908400, 0.024f));
        criUpgradeData.Add(16, new UpgradeData(0.50f, 14091400, 0.031f));
        criUpgradeData.Add(17, new UpgradeData(0.49f, 15164400, 0.031f));
        criUpgradeData.Add(18, new UpgradeData(0.46f, 16290200, 0.039f));
        criUpgradeData.Add(19, new UpgradeData(0.41f, 17470000, 0.046f));
        criUpgradeData.Add(20, new UpgradeData(0.42f, 18705000, 0.046f));
        criUpgradeData.Add(21, new UpgradeData(0.38f, 19996400, 0.051f));
        criUpgradeData.Add(22, new UpgradeData(0.33f, 21345400, 0.051f));
        criUpgradeData.Add(23, new UpgradeData(0.26f, 22753200, 0.055f));
        criUpgradeData.Add(24, new UpgradeData(0.03f, 24221000, 0.06f));
        criUpgradeData.Add(25, new UpgradeData(0.03f, 25750000, 0.07f));

        //criDmg 스탯 업그레이드 데이터 초기화
        criDmgUpgradeData.Add(0, new UpgradeData(1f, 2000, 0.005f));
        criDmgUpgradeData.Add(1, new UpgradeData(0.99f, 5200, 0.005f));
        criDmgUpgradeData.Add(2, new UpgradeData(0.96f, 10800, 0.005f));
        criDmgUpgradeData.Add(3, new UpgradeData(0.91f, 18800, 0.005f));
        criDmgUpgradeData.Add(4, new UpgradeData(0.9f, 29200, 0.005f));
        criDmgUpgradeData.Add(5, new UpgradeData(0.85f, 42000, 0.005f));
        criDmgUpgradeData.Add(6, new UpgradeData(0.78f, 57200, 0.01f));
        criDmgUpgradeData.Add(7, new UpgradeData(0.71f, 74800, 0.01f));
        criDmgUpgradeData.Add(8, new UpgradeData(0.8f, 368400, 0.02f));
        criDmgUpgradeData.Add(9, new UpgradeData(0.79f, 497200, 0.02f));
        criDmgUpgradeData.Add(10, new UpgradeData(0.76f, 652400, 0.02f));
        criDmgUpgradeData.Add(11, new UpgradeData(0.71f, 836400, 0.022f));
        criDmgUpgradeData.Add(12, new UpgradeData(0.68f, 1051600, 0.022f));
        criDmgUpgradeData.Add(13, new UpgradeData(0.62f, 1300400, 0.025f));
        criDmgUpgradeData.Add(14, new UpgradeData(0.54f, 1585200, 0.025f));
        criDmgUpgradeData.Add(15, new UpgradeData(0.45f, 1908400, 0.025f));
        criDmgUpgradeData.Add(16, new UpgradeData(0.50f, 14091400, 0.032f));
        criDmgUpgradeData.Add(17, new UpgradeData(0.49f, 15164400, 0.032f));
        criDmgUpgradeData.Add(18, new UpgradeData(0.46f, 16290200, 0.032f));
        criDmgUpgradeData.Add(19, new UpgradeData(0.41f, 17470000, 0.034f));
        criDmgUpgradeData.Add(20, new UpgradeData(0.42f, 18705000, 0.035f));
        criDmgUpgradeData.Add(21, new UpgradeData(0.38f, 19996400, 0.037f));
        criDmgUpgradeData.Add(22, new UpgradeData(0.33f, 21345400, 0.04f));
        criDmgUpgradeData.Add(23, new UpgradeData(0.26f, 22753200, 0.042f));
        criDmgUpgradeData.Add(24, new UpgradeData(0.03f, 24221000, 0.043f));
        criDmgUpgradeData.Add(25, new UpgradeData(0.03f, 25750000, 0.044f));

        //hpReg 스탯 업그레이드 데이터 초기화
        hpRegUpgradeData.Add(0, new UpgradeData(1f, 2000, 0.2f));
        hpRegUpgradeData.Add(1, new UpgradeData(0.99f, 5200, 0.2f));
        hpRegUpgradeData.Add(2, new UpgradeData(0.96f, 10800, 0.4f));
        hpRegUpgradeData.Add(3, new UpgradeData(0.91f, 18800, 0.4f));
        hpRegUpgradeData.Add(4, new UpgradeData(0.9f, 29200, 0.55f));
        hpRegUpgradeData.Add(5, new UpgradeData(0.85f, 42000, 0.55f));
        hpRegUpgradeData.Add(6, new UpgradeData(0.78f, 57200, 0.55f));
        hpRegUpgradeData.Add(7, new UpgradeData(0.71f, 74800, 0.8f));
        hpRegUpgradeData.Add(8, new UpgradeData(0.8f, 368400, 1.2f));
        hpRegUpgradeData.Add(9, new UpgradeData(0.79f, 497200, 1.2f));
        hpRegUpgradeData.Add(10, new UpgradeData(0.76f, 652400, 1.35f));
        hpRegUpgradeData.Add(11, new UpgradeData(0.71f, 836400, 1.35f));
        hpRegUpgradeData.Add(12, new UpgradeData(0.68f, 1051600, 1.9f));
        hpRegUpgradeData.Add(13, new UpgradeData(0.62f, 1300400, 2.15f));
        hpRegUpgradeData.Add(14, new UpgradeData(0.54f, 1585200, 2.15f));
        hpRegUpgradeData.Add(15, new UpgradeData(0.45f, 1908400, 2.35f));
        hpRegUpgradeData.Add(16, new UpgradeData(0.50f, 14091400, 2.95f));
        hpRegUpgradeData.Add(17, new UpgradeData(0.49f, 15164400, 2.95f));
        hpRegUpgradeData.Add(18, new UpgradeData(0.46f, 16290200, 3.25f));
        hpRegUpgradeData.Add(19, new UpgradeData(0.41f, 17470000, 3.5f));
        hpRegUpgradeData.Add(20, new UpgradeData(0.42f, 18705000, 3.65f));
        hpRegUpgradeData.Add(21, new UpgradeData(0.38f, 19996400, 3.65f));
        hpRegUpgradeData.Add(22, new UpgradeData(0.33f, 21345400, 3.9f));
        hpRegUpgradeData.Add(23, new UpgradeData(0.26f, 22753200, 4.1f));
        hpRegUpgradeData.Add(24, new UpgradeData(0.03f, 24221000, 4.15f));
        hpRegUpgradeData.Add(25, new UpgradeData(0.03f, 25750000, 4.2f));

        //coolReg 스탯 업그레이드 데이터 초기화
        coolRedUpgradeData.Add(0, new UpgradeData(1f, 2000, 0.005f));
        coolRedUpgradeData.Add(1, new UpgradeData(0.99f, 5200, 0.005f));
        coolRedUpgradeData.Add(2, new UpgradeData(0.96f, 10800, 0.005f));
        coolRedUpgradeData.Add(3, new UpgradeData(0.91f, 18800, 0.005f));
        coolRedUpgradeData.Add(4, new UpgradeData(0.9f, 29200, 0.005f));
        coolRedUpgradeData.Add(5, new UpgradeData(0.85f, 42000, 0.005f));
        coolRedUpgradeData.Add(6, new UpgradeData(0.78f, 57200, 0.005f));
        coolRedUpgradeData.Add(7, new UpgradeData(0.71f, 74800, 0.005f));
        coolRedUpgradeData.Add(8, new UpgradeData(0.8f, 368400, 0.01f));
        coolRedUpgradeData.Add(9, new UpgradeData(0.79f, 497200, 0.01f));
        coolRedUpgradeData.Add(10, new UpgradeData(0.76f, 652400, 0.01f));
        coolRedUpgradeData.Add(11, new UpgradeData(0.71f, 836400, 0.01f));
        coolRedUpgradeData.Add(12, new UpgradeData(0.68f, 1051600, 0.01f));
        coolRedUpgradeData.Add(13, new UpgradeData(0.62f, 1300400, 0.013f));
        coolRedUpgradeData.Add(14, new UpgradeData(0.54f, 1585200, 0.013f));
        coolRedUpgradeData.Add(15, new UpgradeData(0.45f, 1908400, 0.013f));
        coolRedUpgradeData.Add(16, new UpgradeData(0.50f, 14091400, 0.016f));
        coolRedUpgradeData.Add(17, new UpgradeData(0.49f, 15164400, 0.016f));
        coolRedUpgradeData.Add(18, new UpgradeData(0.46f, 16290200, 0.016f));
        coolRedUpgradeData.Add(19, new UpgradeData(0.41f, 17470000, 0.017f));
        coolRedUpgradeData.Add(20, new UpgradeData(0.42f, 18705000, 0.017f));
        coolRedUpgradeData.Add(21, new UpgradeData(0.38f, 19996400, 0.02f));
        coolRedUpgradeData.Add(22, new UpgradeData(0.33f, 21345400, 0.02f));
        coolRedUpgradeData.Add(23, new UpgradeData(0.26f, 22753200, 0.025f));
        coolRedUpgradeData.Add(24, new UpgradeData(0.03f, 24221000, 0.03f));
        coolRedUpgradeData.Add(25, new UpgradeData(0.03f, 25750000, 0.032f));
    }

    // UI가 활성화되었을 때 스탯을 서버에서 갱신
    private void OnEnable()
    {
        InitializeUpgradeData();
        UpdateStatsFromServer();
    }

    // 서버로부터 받은 데이터를 이용해 UI를 업데이트
    public void UpdateStatsFromServer()
    {
        FireBaseCharacterData characterData = FirebaseManager.Instance.currentCharacterData;
        FireBaseUserData userData = FirebaseManager.Instance.currentUserData;  // 사용자 데이터 (원화 정보)

        if (characterData != null && userData != null)
        {
            // 원화 데이터 업데이트
            currentGold = userData.won;
            UpdateGoldUI();

            // 캐릭터 이름과 레벨 업데이트
            characterNameText.text = characterData.nickName;
            levelText.text = $"{characterData.level}";

            // 스탯 정보 업데이트
            List<(string, float, float, float, int)> allStats = new List<(string, float, float, float, int)>
            {
                // 스탯 이름, 현재 값, 업그레이드 수치, 성공 확률, 리소스 비용
                ("maxHp", characterData.maxHp, maxHpUpgradeData[characterData.maxHpUpgradeLevel].statIncrease, maxHpUpgradeData[characterData.maxHpUpgradeLevel].upProb, maxHpUpgradeData[characterData.maxHpUpgradeLevel].useWon),
                ("atk", characterData.atk, atkUpgradeData[characterData.atkUpgradeLevel].statIncrease, atkUpgradeData[characterData.atkUpgradeLevel].upProb, atkUpgradeData[characterData.atkUpgradeLevel].useWon),
                ("cri", characterData.cri, criUpgradeData[characterData.criUpgradeLevel].statIncrease, criUpgradeData[characterData.criUpgradeLevel].upProb, criUpgradeData[characterData.criUpgradeLevel].useWon),
                ("criDmg", characterData.criDmg, criDmgUpgradeData[characterData.criDmgUpgradeLevel].statIncrease, criDmgUpgradeData[characterData.criDmgUpgradeLevel].upProb, criDmgUpgradeData[characterData.criDmgUpgradeLevel].useWon),
                ("hpReg", characterData.hpReg, hpRegUpgradeData[characterData.hpRegUpgradeLevel].statIncrease, hpRegUpgradeData[characterData.hpRegUpgradeLevel].upProb, hpRegUpgradeData[characterData.hpRegUpgradeLevel].useWon),
                ("coolRed", characterData.coolRed, coolRedUpgradeData[characterData.coolRedUpgradeLevel].statIncrease, coolRedUpgradeData[characterData.coolRedUpgradeLevel].upProb, coolRedUpgradeData[characterData.coolRedUpgradeLevel].useWon)
            };

            // UI 업데이트
            for (int i = 0; i < statSets.Count; i++)
            {
                if (i < allStats.Count)
                {
                    var (name, current, upgradeValue, chance, cost) = allStats[i];
                    StatsSet uiStat = statSets[i];

                    // UI 업데이트
                    uiStat.UpdateUI(name, current, upgradeValue, chance, cost);

                    // 업그레이드 버튼 이벤트 리스너 추가
                    uiStat.upgradeButton.onClick.RemoveAllListeners();
                    int index = i; // 클로저 문제를 방지하기 위해 인덱스를 캡처
                    uiStat.upgradeButton.onClick.AddListener(() => UpgradeStat(statSets[index]));
                }
            }
        }
    }

    // 골드 UI 업데이트
    private void UpdateGoldUI()
    {
        curGold.text = $"현재 골드: {currentGold}";
    }

    // 스탯 업그레이드 함수
    private void UpgradeStat(StatsSet stat)
    {
        int resourceCostValue = int.Parse(stat.resourceCost.text.Replace("강화 비용 : ", "").Trim());

        if (currentGold < resourceCostValue)
        {
            Debug.Log("골드가 부족합니다");
            return;
        }

        // 성공 확률 계산
        float chance = float.Parse(stat.successChance.text.Replace("성공 확률 : ", "").Replace("%", "")) / 100;
        bool isSuccess = Random.value <= chance;

        if (isSuccess)
        {
            // 업그레이드 성공
            float current = float.Parse(stat.currentStats.text.Replace("현재 스탯 : ", ""));
            float upgradeValue = float.Parse(stat.statsUpgradeValue.text.Replace("업그레이드 값: +", ""));
            stat.UpdateUI(
                stat.statsName.text,
                current + upgradeValue,
                upgradeValue,
                chance,
                int.Parse(stat.resourceCost.text.Replace("리소스 비용 : ", ""))
            );
            Debug.Log($"{stat.statsName.text} 업그레이드 성공!");

            // Firebase에 업그레이드된 데이터 전송
            FirebaseManager.Instance.UpgradeCharacter(stat.statsName.text);

            // 골드 차감
            currentGold -= resourceCostValue;

            // 골드 UI 업데이트
            UpdateGoldUI();
        }
        else
        {
            Debug.Log($"{stat.statsName.text} 업그레이드 실패...");
        }
    }
}
