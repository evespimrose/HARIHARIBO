using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RiskUIController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject riskCardPrefab;    // 由ъ뒪??移대뱶 ?꾨━??
    [SerializeField] private GameObject playerSlotPrefab;  // ?뚮젅?댁뼱 ?щ’ ?꾨━??

    [Header("Header UI")]
    [SerializeField] private TextMeshProUGUI waveInfoText;    // ?⑥씠釉??뺣낫
    [SerializeField] private TextMeshProUGUI goldText;        // 怨⑤뱶 ?띿뒪??
    [SerializeField] private TextMeshProUGUI riskListText;    // 由ъ뒪??紐⑸줉
    [SerializeField] private TextMeshProUGUI selectTimeText;  // ?좏깮 ?쒓컙

    [Header("Containers")]
    [SerializeField] private Transform cardContainer;      // 由ъ뒪??移대뱶?ㅼ씠 ?앹꽦??遺紐??ㅻ툕?앺듃
    [SerializeField] private Transform playerContainer;    // ?뚮젅?댁뼱 ?щ’?ㅼ씠 ?앹꽦??遺紐??ㅻ툕?앺듃

    [Header("UI References")]
    [SerializeField] private Button surrenderButton;       // ?ш린 踰꾪듉
    [SerializeField] private TextMeshProUGUI voteCountText; // ?ы몴 ???띿뒪??

    private List<GameObject> riskCards = new List<GameObject>();    // ?앹꽦??由ъ뒪??移대뱶??
    private List<GameObject> playerSlots = new List<GameObject>();  // ?앹꽦???뚮젅?댁뼱 ?щ’??
    private RiskData[] risks = new RiskData[3];                    // 由ъ뒪???곗씠??諛곗뿴
    private bool isVoting = false;                                 // ?ы몴 吏꾪뻾 以??щ?
    private int selectedCard = -1;                                 // ?좏깮??移대뱶 ?몃뜳??
    private int playerCount = 0;                                   // ?꾩옱 ?뚮젅?댁뼱 ??
    private int surrenderVotes = 0;                                // ?ш린 ?ы몴 ??

    private void Start()
    {
        InitializeUI();
        SetupButtons();
    }

    // UI 珥덇린??
    private void InitializeUI()
    {
        // ?뚯뒪?몄슜 由ъ뒪???곗씠???앹꽦
        for (int i = 0; i < 3; i++)
        {
            risks[i] = new RiskData
            {
                riskName = $"Risk {i + 1}",
                description = $"Risk Description {i + 1}",
                multiplier = 1.5f + (i * 0.5f)
            };
        }

        CreateRiskCards();     // 由ъ뒪??移대뱶 ?앹꽦
        CreatePlayerSlots();   // ?뚮젅?댁뼱 ?щ’ ?앹꽦
        UpdateVoteCountText(); // ?ы몴 ???띿뒪??珥덇린??
    }

    public void UpdateWaveInfo(string waveInfo)
    {
        waveInfoText.text = waveInfo;
    }

    // 怨⑤뱶 ?뺣낫 ?낅뜲?댄듃
    public void UpdateGold(int gold)
    {
        goldText.text = $"Gold: {gold:WON}";
    }

    // 由ъ뒪??紐⑸줉 ?낅뜲?댄듃
    public void UpdateRiskList(string riskList)
    {
        riskListText.text = riskList;
    }

    // ?좏깮 ?쒓컙 ?낅뜲?댄듃
    public void UpdateSelectTime(float time)
    {
        selectTimeText.text = $"Time: {time:F1}";
    }

    // 踰꾪듉 ?대깽???ㅼ젙
    private void SetupButtons()
    {
        // ?ш린 踰꾪듉 ?대깽???곌껐
        surrenderButton.onClick.AddListener(OnSurrenderClick);
    }

    // 由ъ뒪??移대뱶 ?앹꽦
    private void CreateRiskCards()
    {
        // 湲곗〈 移대뱶 ?쒓굅
        foreach (var card in riskCards)
        {
            DestroyImmediate(card);
        }
        riskCards.Clear();

        // ??移대뱶 ?앹꽦
        for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(riskCardPrefab, cardContainer);
            riskCards.Add(card);

            // 移대뱶 ?뺣낫 ?ㅼ젙
            SetupRiskCard(card, risks[i], i);
        }
    }

    // 媛쒕퀎 由ъ뒪??移대뱶 ?ㅼ젙
    private void SetupRiskCard(GameObject card, RiskData risk, int index)
    {
        // ?꾨━?뱀쓽 而댄룷?뚰듃??媛?몄삤湲?
        var nameText = card.GetComponentInChildren<TextMeshProUGUI>(true);
        var descText = card.GetComponentsInChildren<TextMeshProUGUI>(true)[1];
        var multiplierText = card.GetComponentsInChildren<TextMeshProUGUI>(true)[2];
        var cardButton = card.GetComponent<Button>();

        // ?띿뒪???ㅼ젙
        if (nameText != null) nameText.text = risk.riskName;
        if (descText != null) descText.text = risk.description;
        if (multiplierText != null) multiplierText.text = $"x{risk.multiplier}";

        // 踰꾪듉 ?대깽???ㅼ젙
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(() => OnCardClick(index));
        }
    }

    // ?뚮젅?댁뼱 ?щ’ ?앹꽦
    private void CreatePlayerSlots()
    {
        // ?뚯뒪?몄슜 ?뚮젅?댁뼱 ???ㅼ젙
        playerCount = 4; // ?ㅼ젣 寃뚯엫?먯꽌???ㅽ듃?뚰겕?먯꽌 諛쏆븘?????

        // 湲곗〈 ?щ’ ?쒓굅
        foreach (var slot in playerSlots)
        {
            Destroy(slot);
        }
        playerSlots.Clear();

        // ???щ’ ?앹꽦
        for (int i = 0; i < playerCount; i++)
        {
            GameObject slot = Instantiate(playerSlotPrefab, playerContainer);
            playerSlots.Add(slot);
            SetupPlayerSlot(slot, i);
        }
    }

    // 媛쒕퀎 ?뚮젅?댁뼱 ?щ’ ?ㅼ젙
    private void SetupPlayerSlot(GameObject slot, int playerIndex)
    {
        //// ?뚮젅?댁뼱 ?뺣낫 ?ㅼ젙 (?ㅼ젣 寃뚯엫?먯꽌???ㅽ듃?뚰겕?먯꽌 諛쏆븘?????
        //TextMeshProUGUI nameText = slot.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
        //nameText.text = $"Player {playerIndex + 1}";

        // ?꾩옱 ?꾨━??援ъ“??留욊쾶 寃쎈줈 ?섏젙
        var nameText = slot.transform.Find("Player Name").GetComponent<TextMeshProUGUI>();
        var characterIcon = slot.transform.Find("Character Icon").GetComponent<Image>();
        var healthBar = slot.transform.Find("Slider").GetComponent<Slider>();


        // null 泥댄겕 ???ㅼ젙
        if (nameText != null)
        {
            nameText.text = $"Player {playerIndex + 1}";
        }

        if (healthBar != null)
        {
            healthBar.value = 1f;  // 珥덇린 泥대젰 理쒕?移?
        }

        if (characterIcon != null)
        {
            // characterIcon.sprite = 罹먮┃???ㅽ봽?쇱씠???ㅼ젙
        }
    }

    // 由ъ뒪??移대뱶 ?대┃ 泥섎━
    private void OnCardClick(int index)
    {
        if (isVoting) return;

        selectedCard = index;
        isVoting = true;

        // UI ?낅뜲?댄듃
        UpdateCardVisuals();
        surrenderButton.interactable = false;
    }

    // ?ш린 踰꾪듉 ?대┃ 泥섎━
    private void OnSurrenderClick()
    {
        if (isVoting) return;

        surrenderVotes++;
        UpdateVoteCountText();
        surrenderButton.interactable = false;

        // 紐⑤뱺 ?뚮젅?댁뼱媛 ?ш린???숈쓽?덈뒗吏 ?뺤씤
        if (surrenderVotes >= playerCount)
        {
            OnSurrenderConfirmed();
        }
    }

    // 移대뱶 鍮꾩＜???낅뜲?댄듃
    private void UpdateCardVisuals()
    {
        for (int i = 0; i < riskCards.Count; i++)
        {
            var cardImage = riskCards[i].GetComponent<Image>();
            cardImage.color = (i == selectedCard) ? Color.yellow : Color.gray;
        }
    }

    // ?ы몴 ???띿뒪???낅뜲?댄듃
    private void UpdateVoteCountText()
    {
        voteCountText.text = $"{surrenderVotes}/{playerCount}";
    }

    // ?ш린 ?뺤젙 泥섎━
    private void OnSurrenderConfirmed()
    {
        Debug.Log("寃뚯엫 ?ш린 ?뺤젙!");
        // ?ш린??寃뚯엫 醫낅즺 濡쒖쭅 異붽?
    }

    // ?ы몴 ?곹깭 珥덇린??(?몃??먯꽌 ?몄텧 媛??
    public void ResetVoting()
    {
        isVoting = false;
        selectedCard = -1;
        surrenderVotes = 0;
        UpdateVoteCountText();
        surrenderButton.interactable = true;

        // 移대뱶 ?됱긽 珥덇린??
        foreach (var card in riskCards)
        {
            card.GetComponent<Image>().color = Color.white;
        }
    }

    // ?뚮젅?댁뼱 泥대젰 ?낅뜲?댄듃 (?몃??먯꽌 ?몄텧 媛??
    public void UpdatePlayerHealth(int playerIndex, float healthPercent)
    {
        if (playerIndex < playerSlots.Count)
        {
            var healthBar = playerSlots[playerIndex].transform.Find("HealthBar").GetComponent<Slider>();
            healthBar.value = healthPercent;
        }
    }
}
