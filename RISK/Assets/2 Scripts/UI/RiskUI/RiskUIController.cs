using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RiskUIController : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject riskCardPrefab;    // 리스크 카드 프리팹
    [SerializeField] private GameObject playerSlotPrefab;  // 플레이어 슬롯 프리팹

    [Header("Header UI")]
    [SerializeField] private TextMeshProUGUI waveInfoText;    // 웨이브 정보
    [SerializeField] private TextMeshProUGUI goldText;        // 골드 텍스트
    [SerializeField] private TextMeshProUGUI riskListText;    // 리스크 목록
    [SerializeField] private TextMeshProUGUI selectTimeText;  // 선택 시간

    [Header("Containers")]
    [SerializeField] private Transform cardContainer;      // 리스크 카드들이 생성될 부모 오브젝트
    [SerializeField] private Transform playerContainer;    // 플레이어 슬롯들이 생성될 부모 오브젝트

    [Header("UI References")]
    [SerializeField] private Button surrenderButton;       // 포기 버튼
    [SerializeField] private TextMeshProUGUI voteCountText; // 투표 수 텍스트

    private List<GameObject> riskCards = new List<GameObject>();    // 생성된 리스크 카드들
    private List<GameObject> playerSlots = new List<GameObject>();  // 생성된 플레이어 슬롯들
    private RiskData[] risks = new RiskData[3];                    // 리스크 데이터 배열
    private bool isVoting = false;                                 // 투표 진행 중 여부
    private int selectedCard = -1;                                 // 선택된 카드 인덱스
    private int playerCount = 0;                                   // 현재 플레이어 수
    private int surrenderVotes = 0;                                // 포기 투표 수

    private void Start()
    {
        InitializeUI();
        SetupButtons();
    }

    // UI 초기화
    private void InitializeUI()
    {
        // 테스트용 리스크 데이터 생성
        for (int i = 0; i < 3; i++)
        {
            risks[i] = new RiskData
            {
                riskName = $"Risk {i + 1}",
                description = $"Risk Description {i + 1}",
                multiplier = 1.5f + (i * 0.5f)
            };
        }

        CreateRiskCards();     // 리스크 카드 생성
        CreatePlayerSlots();   // 플레이어 슬롯 생성
        UpdateVoteCountText(); // 투표 수 텍스트 초기화
    }

    public void UpdateWaveInfo(string waveInfo)
    {
        waveInfoText.text = waveInfo;
    }

    // 골드 정보 업데이트
    public void UpdateGold(int gold)
    {
        goldText.text = $"Gold: {gold:WON}";
    }

    // 리스크 목록 업데이트
    public void UpdateRiskList(string riskList)
    {
        riskListText.text = riskList;
    }

    // 선택 시간 업데이트
    public void UpdateSelectTime(float time)
    {
        selectTimeText.text = $"Time: {time:F1}";
    }

    // 버튼 이벤트 설정
    private void SetupButtons()
    {
        // 포기 버튼 이벤트 연결
        surrenderButton.onClick.AddListener(OnSurrenderClick);
    }

    // 리스크 카드 생성
    private void CreateRiskCards()
    {
        // 기존 카드 제거
        foreach (var card in riskCards)
        {
            DestroyImmediate(card);
        }
        riskCards.Clear();

        // 새 카드 생성
        for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(riskCardPrefab, cardContainer);
            riskCards.Add(card);

            // 카드 정보 설정
            SetupRiskCard(card, risks[i], i);
        }
    }

    // 개별 리스크 카드 설정
    private void SetupRiskCard(GameObject card, RiskData risk, int index)
    {
        // 프리팹의 컴포넌트들 가져오기
        var nameText = card.GetComponentInChildren<TextMeshProUGUI>(true);
        var descText = card.GetComponentsInChildren<TextMeshProUGUI>(true)[1];
        var multiplierText = card.GetComponentsInChildren<TextMeshProUGUI>(true)[2];
        var cardButton = card.GetComponent<Button>();

        // 텍스트 설정
        if (nameText != null) nameText.text = risk.riskName;
        if (descText != null) descText.text = risk.description;
        if (multiplierText != null) multiplierText.text = $"x{risk.multiplier}";

        // 버튼 이벤트 설정
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(() => OnCardClick(index));
        }
    }

    // 플레이어 슬롯 생성
    private void CreatePlayerSlots()
    {
        // 테스트용 플레이어 수 설정
        playerCount = 4; // 실제 게임에서는 네트워크에서 받아와야 함

        // 기존 슬롯 제거
        foreach (var slot in playerSlots)
        {
            Destroy(slot);
        }
        playerSlots.Clear();

        // 새 슬롯 생성
        for (int i = 0; i < playerCount; i++)
        {
            GameObject slot = Instantiate(playerSlotPrefab, playerContainer);
            playerSlots.Add(slot);
            SetupPlayerSlot(slot, i);
        }
    }

    // 개별 플레이어 슬롯 설정
    private void SetupPlayerSlot(GameObject slot, int playerIndex)
    {
        //// 플레이어 정보 설정 (실제 게임에서는 네트워크에서 받아와야 함)
        //TextMeshProUGUI nameText = slot.transform.Find("PlayerName").GetComponent<TextMeshProUGUI>();
        //nameText.text = $"Player {playerIndex + 1}";

        // 현재 프리팹 구조에 맞게 경로 수정
        var nameText = slot.transform.Find("Player Name").GetComponent<TextMeshProUGUI>();
        var characterIcon = slot.transform.Find("Character Icon").GetComponent<Image>();
        var healthBar = slot.transform.Find("Slider").GetComponent<Slider>();
        

        // null 체크 후 설정
        if (nameText != null)
        {
            nameText.text = $"Player {playerIndex + 1}";
        }

        if (healthBar != null)
        {
            healthBar.value = 1f;  // 초기 체력 최대치
        }

        if (characterIcon != null)
        {
            // characterIcon.sprite = 캐릭터 스프라이트 설정
        }
    }

    // 리스크 카드 클릭 처리
    private void OnCardClick(int index)
    {
        if (isVoting) return;

        selectedCard = index;
        isVoting = true;

        // UI 업데이트
        UpdateCardVisuals();
        surrenderButton.interactable = false;
    }

    // 포기 버튼 클릭 처리
    private void OnSurrenderClick()
    {
        if (isVoting) return;

        surrenderVotes++;
        UpdateVoteCountText();
        surrenderButton.interactable = false;

        // 모든 플레이어가 포기에 동의했는지 확인
        if (surrenderVotes >= playerCount)
        {
            OnSurrenderConfirmed();
        }
    }

    // 카드 비주얼 업데이트
    private void UpdateCardVisuals()
    {
        for (int i = 0; i < riskCards.Count; i++)
        {
            var cardImage = riskCards[i].GetComponent<Image>();
            cardImage.color = (i == selectedCard) ? Color.yellow : Color.gray;
        }
    }

    // 투표 수 텍스트 업데이트
    private void UpdateVoteCountText()
    {
        voteCountText.text = $"{surrenderVotes}/{playerCount}";
    }

    // 포기 확정 처리
    private void OnSurrenderConfirmed()
    {
        Debug.Log("게임 포기 확정!");
        // 여기에 게임 종료 로직 추가
    }

    // 투표 상태 초기화 (외부에서 호출 가능)
    public void ResetVoting()
    {
        isVoting = false;
        selectedCard = -1;
        surrenderVotes = 0;
        UpdateVoteCountText();
        surrenderButton.interactable = true;

        // 카드 색상 초기화
        foreach (var card in riskCards)
        {
            card.GetComponent<Image>().color = Color.white;
        }
    }

    // 플레이어 체력 업데이트 (외부에서 호출 가능)
    public void UpdatePlayerHealth(int playerIndex, float healthPercent)
    {
        if (playerIndex < playerSlots.Count)
        {
            var healthBar = playerSlots[playerIndex].transform.Find("HealthBar").GetComponent<Slider>();
            healthBar.value = healthPercent;
        }
    }
}
