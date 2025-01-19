using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatsSet
{
    [Tooltip("스텟 이름")]
    public TextMeshProUGUI statsName;
    [Tooltip("현재 스텟")]
    public TextMeshProUGUI currentStats;
    [Tooltip("스텟 성장값")]
    public TextMeshProUGUI statsUpgradeValue;
    [Tooltip("강화 성공 확률")]
    public TextMeshProUGUI successChance;
    [Tooltip("재화 소모값")]
    public TextMeshProUGUI resourceCost;
    [Tooltip("강화 버튼")]
    public Button upgradeButton;

    public void UpdateUI(string name, float current, float upgradeValue, float chance, int cost)
    {
        statsName.text = name;
        currentStats.text = $"현재 스탯 : {current}";
        statsUpgradeValue.text = $"성장값 : +{upgradeValue}";
        successChance.text = $"강화 확률 : {chance * 100}%";
        resourceCost.text = $"소모 재화 : {cost}";
    }
}

//임시
[System.Serializable]
public class ServerStatData
{
    public string statsName;
    public float currentStats;
    public float statsUpgradeValue;
    public float successChance;
    public int resourceCost;
}

public class ServerCharacterData
{
    public string characterName; 
    public int level;            
    public int currentGold;
    public ServerStatData maxHP;
    public ServerStatData atkPower;
    public ServerStatData critChance;
    public ServerStatData critDamage;
    public ServerStatData dmgRed;
    public ServerStatData hpRegen;
    public ServerStatData cDReduction;
}

public class CharacterUpgradeUI : MonoBehaviour
{
    [Header("Character Info"), SerializeField]
    public TextMeshProUGUI characterNameText; 
    public TextMeshProUGUI levelText;

    [Header("Stats Sets"), SerializeField]
    public TextMeshProUGUI curGold;
    [SerializeField] private List<StatsSet> statSets;

    private int currentGold;

    //서버로부터 받은 데이터를 UI에 적용
    public void UpdateStatsFromServer(ServerCharacterData serverData)
    {
        //골드 업데이트
        currentGold = serverData.currentGold;
        UpdateGoldUI();

        // 캐릭터 이름과 레벨 업데이트
        characterNameText.text = serverData.characterName;
        levelText.text = $"{serverData.level}"; 

        //모든 스텟 UI 업데이트
        List<ServerStatData> allStats = new List<ServerStatData>
        {
            serverData.maxHP,
            serverData.atkPower,
            serverData.critChance,
            serverData.critDamage,
            serverData.hpRegen,
            serverData.cDReduction,
            serverData.dmgRed
        };

        //각 스텟에 대해 UI 업데이트 및 버튼 이벤트 설정
        for (int i = 0; i < statSets.Count; i++)
        {
            if (i < allStats.Count)
            {
                ServerStatData serverStat = allStats[i];
                StatsSet uiStat = statSets[i];

                //UI 업데이트
                uiStat.UpdateUI(
                    serverStat.statsName,
                    serverStat.currentStats,
                    serverStat.statsUpgradeValue,
                    serverStat.successChance,
                    serverStat.resourceCost
                );

                //버튼 이벤트 설정
                uiStat.upgradeButton.onClick.RemoveAllListeners();
                int index = i; //closure 문제 해결을 위한 인덱스 저장 (람다식에서 오류를 방지)
                uiStat.upgradeButton.onClick.AddListener(() => UpgradeStat(statSets[index]));
            }
        }
    }

    //골드 UI 업데이트
    private void UpdateGoldUI()
    {
        curGold.text = $"현재 골드: {currentGold}";
    }

    //강화 버튼 클릭 이벤트
    private void UpgradeStat(StatsSet stat)
    {
        float chance = float.Parse(stat.successChance.text.Replace("강화 확률 : ", "").Replace("%", "")) / 100;
        bool isSuccess = Random.value <= chance;

        if (isSuccess)
        {
            float current = float.Parse(stat.currentStats.text.Replace("현재 스탯 : ", ""));
            float upgradeValue = float.Parse(stat.statsUpgradeValue.text.Replace("성장값 : +", ""));
            stat.UpdateUI(
                stat.statsName.text,
                current + upgradeValue,
                upgradeValue,
                chance,
                int.Parse(stat.resourceCost.text.Replace("소모 재화 : ", ""))
            );
            Debug.Log($"{stat.statsName.text} 강화 성공!");
        }
        else
        {
            Debug.Log($"{stat.statsName.text} 강화 실패...");
        }
    }
}
