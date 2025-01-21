using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.VersionControl.Asset;

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

public class CharacterUpgradeUI : MonoBehaviour
{
    [Header("Character Info"), SerializeField]
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI curGold;

    [Header("Stats Sets"), SerializeField]
    private List<StatsSet> statSets;

    private int currentGold;

    private void OnEnable()
    {
        UpdateStatsFromServer();
    }

    // 서버로부터 받은 데이터를 UI에 적용
    public void UpdateStatsFromServer()
    {
        // FirebaseManager에서 직접 가져온 데이터를 사용
        FireBaseCharacterData characterData = FirebaseManager.Instance.currentCharacterData;
        FireBaseUserData userData = FirebaseManager.Instance.currentUserData;  // won 값을 가져오는 데 사용

        if (characterData != null && userData != null)
        {
            // 골드 업데이트 (won 값 사용)
            currentGold = userData.won;
            UpdateGoldUI();

            // 캐릭터 이름과 레벨 업데이트
            characterNameText.text = characterData.nickName;
            levelText.text = $"{characterData.level}";

            // 스텟 리스트 업데이트 (성공 확률을 레벨에 맞춰 계산)
            List<(string, float, float, float, int)> allStats = new List<(string, float, float, float, int)>
            {
                // 스텟 이름, 현재 스텟, 성장 수치, 성공 확률, 소모 재화
                ("maxHp", characterData.maxHp, 1.0f, CalculateSuccessChance(characterData.level), 100),
                ("atk", characterData.atk, 0.5f, CalculateSuccessChance(characterData.level), 150),
                ("cri", characterData.cri, 1.5f, CalculateSuccessChance(characterData.level), 200),
                ("criDmg", characterData.criDmg, 0.5f, CalculateSuccessChance(characterData.level), 250),
                ("dmgRed", characterData.dmgRed, 0.2f, CalculateSuccessChance(characterData.level), 300),
                ("hpReg", characterData.hpReg, 0.25f, CalculateSuccessChance(characterData.level), 400),
                ("coolRed", characterData.coolRed, 0.1f, CalculateSuccessChance(characterData.level), 500)
            };

            // UI 업데이트 및 버튼 이벤트 설정
            for (int i = 0; i < statSets.Count; i++)
            {
                if (i < allStats.Count)
                {
                    var (name, current, upgradeValue, chance, cost) = allStats[i];
                    StatsSet uiStat = statSets[i];

                    // UI 업데이트
                    uiStat.UpdateUI(name, current, upgradeValue, chance, cost);

                    // 버튼 클릭 시 강화 처리
                    uiStat.upgradeButton.onClick.RemoveAllListeners();
                    int index = i; // 람다식의 closure 문제 해결을 위한 인덱스 저장
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

    // 성공 확률을 레벨에 맞게 계산하는 메서드
    private float CalculateSuccessChance(int level)
    {
        // 기본 성공 확률 (75%)
        float baseChance = 0.75f;

        // 레벨이 증가함에 따라 성공 확률을 0.5%씩 감소 (최소 5%로 제한)
        float decreasePerLevel = 0.005f; // 0.5% -> 0.005

        // 성공 확률 계산
        float successChance = Mathf.Max(baseChance - (level - 1) * decreasePerLevel, 0.05f);

        return successChance;
    }

    // 강화 버튼 클릭 이벤트
    private void UpgradeStat(StatsSet stat)
    {
        int resourceCostValue = int.Parse(stat.resourceCost.text.Replace("소모 재화 : ", "").Trim());

        if (currentGold < resourceCostValue)
        {
            Debug.Log("돈 부족");
            return;
        }

            // 성공 확률을 얻기 위해 text를 파싱
            float chance = float.Parse(stat.successChance.text.Replace("강화 확률 : ", "").Replace("%", "")) / 100;
            bool isSuccess = Random.value <= chance;

        if (isSuccess)
        {
            // 강화 성공 시 UI 업데이트
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

            // Firebase에서 데이터 업데이트
            FirebaseManager.Instance.UpgradeCharacter(stat.statsName.text);  // 강화된 스텟을 Firebase에 반영

            // 강화 성공 후, 소모된 금액만큼 골드를 차감
            currentGold -= resourceCostValue;

            // 골드 UI 업데이트
            UpdateGoldUI();  // 골드 UI를 갱신
        }
        else
        {
            Debug.Log($"{stat.statsName.text} 강화 실패...");
        }
    }
}
