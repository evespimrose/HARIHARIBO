using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using PhotonRealtimePlayer = Photon.Realtime.Player;
using HashTable = ExitGames.Client.Photon.Hashtable;


public class PartyMemberInfoUI : MonoBehaviourPunCallbacks
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private Image partyLeaderIcon;
    [SerializeField] private Image memberImage;

    private PhotonRealtimePlayer player;
    private PlayerStats playerStats;

    public void Initialize(PhotonRealtimePlayer partyMember)
    {
        player = partyMember;
        UpdateUI();
    }

    public void Initialize(FireBaseCharacterData partyMember)
    {
        UpdateUI(partyMember);
    }

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (player == null) return;

        if (playerStats != null)
        {
            playerNameText.text = playerStats.nickName;
            levelText.text = playerStats.level.ToString();
            classText.text = PartyManager.Instance.GetPartyMemberClass(player);
            partyLeaderIcon.gameObject.SetActive(PartyManager.Instance.IsPartyLeader(player));
        }

    }

    private void UpdateUI(FireBaseCharacterData fireBaseCharacterData)
    {
        playerNameText.text = fireBaseCharacterData.nickName;
        levelText.text = fireBaseCharacterData.level.ToString();
        classText.text = fireBaseCharacterData.classType.ToString();
    }
}
