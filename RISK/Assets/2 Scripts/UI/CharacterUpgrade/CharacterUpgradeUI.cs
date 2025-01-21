using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatsSet
{
    [Tooltip("?ㅽ뀩 ?대쫫")]
    public TextMeshProUGUI statsName;
    [Tooltip("?꾩옱 ?ㅽ뀩")]
    public TextMeshProUGUI currentStats;
    [Tooltip("?ㅽ뀩 ?깆옣媛?")]
    public TextMeshProUGUI statsUpgradeValue;
    [Tooltip("媛뺥솕 ?깃났 ?뺣쪧")]
    public TextMeshProUGUI successChance;
    [Tooltip("?ы솕 ?뚮え媛?")]
    public TextMeshProUGUI resourceCost;
    [Tooltip("媛뺥솕 踰꾪듉")]
    public Button upgradeButton;

    public void UpdateUI(string name, float current, float upgradeValue, float chance, int cost)
    {
        statsName.text = name;
        currentStats.text = $"?꾩옱 ?ㅽ꺈 : {current}";
        statsUpgradeValue.text = $"?깆옣媛?: +{upgradeValue}";
        successChance.text = $"媛뺥솕 ?뺣쪧 : {chance * 100}%";
        resourceCost.text = $"?뚮え ?ы솕 : {cost}";
    }
}

[System.Serializable]
public class UpgradeData
{
    public float upProb;  // 媛뺥솕 ?깃났 ?뺣쪧
    public int useWon;    // 媛뺥솕???꾩슂???ы솕 (怨⑤뱶)
    public float statIncrease;  // ?대떦 ?ㅽ뀩???깆옣 ?섏튂 (?? maxHp 利앷??? atk 利앷?????

    // ?앹꽦??(?뚯씠釉??곗씠?곕? ?ｌ쓣 ???덇쾶)
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

    private void InitializeUpgradeData()
    {
        // maxHp ?ㅽ뀩 ?곗씠??珥덇린??
        maxHpUpgradeData.Add(0, new UpgradeData(1f, 2000, 5));
        maxHpUpgradeData.Add(1, new UpgradeData(0.99f, 5200, 5));
        maxHpUpgradeData.Add(2, new UpgradeData(0.96f, 10800, 5));
        maxHpUpgradeData.Add(3, new UpgradeData(0.91f, 18800, 5));
        // 異붽??곸씤 ?덈꺼 ?곗씠?곕? ?쎌엯...

        // atk ?ㅽ뀩 ?곗씠??珥덇린??
        atkUpgradeData.Add(0, new UpgradeData(1f, 1500, 4));
        atkUpgradeData.Add(1, new UpgradeData(0.99f, 5200, 4));
        atkUpgradeData.Add(2, new UpgradeData(0.96f, 10800, 6));
        // 異붽??곸씤 ?덈꺼 ?곗씠?곕? ?쎌엯...

        // ?섎㉧吏 ?ㅽ뀩?ㅼ뿉 ??댁꽌??鍮꾩듂?섍쾶 異붽?
    }

    private void OnEnable()
    {
        UpdateStatsFromServer();
    }

