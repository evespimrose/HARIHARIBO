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
            new RiskData { riskId = 1, riskName = "위험요소1", description = "설명 1", multiplier = 1.5f },
            new RiskData { riskId = 2, riskName = "위험요소2", description = "설명 2", multiplier = 2.0f },
            new RiskData { riskId = 3, riskName = "위험요소3", description = "설명 3", multiplier = 2.5f }
        };

        // ?λ뜃由???紐??怨쀬뵠????쇱젟
        var initialVotes = new ExitGames.Client.Photon.Hashtable
        {
            { RISK_VOTES_KEY, new Dictionary<int, int>() },
            { SURRENDER_VOTES_KEY, new Dictionary<int, bool>() }
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

        // ?袁⑹삺 ??紐??怨밴묶 揶쎛?紐꾩궎疫?
        var votes = (Dictionary<int, int>)PhotonNetwork.CurrentRoom.CustomProperties[RISK_VOTES_KEY];
        votes[PhotonNetwork.LocalPlayer.ActorNumber] = risks[index].riskId;

        // ??紐??類ｋ궖 ??낅쑓??꾨뱜
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

        var surrenderVotes = (Dictionary<int, bool>)PhotonNetwork.CurrentRoom.CustomProperties[SURRENDER_VOTES_KEY];
        surrenderVotes[PhotonNetwork.LocalPlayer.ActorNumber] = true;

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
        // [변경] HashSet에서 Dictionary로 타입 변경
        if (propertiesThatChanged.ContainsKey(SURRENDER_VOTES_KEY))
        {
            var surrenderVotes = (Dictionary<int, bool>)propertiesThatChanged[SURRENDER_VOTES_KEY];
            UpdateSurrenderVotes(surrenderVotes);
        }
    }

    private void UpdateVoteCounts(Dictionary<int, int> votes)
    {
        // 揶?燁삳?諭?????쟿??곷선 ??紐??怨밴묶 ??낅쑓??꾨뱜
        foreach (RiskCard card in cardContainer.GetComponentsInChildren<RiskCard>())
        {
            // 揶????쟿??곷선????紐???? ?類ㅼ뵥
            foreach (var vote in votes)
            {
                int playerIndex = vote.Key - 1; // ActorNumber??1?봔????뽰삂???嚥?0-based index嚥?癰궰??
                bool votedForThisCard = vote.Value == card.RiskId;
                card.UpdatePlayerVote(playerIndex, votedForThisCard);
            }
        }

        // 筌뤴뫀諭????쟿??곷선揶쎛 ??紐??덈뮉筌왖 ?類ㅼ뵥
        if (votes.Count == PhotonNetwork.CurrentRoom.PlayerCount && PhotonNetwork.IsMasterClient)
        {
            FinalizeVoting(votes);
        }
    }


    private void UpdateSurrenderVotes(Dictionary<int, bool> surrenderVotes)
    {
        int voteCount = surrenderVotes.Values.Count(v => v);
        voteCountText.text = $"항복 투표: {voteCount}/{PhotonNetwork.CurrentRoom.PlayerCount}";

        if (voteCount >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            OnSurrenderConfirmed();
        }
    }

    private void OnSurrenderConfirmed()
    {
        ProcessSurrender();

        gameObject.SetActive(false);
    }

    private void FinalizeVoting(Dictionary<int, int> votes)
    {
        var mostVotedRisk = votes.GroupBy(x => x.Value)
                                .OrderByDescending(g => g.Count())
                                .First()
                                .Key;

        // ?醫뤾문???귐딅뮞????ｋ궢 ?怨몄뒠
        var selectedRisk = risks.First(r => r.riskId == mostVotedRisk);
        OnRiskSelected(selectedRisk);
    }

    private void OnRiskSelected(RiskData selectedRisk)
    {
        Debug.Log($"?醫뤾문???귐딅뮞?? {selectedRisk.riskName}");
        // TODO: ?醫뤾문???귐딅뮞????ｋ궢 ?怨몄뒠 嚥≪뮇彛??닌뗭겱 ?袁⑹뒄
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
                float baseReward = 1000f; // 疫꿸퀡??癰귣똻湲?
                float levelMultiplier = player.Stats.level * 0.1f; // ??덇볼 癰귣?瑗??
                float surrenderPenalty = isSurrender ? 0.5f : 1f; // ??????롪섯??

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
