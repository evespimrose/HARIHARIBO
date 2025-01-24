using Newtonsoft.Json;
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
            surrenderButton.onClick.AddListener(OnSurrenderClick);

        if (PhotonNetwork.IsMasterClient)
            InitializeRiskData();
    }

    private void InitializeRiskData()
    {
        risks = new RiskData[]
        {
            new RiskData { riskId = 1, riskName = "?洹먮봾裕??1", description = "???닿뎄 1", multiplier = 1.5f },
            new RiskData { riskId = 2, riskName = "?洹먮봾裕??2", description = "???닿뎄 2", multiplier = 2.0f },
            new RiskData { riskId = 3, riskName = "?洹먮봾裕??3", description = "???닿뎄 3", multiplier = 2.5f }
        };

        // ?貫?껆뵳???筌???⑥щ턄?????깆젧
        var initialVotes = new ExitGames.Client.Photon.Hashtable
        {
            { RISK_VOTES_KEY, new Dictionary<int, int>() },
            { SURRENDER_VOTES_KEY, new int[]{ } }
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

        // ?熬곣뫗????筌???⑤객臾??띠럾??筌뤾쑴沅롧뼨?
        var votes = (Dictionary<int, int>)PhotonNetwork.CurrentRoom.CustomProperties[RISK_VOTES_KEY];
        votes[PhotonNetwork.LocalPlayer.ActorNumber] = risks[index].riskId;

        // ??筌??筌먲퐢沅????낆몥??袁⑤콦
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

        var surrenderVotes = (List<int>)PhotonNetwork.CurrentRoom.CustomProperties[SURRENDER_VOTES_KEY];
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
            var surrenderVotes = (List<int>)propertiesThatChanged[SURRENDER_VOTES_KEY];
            UpdateSurrenderVotes(surrenderVotes);
        }
    }

    private void UpdateVoteCounts(Dictionary<int, int> votes)
    {
        // ???곸궠?獄????????怨룹꽑 ??筌???⑤객臾????낆몥??袁⑤콦
        foreach (RiskCard card in cardContainer.GetComponentsInChildren<RiskCard>())
        {
            // ????????怨룹꽑????筌???? ?筌먦끉逾?
            foreach (var vote in votes)
            {
                int playerIndex = vote.Key - 1; // ActorNumber??1?遊붋????戮곗굚?????0-based index???곌떠???
                bool votedForThisCard = vote.Value == card.RiskId;
                card.UpdatePlayerVote(playerIndex, votedForThisCard);
            }
        }

        // 嶺뚮ㅄ維獄???????怨룹꽑?띠럾? ??筌???덈츎嶺뚯솘? ?筌먦끉逾?
        if (votes.Count == PhotonNetwork.CurrentRoom.PlayerCount && PhotonNetwork.IsMasterClient)
        {
            FinalizeVoting(votes);
        }
    }


    private void UpdateSurrenderVotes(List<int> surrenderVotes)
    {
        voteCountText.text = $"??????筌? {surrenderVotes.Count}/{PhotonNetwork.CurrentRoom.PlayerCount}";

        if (surrenderVotes.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            OnSurrenderConfirmed();
        }
    }

    private void OnSurrenderConfirmed()
    {
        Debug.Log("??????筌뤿떣泥? ???沅??琉????鍮??");
        // ?롪퍓???嶺뚮씞??????????嶺뚳퐣瑗????븐슙??
        // TODO: ?롪퍓???嶺뚮씞??????????嶺뚳퐣瑗??嶺뚮∥?꾥땻????뚮뿭寃??熬곣뫗??

        ProcessSurrender();

        // UI ?????繹먮봿??
        gameObject.SetActive(false);
    }

    private void FinalizeVoting(Dictionary<int, int> votes)
    {
        var mostVotedRisk = votes.GroupBy(x => x.Value)
                                .OrderByDescending(g => g.Count())
                                .First()
                                .Key;

        // ??ルㅎ臾???洹먮봾裕????節뗪땁 ??⑤챷??
        var selectedRisk = risks.First(r => r.riskId == mostVotedRisk);
        OnRiskSelected(selectedRisk);
    }

    private void OnRiskSelected(RiskData selectedRisk)
    {
        Debug.Log($"??ルㅎ臾???洹먮봾裕?? {selectedRisk.riskName}");
        // TODO: ??ルㅎ臾???洹먮봾裕????節뗪땁 ??⑤챷???β돦裕뉐퐲???뚮뿭寃??熬곣뫗??
        ApplyRiskEffect(selectedRisk);

        gameObject.SetActive(false);

        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.Instance.isWaveDone = false;
        }
    }

    private void UpdateCardVisuals()
    {
        foreach (RiskCard card in cardContainer.GetComponentsInChildren<RiskCard>())
        {
            card.SetSelected(card.RiskId == selectedCard);
        }
    }

    public void ApplyRiskEffect(RiskData selectedRisk)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("ApplyRiskEffectRPC", RpcTarget.All, JsonConvert.SerializeObject(selectedRisk));
    }

    [PunRPC]
    private void ApplyRiskEffectRPC(string riskDataJson)
    {
        RiskData risk = JsonConvert.DeserializeObject<RiskData>(riskDataJson);

        foreach (var playerObj in UnitManager.Instance.players.Values)
        {
            if (playerObj.TryGetComponent(out Player player))
            {
                PlayerStats stats = player.Stats;

                switch (risk.riskId)
                {
                    case 1:
                        stats.maxHealth *= 0.8f;
                        stats.currentHealth = Mathf.Min(stats.currentHealth, stats.maxHealth);
                        break;
                    case 2:
                        stats.attackPower *= 1.3f;
                        stats.damageReduction *= 0.7f;
                        break;
                    case 3:
                        stats.moveSpeed *= 1.2f;
                        stats.healthRegen *= 0.5f;
                        break;
                }
            }
        }
    }

    public void ProcessSurrender()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("ProcessSurrenderRPC", RpcTarget.All);
    }

    [PunRPC]
    private void ProcessSurrenderRPC()
    {
        GameManager.Instance.isWaveDone = true;

        CalculateRewards(true);
    }

    private void CalculateRewards(bool isSurrender)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Dictionary<string, float> playerRewards = new Dictionary<string, float>();

        foreach (var playerObj in UnitManager.Instance.players.Values)
        {
            if (playerObj.TryGetComponent(out Player player))
            {
                float baseReward = 1000f; // ?リ옇????곌랜?삥묾?
                float levelMultiplier = player.Stats.level * 0.1f; // ???뉖낵 ?곌랜????
                float surrenderPenalty = isSurrender ? 0.5f : 1f; // ??????濡れ꽢??

                float finalReward = baseReward * (1 + levelMultiplier) * surrenderPenalty;
                playerRewards.Add(player.Stats.nickName, finalReward);
            }
        }

        photonView.RPC("UpdatePlayerRewardsRPC", RpcTarget.All, JsonConvert.SerializeObject(playerRewards));
    }

    [PunRPC]
    private void UpdatePlayerRewardsRPC(string rewardsJson)
    {
        Dictionary<string, float> rewards = JsonConvert.DeserializeObject<Dictionary<string, float>>(rewardsJson);

        foreach (var reward in rewards)
        {

            if (reward.Key == FirebaseManager.Instance.currentCharacterData.nickName)
            {
                // TODO: Update reward to Firebase Database
            }
        }
    }
}
