using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RiskUIController : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Button surrenderButton;
    [SerializeField] private TextMeshProUGUI voteCountText;

    private RiskData[] risks;
    private int selectedCard = -1;
    private bool isVoting = false;

    private const string RISK_VOTES_KEY = "RiskVotes";
    private const string SURRENDER_VOTES_KEY = "SurrenderVotes";

    private void Start()
    {
        if (surrenderButton != null)
        {
            surrenderButton.onClick.AddListener(OnSurrenderClick);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            InitializeRiskData();
        }
    }

    private void InitializeRiskData()
    {
        risks = new RiskData[]
        {
            new RiskData { riskId = 1, riskName = "리스크 1", description = "설명 1", multiplier = 1.5f },
            new RiskData { riskId = 2, riskName = "리스크 2", description = "설명 2", multiplier = 2.0f },
            new RiskData { riskId = 3, riskName = "리스크 3", description = "설명 3", multiplier = 2.5f }
        };

        // 초기 투표 데이터 설정
        var initialVotes = new ExitGames.Client.Photon.Hashtable
        {
            { RISK_VOTES_KEY, new Dictionary<int, int>() },
            { SURRENDER_VOTES_KEY, new HashSet<int>() }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(initialVotes);

        CreateRiskCards();
    }

    private void CreateRiskCards()
    {
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < risks.Length; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            RiskCard card = cardObj.GetComponent<RiskCard>();
            card.Initialize(risks[i], i);

            int index = i;
            card.SelectButton.onClick.AddListener(() => OnCardClick(index));
            card.CancelButton.onClick.AddListener(() => OnCardClick(index));
        }
    }

    private void OnCardClick(int index)
    {
        if (isVoting) return;

        selectedCard = index;
        isVoting = true;

        // 현재 투표 상태 가져오기
        var votes = (Dictionary<int, int>)PhotonNetwork.CurrentRoom.CustomProperties[RISK_VOTES_KEY];
        votes[PhotonNetwork.LocalPlayer.ActorNumber] = risks[index].riskId;

        // 투표 정보 업데이트
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { RISK_VOTES_KEY, votes }
        });

        UpdateCardVisuals();
        surrenderButton.interactable = false;
    }

    private void OnSurrenderClick()
    {
        if (isVoting) return;

        var surrenderVotes = (HashSet<int>)PhotonNetwork.CurrentRoom.CustomProperties[SURRENDER_VOTES_KEY];
        surrenderVotes.Add(PhotonNetwork.LocalPlayer.ActorNumber);

        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { SURRENDER_VOTES_KEY, surrenderVotes }
        });

        surrenderButton.interactable = false;
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(RISK_VOTES_KEY))
        {
            var votes = (Dictionary<int, int>)propertiesThatChanged[RISK_VOTES_KEY];
            UpdateVoteCounts(votes);
        }

        if (propertiesThatChanged.ContainsKey(SURRENDER_VOTES_KEY))
        {
            var surrenderVotes = (HashSet<int>)propertiesThatChanged[SURRENDER_VOTES_KEY];
            UpdateSurrenderVotes(surrenderVotes);
        }
    }

    private void UpdateVoteCounts(Dictionary<int, int> votes)
    {
        // 각 카드의 플레이어 투표 상태 업데이트
        foreach (RiskCard card in cardContainer.GetComponentsInChildren<RiskCard>())
        {
            // 각 플레이어의 투표 여부 확인
            foreach (var vote in votes)
            {
                int playerIndex = vote.Key - 1; // ActorNumber는 1부터 시작하므로 0-based index로 변환
                bool votedForThisCard = vote.Value == card.RiskId;
                card.UpdatePlayerVote(playerIndex, votedForThisCard);
            }
        }

        // 모든 플레이어가 투표했는지 확인
        if (votes.Count == PhotonNetwork.CurrentRoom.PlayerCount && PhotonNetwork.IsMasterClient)
        {
            FinalizeVoting(votes);
        }
    }


    private void UpdateSurrenderVotes(HashSet<int> surrenderVotes)
    {
        voteCountText.text = $"항복 투표: {surrenderVotes.Count}/{PhotonNetwork.CurrentRoom.PlayerCount}";

        if (surrenderVotes.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            OnSurrenderConfirmed();
        }
    }

    private void OnSurrenderConfirmed()
    {
        Debug.Log("항복 투표가 통과되었습니다.");
        // 게임 매니저에 항복 처리 요청
        // TODO: 게임 매니저에 항복 처리 메서드 구현 필요
    }

    private void FinalizeVoting(Dictionary<int, int> votes)
    {
        var mostVotedRisk = votes.GroupBy(x => x.Value)
                                .OrderByDescending(g => g.Count())
                                .First()
                                .Key;

        // 선택된 리스크 효과 적용
        var selectedRisk = risks.First(r => r.riskId == mostVotedRisk);
        OnRiskSelected(selectedRisk);
    }

    private void OnRiskSelected(RiskData selectedRisk)
    {
        Debug.Log($"선택된 리스크: {selectedRisk.riskName}");
        // TODO: 선택된 리스크 효과 적용 로직 구현 필요
    }

    private void UpdateCardVisuals()
    {
        foreach (RiskCard card in cardContainer.GetComponentsInChildren<RiskCard>())
        {
            card.SetSelected(card.RiskId == selectedCard);
        }
    }
}
