using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatsSet
{
    [Tooltip("스탯 이름")]
    public TextMeshProUGUI statsName; 
    [Tooltip("현재 스탯 값")]
    public TextMeshProUGUI currentStats; 
    [Tooltip("스탯 업그레이드 값")]
    public TextMeshProUGUI statsUpgradeValue;  
    [Tooltip("성공 확률")]
    public TextMeshProUGUI successChance;  
    [Tooltip("리소스 비용")]
    public TextMeshProUGUI resourceCost; 
    [Tooltip("업그레이드 버튼")]
    public Button upgradeButton;  

    public void UpdateUI(string name, float current, float upgradeValue, float chance, int cost, int upgradeLevel)
    {
        if (upgradeLevel >= 25)
        {
            statsName.text = "MAX";         //statsName
            currentStats.text = "MAX";      //currentStats
            statsUpgradeValue.text = "MAX"; //statsUpgradeValue
            successChance.text = "MAX";     //successChance
            resourceCost.text = "MAX";      //resourceCost
            upgradeButton.interactable = false;
        }
        else
        {
            statsName.text = name;  
            currentStats.text = $"{current}";  
            statsUpgradeValue.text = $"{upgradeValue}"; 
            successChance.text = $"{chance * 100}";
            resourceCost.text = $"{cost}"; 
            upgradeButton.interactable = true;
        }
    }
}

[System.Serializable]
public class UpgradeData
{
    public float upProb;       //Upgrade %
    public int useWon;         //Upgrade won
    public float statIncrease; //UpStat

    //Initialize Upgrade Data
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
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI levelText; 
    public TextMeshProUGUI curGold; 
    public Image characterImage;

    public Button closeButton;

    [Header("Stats Sets"), SerializeField]
    private List<StatsSet> statSets; 

    [Header("Upgrade Data")]
    [SerializeField] private Dictionary<int, UpgradeData> maxHpUpgradeData = new Dictionary<int, UpgradeData>();  
    [SerializeField] private Dictionary<int, UpgradeData> atkUpgradeData = new Dictionary<int, UpgradeData>(); 
    [SerializeField] private Dictionary<int, UpgradeData> criUpgradeData = new Dictionary<int, UpgradeData>(); 
    [SerializeField] private Dictionary<int, UpgradeData> criDmgUpgradeData = new Dictionary<int, UpgradeData>();  
    [SerializeField] private Dictionary<int, UpgradeData> hpRegUpgradeData = new Dictionary<int, UpgradeData>();  
    [SerializeField] private Dictionary<int, UpgradeData> coolRedUpgradeData = new Dictionary<int, UpgradeData>();  

    private int currentGold; 

    private FireBaseCharacterData characterData;
    private FireBaseUserData userData;

    //Update stat on server when UI is enabled
    private void OnEnable()
    {
        characterData = FirebaseManager.Instance.currentCharacterData;
        userData = FirebaseManager.Instance.currentUserData;
        InitializeUpgradeData();
        UpdateStatsFromServer();
    }

    private void Start()
    {
        closeButton.onClick.AddListener(CloseUI);
    }

    public void CloseUI()
    {
        PanelManager.Instance.PanelOpen("PartyListBoard");
    }

