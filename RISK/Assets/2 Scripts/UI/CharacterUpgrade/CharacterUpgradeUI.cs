using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatsSet
{
    [Tooltip("??쎈???已?")]
    public TextMeshProUGUI statsName;
    [Tooltip("?袁⑹삺 ??쎈?")]
    public TextMeshProUGUI currentStats;
    [Tooltip("??쎈??源놁삢揶?")]
    public TextMeshProUGUI statsUpgradeValue;
    [Tooltip("揶쏅벤???源껊궗 ?類ｌぇ")]
    public TextMeshProUGUI successChance;
    [Tooltip("???????걟揶?")]
    public TextMeshProUGUI resourceCost;
    [Tooltip("揶쏅벤??甕곌쑵??")]
    public Button upgradeButton;

    public void UpdateUI(string name, float current, float upgradeValue, float chance, int cost)
    {
        statsName.text = name;
        currentStats.text = $"?袁⑹삺 ??쎄틛 : {current}";
        statsUpgradeValue.text = $"?源놁삢揶?: +{upgradeValue}";
        successChance.text = $"揶쏅벤???類ｌぇ : {chance * 100}%";
        resourceCost.text = $"???걟 ????: {cost}";
    }
}

[System.Serializable]
public class UpgradeData
{
    public float upProb;  // 揶쏅벤???源껊궗 ?類ｌぇ (1.0?? 100%)
    public int useWon;    // 揶쏅벤????袁⑹뒄??????(?ⓥ뫀諭?
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

    // ??뺤쒔嚥≪뮆???獄쏆룇? ?怨쀬뵠?怨? UI???怨몄뒠
    public void UpdateStatsFromServer()
    {
        // FirebaseManager?癒?퐣 筌욊낯??揶쎛?紐꾩궔 ?怨쀬뵠?怨? ????
        FireBaseCharacterData characterData = FirebaseManager.Instance.currentCharacterData;
        FireBaseUserData userData = FirebaseManager.Instance.currentUserData;  // won 揶쏅???揶쎛?紐꾩궎????????

        if (characterData != null && userData != null)
        {
            // ?ⓥ뫀諭???낅쑓??꾨뱜 (won 揶?????
            currentGold = userData.won;
            UpdateGoldUI();

            // 筌?Ŧ?????已ユ???덇볼 ??낅쑓??꾨뱜
            characterNameText.text = characterData.nickName;
            levelText.text = $"{characterData.level}";

            // ??쎈??귐딅뮞????낅쑓??꾨뱜 (?源껊궗 ?類ｌぇ????덇볼??筌띿쉸???④쑴沅?
            List<(string, float, float, float, int)> allStats = new List<(string, float, float, float, int)>
            {
                // ??쎈???已? ?袁⑹삺 ??쎈? ?源놁삢 ??륂뒄, ?源껊궗 ?類ｌぇ, ???걟 ????
                ("maxHp", characterData.maxHp, 1.0f, CalculateSuccessChance(characterData.level), 100),
                ("atk", characterData.atk, 0.5f, CalculateSuccessChance(characterData.level), 150),
                ("cri", characterData.cri, 1.5f, CalculateSuccessChance(characterData.level), 200),
                ("criDmg", characterData.criDmg, 0.5f, CalculateSuccessChance(characterData.level), 250),
                ("dmgRed", characterData.dmgRed, 0.2f, CalculateSuccessChance(characterData.level), 300),
                ("hpReg", characterData.hpReg, 0.25f, CalculateSuccessChance(characterData.level), 400),
                ("coolRed", characterData.coolRed, 0.1f, CalculateSuccessChance(characterData.level), 500)
            };

            // UI ??낅쑓??꾨뱜 獄?甕곌쑵????源????쇱젟
            for (int i = 0; i < statSets.Count; i++)
            {
                if (i < allStats.Count)
                {
                    var (name, current, upgradeValue, chance, cost) = allStats[i];
                    StatsSet uiStat = statSets[i];

                    // UI ??낅쑓??꾨뱜
                    uiStat.UpdateUI(name, current, upgradeValue, chance, cost);

                    // 甕곌쑵????????揶쏅벤??筌ｌ꼶??
                    uiStat.upgradeButton.onClick.RemoveAllListeners();
                    int index = i; // ???뼄??뱀벥 closure ?얜챷????욧퍙???袁る립 ?紐껊쑔??????
                    uiStat.upgradeButton.onClick.AddListener(() => UpgradeStat(statSets[index]));
                }
            }
        }
    }

    // ?ⓥ뫀諭?UI ??낅쑓??꾨뱜
    private void UpdateGoldUI()
    {
        curGold.text = $"?袁⑹삺 ?ⓥ뫀諭? {currentGold}";
    }

    // ?源껊궗 ?類ｌぇ????덇볼??筌띿쉳苡??④쑴沅??롫뮉 筌롫뗄苑??
    private float CalculateSuccessChance(int level)
    {
        // 疫꿸퀡???源껊궗 ?類ｌぇ (75%)
        float baseChance = 0.75f;

        // ??덇볼??筌앹빓???λ퓠 ?怨뺤뵬 ?源껊궗 ?類ｌぇ??0.5%??揶쏅Ŋ??(筌ㅼ뮇??5%嚥???쀫립)
        float decreasePerLevel = 0.005f; // 0.5% -> 0.005

        // ?源껊궗 ?類ｌぇ ?④쑴沅?
        float successChance = Mathf.Max(baseChance - (level - 1) * decreasePerLevel, 0.05f);

        return successChance;
    }

    // 揶쏅벤??甕곌쑵????????源??
    private void UpgradeStat(StatsSet stat)
    {
        int resourceCostValue = int.Parse(stat.resourceCost.text.Replace("???걟 ????: ", "").Trim());

        if (currentGold < resourceCostValue)
        {
            Debug.Log("???봔鈺?");
            return;
        }

        // ?源껊궗 ?類ｌぇ????곕┛ ?袁る퉸 text?????뼓
        float chance = float.Parse(stat.successChance.text.Replace("揶쏅벤???類ｌぇ : ", "").Replace("%", "")) / 100;
        bool isSuccess = Random.value <= chance;

        if (isSuccess)
        {
            // 揶쏅벤???源껊궗 ??UI ??낅쑓??꾨뱜
            float current = float.Parse(stat.currentStats.text.Replace("?袁⑹삺 ??쎄틛 : ", ""));
            float upgradeValue = float.Parse(stat.statsUpgradeValue.text.Replace("?源놁삢揶?: +", ""));
            stat.UpdateUI(
                stat.statsName.text,
                current + upgradeValue,
                upgradeValue,
                chance,
                int.Parse(stat.resourceCost.text.Replace("???걟 ????: ", ""))
            );
            Debug.Log($"{stat.statsName.text} 揶쏅벤???源껊궗!");

            // Firebase?癒?퐣 ?怨쀬뵠????낅쑓??꾨뱜
            FirebaseManager.Instance.UpgradeCharacter(stat.statsName.text);  // 揶쏅벤?????쎈??Firebase??獄쏆꼷??

            // 揶쏅벤???源껊궗 ?? ???걟??疫뀀뜆釉몌쭕?곌껍 ?ⓥ뫀諭띄몴?筌△몿而?
            currentGold -= resourceCostValue;

            // ?ⓥ뫀諭?UI ??낅쑓??꾨뱜
            UpdateGoldUI();  // ?ⓥ뫀諭?UI??揶쏄퉮??
        }
        else
        {
            Debug.Log($"{stat.statsName.text} 揶쏅벤????쎈솭...");
        }
    }
}
