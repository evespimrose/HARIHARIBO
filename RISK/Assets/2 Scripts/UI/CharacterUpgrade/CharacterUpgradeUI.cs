using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatsSet
{
    [Tooltip("???댟????藥?")]
    public TextMeshProUGUI statsName;
    [Tooltip("?熬곣뫗?????댟?")]
    public TextMeshProUGUI currentStats;
    [Tooltip("???댟??繹먮냱?€뤆?")]
    public TextMeshProUGUI statsUpgradeValue;
    [Tooltip("?띠룆踰???繹먭퍓沅??筌먲퐣??")]
    public TextMeshProUGUI successChance;
    [Tooltip("???????嫄잍뤆?")]
    public TextMeshProUGUI resourceCost;
    [Tooltip("?띠룆踰???뺢퀗???")]
    public Button upgradeButton;

    public void UpdateUI(string name, float current, float upgradeValue, float chance, int cost)
    {
        statsName.text = name;
        currentStats.text = $"?熬곣뫗?????꾪떅 : {current}";
        statsUpgradeValue.text = $"?繹먮냱?€뤆?: +{upgradeValue}";
        successChance.text = $"?띠룆踰???筌먲퐣??: {chance * 100}%";
        resourceCost.text = $"???嫄?????: {cost}";
    }
}

[System.Serializable]
public class UpgradeData
{
    public float upProb;  // ?띠룆踰???繹먭퍓沅??筌먲퐣??(1.0?? 100%)
    public int useWon;    // ?띠룆踰????熬곣뫗???????(??λ?獄?
    public float maxHpIncrease;
    public float atkIncrease;
    public float criIncrease;
    public float criDmgIncrease;
    public float hpRegIncrease;
    public float coolRedIncrease;
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

    private void OnEnable()
    {
        UpdateStatsFromServer();
    }

    // ??類ㅼ뮅?β돦裕????꾩룇猷? ??⑥щ턄??? UI????⑤챷??
    public void UpdateStatsFromServer()
    {
        // FirebaseManager?????嶺뚯쉳????띠럾??筌뤾쑴沅???⑥щ턄??? ????
        FireBaseCharacterData characterData = FirebaseManager.Instance.currentCharacterData;
        FireBaseUserData userData = FirebaseManager.Instance.currentUserData;  // won ?띠룆????띠럾??筌뤾쑴沅????????

        if (characterData != null && userData != null)
        {
            // ??λ?獄????낆몥??袁⑤콦 (won ??????
            currentGold = userData.won;
            UpdateGoldUI();

            // 嶺?큔??????藥꿔깺?????뉖낵 ???낆몥??袁⑤콦
            characterNameText.text = characterData.nickName;
            levelText.text = $"{characterData.level}";

            // ???댟??洹먮봾裕?????낆몥??袁⑤콦 (?繹먭퍓沅??筌먲퐣??????뉖낵??嶺뚮씮?????ｌ뫒亦?
            List<(string, float, float, float, int)> allStats = new List<(string, float, float, float, int)>
            {
                // ???댟????藥? ?熬곣뫗?????댟? ?繹먮냱????瑜귣뭵, ?繹먭퍓沅??筌먲퐣?? ???嫄?????
                ("maxHp", characterData.maxHp, 1.0f, CalculateSuccessChance(characterData.level), 100),
                ("atk", characterData.atk, 0.5f, CalculateSuccessChance(characterData.level), 150),
                ("cri", characterData.cri, 1.5f, CalculateSuccessChance(characterData.level), 200),
                ("criDmg", characterData.criDmg, 0.5f, CalculateSuccessChance(characterData.level), 250),
                ("dmgRed", characterData.dmgRed, 0.2f, CalculateSuccessChance(characterData.level), 300),
                ("hpReg", characterData.hpReg, 0.25f, CalculateSuccessChance(characterData.level), 400),
                ("coolRed", characterData.coolRed, 0.1f, CalculateSuccessChance(characterData.level), 500)
            };

            // UI ???낆몥??袁⑤콦 ???뺢퀗??????繹?????깆젧
            for (int i = 0; i < statSets.Count; i++)
            {
                if (i < allStats.Count)
                {
                    var (name, current, upgradeValue, chance, cost) = allStats[i];
                    StatsSet uiStat = statSets[i];

                    // UI ???낆몥??袁⑤콦
                    uiStat.UpdateUI(name, current, upgradeValue, chance, cost);

                    // ?뺢퀗???????????띠룆踰??嶺뚳퐣瑗??
                    uiStat.upgradeButton.onClick.RemoveAllListeners();
                    int index = i; // ???堉??諭踰?closure ??쒖굣?????㏉뜖???熬곥굥由??筌뤾퍓???????
                    uiStat.upgradeButton.onClick.AddListener(() => UpgradeStat(statSets[index]));
                }
            }
        }
    }