    //Update UI using data received from the server
    public void UpdateStatsFromServer()
    {
        if (characterData != null && userData != null)
        {
            UpdateGoldUI(userData);

            characterNameText.text = characterData.nickName;
            levelText.text = $"{characterData.level}";
            characterImage.sprite = GameManager.Instance.characterDataDic[characterData.classType].sprite;

            List<(string, float, float, float, int, int)> allStats = new List<(string, float, float, float, int, int)>
            {
                //stat name, current value, upgrade number, success probability, resource cost, Upgrade Level
                ("maxHp", characterData.maxHp, maxHpUpgradeData[characterData.maxHpUpgradeLevel].statIncrease, maxHpUpgradeData[characterData.maxHpUpgradeLevel].upProb, maxHpUpgradeData[characterData.maxHpUpgradeLevel].useWon, characterData.maxHpUpgradeLevel),
                ("atk", characterData.atk, atkUpgradeData[characterData.atkUpgradeLevel].statIncrease, atkUpgradeData[characterData.atkUpgradeLevel].upProb, atkUpgradeData[characterData.atkUpgradeLevel].useWon, characterData.atkUpgradeLevel),
                ("cri", characterData.cri, criUpgradeData[characterData.criUpgradeLevel].statIncrease, criUpgradeData[characterData.criUpgradeLevel].upProb, criUpgradeData[characterData.criUpgradeLevel].useWon, characterData.criUpgradeLevel),
                ("criDmg", characterData.criDmg, criDmgUpgradeData[characterData.criDmgUpgradeLevel].statIncrease, criDmgUpgradeData[characterData.criDmgUpgradeLevel].upProb, criDmgUpgradeData[characterData.criDmgUpgradeLevel].useWon, characterData.criDmgUpgradeLevel),
                ("hpReg", characterData.hpReg, hpRegUpgradeData[characterData.hpRegUpgradeLevel].statIncrease, hpRegUpgradeData[characterData.hpRegUpgradeLevel].upProb, hpRegUpgradeData[characterData.hpRegUpgradeLevel].useWon, characterData.hpRegUpgradeLevel),
                ("coolRed", characterData.coolRed, coolRedUpgradeData[characterData.coolRedUpgradeLevel].statIncrease, coolRedUpgradeData[characterData.coolRedUpgradeLevel].upProb, coolRedUpgradeData[characterData.coolRedUpgradeLevel].useWon, characterData.coolRedUpgradeLevel)
            };

            //UI Update
            for (int i = 0; i < statSets.Count; i++)
            {
                if (i < allStats.Count)
                {
                    var (name, current, upgradeValue, chance, cost, upgradeLavel) = allStats[i];
                    StatsSet uiStat = statSets[i];

                    uiStat.UpdateUI(name, current, upgradeValue, chance, cost, upgradeLavel);

                    uiStat.upgradeButton.onClick.RemoveAllListeners();
                    int index = i; //Capture indexes to avoid closure issues
                    uiStat.upgradeButton.onClick.AddListener(() => UpgradeStat(statSets[index]));
                }
            }
        }
    }

    private void UpdateGoldUI(FireBaseUserData userData)
    {
        curGold.text = $"현재 골드: {userData.won}";
    }

    private void UpgradeStat(StatsSet stat)
    {
        int resourceCostValue = int.Parse(stat.resourceCost.text);

        if (userData.won < resourceCostValue)
        {
            Debug.Log("골드가 부족합니다");
            return;
        }
        userData.won -= resourceCostValue;
        Debug.Log($"골드소모 {userData.won}");
        //userData에 골드 업데이트
        FirebaseManager.Instance.currentUserData = userData;
        FirebaseManager.Instance.UpdateWon(() => UpdateGoldUI(userData));

        float chance = float.Parse(stat.successChance.text) / 100f;
        bool isSuccess = Random.value <= chance;
        Debug.Log("강화 시작");

        if (isSuccess)
        {
            Debug.Log($"{stat.statsName.text} 강화 성공");

            float current = float.Parse(stat.currentStats.text);
            float upgradeValue = float.Parse(stat.statsUpgradeValue.text);

            //Processing after update using callback
            FirebaseManager.Instance.UpgradeCharacter(stat.statsName.text, upgradeValue, () =>
            {
                UpdateStatsFromServer();
            });
        }
        else
        {
            Debug.Log($"{stat.statsName.text} 강화 실패");
        }
        UpdateGoldUI(userData);
    }

    private void InitializeUpgradeData()
    {
        //maxHpUpgradeData
        maxHpUpgradeData.Clear();
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

        //atkUpgradeData
        atkUpgradeData.Clear();
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

        //criUpgradeData
        criUpgradeData.Clear();
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

        //criDmgUpgradeData
        criDmgUpgradeData.Clear();
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

        //hpRegUpgradeData
        hpRegUpgradeData.Clear();
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

        //coolRedUpgradeData
        coolRedUpgradeData.Clear();
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
}
