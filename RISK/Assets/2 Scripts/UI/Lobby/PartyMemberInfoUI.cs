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
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
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
            levelText.text = $"Lv.{playerStats.level}";
            classText.text = PartyManager.Instance.GetPartyMemberClass(player);
            
            healthSlider.maxValue = playerStats.maxHealth;
            healthSlider.value = playerStats.currentHealth;
            healthText.text = $"{Mathf.Round(playerStats.currentHealth)}/{playerStats.maxHealth}";
        }
    }

    private void Update()
    {
        UpdateUI();
    }
} 