    // ??λ?獄?UI ???낆몥??袁⑤콦
    private void UpdateGoldUI()
    {
        curGold.text = $"?熬곣뫗????λ?獄? {currentGold}";
    }

    // ?繹먭퍓沅??筌먲퐣??????뉖낵??嶺뚮씮?녘떋???ｌ뫒亦??濡ル츎 嶺뚮∥?꾥땻??
    private float CalculateSuccessChance(int level)
    {
        // ?リ옇????繹먭퍓沅??筌먲퐣??(75%)
        float baseChance = 0.75f;

        // ???뉖낵??嶺뚯빘鍮???貫????⑤벡逾??繹먭퍓沅??筌먲퐣???0.5%???띠룆흮??(嶺뚣끉裕??5%?????ル┰)
        float decreasePerLevel = 0.005f; // 0.5% -> 0.005

        // ?繹먭퍓沅??筌먲퐣????ｌ뫒亦?
        float successChance = Mathf.Max(baseChance - (level - 1) * decreasePerLevel, 0.05f);

        return successChance;
    }

    // ?띠룆踰???뺢퀗???????????繹??
    private void UpgradeStat(StatsSet stat)
    {
        int resourceCostValue = int.Parse(stat.resourceCost.text.Replace("???嫄?????: ", "").Trim());

        if (currentGold < resourceCostValue)
        {
            Debug.Log("???遊붋??");
            return;
        }

        // ?繹먭퍓沅??筌먲퐣?????怨뺚뵛 ?熬곥굥??text?????堉?
        float chance = float.Parse(stat.successChance.text.Replace("?띠룆踰???筌먲퐣??: ", "").Replace("%", "")) / 100;
        bool isSuccess = Random.value <= chance;

        if (isSuccess)
        {
            // ?띠룆踰???繹먭퍓沅???UI ???낆몥??袁⑤콦
            float current = float.Parse(stat.currentStats.text.Replace("?熬곣뫗?????꾪떅 : ", ""));
            float upgradeValue = float.Parse(stat.statsUpgradeValue.text.Replace("?繹먮냱?€뤆?: +", ""));
            stat.UpdateUI(
                stat.statsName.text,
                current + upgradeValue,
                upgradeValue,
                chance,
                int.Parse(stat.resourceCost.text.Replace("???嫄?????: ", ""))
            );
            Debug.Log($"{stat.statsName.text} ?띠룆踰???繹먭퍓沅?");

            // Firebase???????⑥щ턄?????낆몥??袁⑤콦
            FirebaseManager.Instance.UpgradeCharacter(stat.statsName.text);  // ?띠룆踰??????댟??Firebase???꾩룇瑗??

            // ?띠룆踰???繹먭퍓沅??? ???嫄???ル?녽뇡紐뚯춹?怨뚭퍖 ??λ?獄?쓣紐?嶺뚢뼰紐욤?
            currentGold -= resourceCostValue;

            // ??λ?獄?UI ???낆몥??袁⑤콦
            UpdateGoldUI();  // ??λ?獄?UI???띠룄???
        }
        else
        {
            Debug.Log($"{stat.statsName.text} ?띠룆踰?????덉넮...");
        }
    }
}
