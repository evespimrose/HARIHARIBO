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
            new RiskData { riskId = 1, riskName = "由ъ뒪??1", description = "?ㅻ챸 1", multiplier = 1.5f },
            new RiskData { riskId = 2, riskName = "由ъ뒪??2", description = "?ㅻ챸 2", multiplier = 2.0f },
            new RiskData { riskId = 3, riskName = "由ъ뒪??3", description = "?ㅻ챸 3", multiplier = 2.5f }
        };

        // 珥덇린 ?ы몴 ?곗씠???ㅼ젙
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

        // ?꾩옱 ?ы몴 ?곹깭 媛?몄삤湲?
        var votes = (Dictionary<int, int>)PhotonNetwork.CurrentRoom.CustomProperties[RISK_VOTES_KEY];
        votes[PhotonNetwork.LocalPlayer.ActorNumber] = risks[index].riskId;

        // ?ы몴 ?뺣낫 ?낅뜲?댄듃
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
        // 媛?移대뱶???뚮젅?댁뼱 ?ы몴 ?곹깭 ?낅뜲?댄듃
        foreach (RiskCard card in cardContainer.GetComponentsInChildren<RiskCard>())
        {
            // 媛??뚮젅?댁뼱???ы몴 ?щ? ?뺤씤
            foreach (var vote in votes)
            {
                int playerIndex = vote.Key - 1; // ActorNumber??1遺???쒖옉?섎?濡?0-based index濡?蹂??
                bool votedForThisCard = vote.Value == card.RiskId;
                card.UpdatePlayerVote(playerIndex, votedForThisCard);
            }
        }

        // 紐⑤뱺 ?뚮젅?댁뼱媛 ?ы몴?덈뒗吏 ?뺤씤
        if (votes.Count == PhotonNetwork.CurrentRoom.PlayerCount && PhotonNetwork.IsMasterClient)
        {
            FinalizeVoting(votes);
        }
    }


    private void UpdateSurrenderVotes(HashSet<int> surrenderVotes)
    {
        voteCountText.text = $"??났 ?ы몴: {surrenderVotes.Count}/{PhotonNetwork.CurrentRoom.PlayerCount}";

        if (surrenderVotes.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            OnSurrenderConfirmed();
        }
    }

    private void OnSurrenderConfirmed()
    {
        Debug.Log("??났 ?ы몴媛 ?듦낵?섏뿀?듬땲??");
        // 寃뚯엫 留ㅻ땲?????났 泥섎━ ?붿껌
        // TODO: 寃뚯엫 留ㅻ땲?????났 泥섎━ 硫붿꽌??援ы쁽 ?꾩슂

        ProcessSurrender();

        // UI 鍮꾪솢?깊솕
        gameObject.SetActive(false);
    }

    private void FinalizeVoting(Dictionary<int, int> votes)
    {
        var mostVotedRisk = votes.GroupBy(x => x.Value)
                                .OrderByDescending(g => g.Count())
                                .First()
                                .Key;

        // ?좏깮??由ъ뒪???④낵 ?곸슜
        var selectedRisk = risks.First(r => r.riskId == mostVotedRisk);
        OnRiskSelected(selectedRisk);
    }

    private void OnRiskSelected(RiskData selectedRisk)
    {
        Debug.Log($"?좏깮??由ъ뒪?? {selectedRisk.riskName}");
        // TODO: ?좏깮??由ъ뒪???④낵 ?곸슜 濡쒖쭅 援ы쁽 ?꾩슂
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
                float baseReward = 1000f; // 湲곕낯 蹂댁긽
                float levelMultiplier = player.Stats.level * 0.1f; // ?덈꺼 蹂대꼫??
                float surrenderPenalty = isSurrender ? 0.5f : 1f; // ??났 ?섎꼸??

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