    // ?쒕쾭濡쒕???諛쏆? ?곗씠?곕? UI???곸슜
    public void UpdateStatsFromServer()
    {
        // FirebaseManager?먯꽌 吏곸젒 媛?몄삩 ?곗씠?곕? ?ъ슜
        FireBaseCharacterData characterData = FirebaseManager.Instance.currentCharacterData;
        FireBaseUserData userData = FirebaseManager.Instance.currentUserData;  // won 媛믪쓣 媛?몄삤?????ъ슜

        if (characterData != null && userData != null)
        {
            // 怨⑤뱶 ?낅뜲?댄듃 (won 媛??ъ슜)
            currentGold = userData.won;
            UpdateGoldUI();

            // 罹먮┃???대쫫怨??덈꺼 ?낅뜲?댄듃
            characterNameText.text = characterData.nickName;
            levelText.text = $"{characterData.level}";

            // ?ㅽ뀩 由ъ뒪???낅뜲?댄듃 (?깃났 ?뺣쪧???덈꺼??留욎떠 怨꾩궛)
            List<(string, float, float, float, int)> allStats = new List<(string, float, float, float, int)>
            {
                // ?ㅽ뀩 ?대쫫, ?꾩옱 ?ㅽ뀩, ?깆옣 ?섏튂, ?깃났 ?뺣쪧, ?뚮え ?ы솕
                ("maxHp", characterData.maxHp, maxHpUpgradeData[characterData.maxHpUpgradeLevel].statIncrease, maxHpUpgradeData[characterData.maxHpUpgradeLevel].upProb, maxHpUpgradeData[characterData.maxHpUpgradeLevel].useWon),
                ("atk", characterData.atk, atkUpgradeData[characterData.atkUpgradeLevel].statIncrease, atkUpgradeData[characterData.atkUpgradeLevel].upProb, atkUpgradeData[characterData.atkUpgradeLevel].useWon),
                ("cri", characterData.cri, criUpgradeData[characterData.criUpgradeLevel].statIncrease, criUpgradeData[characterData.criUpgradeLevel].upProb, criUpgradeData[characterData.criUpgradeLevel].useWon),
                ("criDmg", characterData.criDmg, criDmgUpgradeData[characterData.criDmgUpgradeLevel].statIncrease, criDmgUpgradeData[characterData.criDmgUpgradeLevel].upProb, criDmgUpgradeData[characterData.criDmgUpgradeLevel].useWon),
                ("hpReg", characterData.hpReg, hpRegUpgradeData[characterData.hpRegUpgradeLevel].statIncrease, hpRegUpgradeData[characterData.hpRegUpgradeLevel].upProb, hpRegUpgradeData[characterData.hpRegUpgradeLevel].useWon),
                ("coolRed", characterData.coolRed, coolRedUpgradeData[characterData.coolRedUpgradeLevel].statIncrease, coolRedUpgradeData[characterData.coolRedUpgradeLevel].upProb, coolRedUpgradeData[characterData.coolRedUpgradeLevel].useWon)
            };

            // UI ?낅뜲?댄듃 諛?踰꾪듉 ?대깽???ㅼ젙
            for (int i = 0; i < statSets.Count; i++)
            {
                if (i < allStats.Count)
                {
                    var (name, current, upgradeValue, chance, cost) = allStats[i];
                    StatsSet uiStat = statSets[i];

                    // UI ?낅뜲?댄듃
                    uiStat.UpdateUI(name, current, upgradeValue, chance, cost);

                    // 踰꾪듉 ?대┃ ??媛뺥솕 泥섎━
                    uiStat.upgradeButton.onClick.RemoveAllListeners();
                    int index = i; // ?뚮떎?앹쓽 closure 臾몄젣 ?닿껐???꾪븳 ?몃뜳?????
                    uiStat.upgradeButton.onClick.AddListener(() => UpgradeStat(statSets[index]));
                }
            }
        }
    }

    // 怨⑤뱶 UI ?낅뜲?댄듃
    private void UpdateGoldUI()
    {
        curGold.text = $"?꾩옱 怨⑤뱶: {currentGold}";
    }

    // ?깃났 ?뺣쪧???덈꺼??留욊쾶 怨꾩궛?섎뒗 硫붿꽌??
    private float CalculateSuccessChance(int level)
    {
        // 湲곕낯 ?깃났 ?뺣쪧 (75%)
        float baseChance = 0.75f;

        // ?덈꺼??利앷??⑥뿉 ?곕씪 ?깃났 ?뺣쪧??0.5%??媛먯냼 (理쒖냼 5%濡??쒗븳)
        float decreasePerLevel = 0.005f; // 0.5% -> 0.005

        // ?깃났 ?뺣쪧 怨꾩궛
        float successChance = Mathf.Max(baseChance - (level - 1) * decreasePerLevel, 0.05f);

        return successChance;
    }

    // 媛뺥솕 踰꾪듉 ?대┃ ?대깽??
    private void UpgradeStat(StatsSet stat)
    {
        int resourceCostValue = int.Parse(stat.resourceCost.text.Replace("?뚮え ?ы솕 : ", "").Trim());

        if (currentGold < resourceCostValue)
        {
            Debug.Log("??遺議?");
            return;
        }

        // ?깃났 ?뺣쪧???산린 ?꾪빐 text瑜??뚯떛
        float chance = float.Parse(stat.successChance.text.Replace("媛뺥솕 ?뺣쪧 : ", "").Replace("%", "")) / 100;
        bool isSuccess = Random.value <= chance;

        if (isSuccess)
        {
            // 媛뺥솕 ?깃났 ??UI ?낅뜲?댄듃
            float current = float.Parse(stat.currentStats.text.Replace("?꾩옱 ?ㅽ꺈 : ", ""));
            float upgradeValue = float.Parse(stat.statsUpgradeValue.text.Replace("?깆옣媛?: +", ""));
            stat.UpdateUI(
                stat.statsName.text,
                current + upgradeValue,
                upgradeValue,
                chance,
                int.Parse(stat.resourceCost.text.Replace("?뚮え ?ы솕 : ", ""))
            );
            Debug.Log($"{stat.statsName.text} 媛뺥솕 ?깃났!");

            // Firebase?먯꽌 ?곗씠???낅뜲?댄듃
            FirebaseManager.Instance.UpgradeCharacter(stat.statsName.text);  // 媛뺥솕???ㅽ뀩??Firebase??諛섏쁺

            // 媛뺥솕 ?깃났 ?? ?뚮え??湲덉븸留뚰겮 怨⑤뱶瑜?李④컧
            currentGold -= resourceCostValue;

            // 怨⑤뱶 UI ?낅뜲?댄듃
            UpdateGoldUI();  // 怨⑤뱶 UI瑜?媛깆떊
        }
        else
        {
            Debug.Log($"{stat.statsName.text} 媛뺥솕 ?ㅽ뙣...");
        }
    }
}
