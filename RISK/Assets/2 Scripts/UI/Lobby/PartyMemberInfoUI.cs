using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using PhotonRealtimePlayer = Photon.Realtime.Player;


public class PartyMemberInfoUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private Image partyLeaderIcon;

    private PhotonRealtimePlayer player;
    private PlayerStats playerStats;

    public void Initialize(PhotonRealtimePlayer partyMember)
    {
        player = partyMember;
        UpdateUI();

        partyLeaderIcon.gameObject.SetActive(PartyManager.Instance.IsPartyLeader(player));
    }

    private void UpdateUI()
    {
        if (player == null) return;

        playerStats = PartyManager.Instance.GetPartyMemberStats(player);
        if (playerStats != null)
        {
            playerNameText.text = playerStats.nickName;
            levelText.text = playerStats.level.ToString();
            classText.text = PartyManager.Instance.GetPartyMemberClass(player);
        }
    }

    private void Update()
    {
        UpdateUI();
    }
}
