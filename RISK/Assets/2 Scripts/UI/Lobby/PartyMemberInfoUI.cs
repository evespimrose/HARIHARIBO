using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using PhotonRealtimePlayer = Photon.Realtime.Player;
using HashTable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;


public class PartyMemberInfoUI : MonoBehaviourPunCallbacks
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private Image partyLeaderIcon;
    [SerializeField] private Image memberImage;
    [SerializeField] private Button fireButton;


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
        fireButton.onClick.RemoveAllListeners();
        fireButton.onClick.AddListener(FireButtonOnClick);
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
            //classText.text = PartyManager.Instance.GetPartyMemberClass(player);
            //partyLeaderIcon.gameObject.SetActive(PartyManager.Instance.IsPartyLeader(player));
        }

    }

    private void UpdateUI(FireBaseCharacterData fireBaseCharacterData)
    {
        playerNameText.text = fireBaseCharacterData.nickName;
        levelText.text = fireBaseCharacterData.level.ToString();
        classText.text = fireBaseCharacterData.classType.ToString();
        memberImage.sprite = SetClassImage(fireBaseCharacterData);
    }

    private Sprite SetClassImage(FireBaseCharacterData fireBaseCharacterData)
    {
        Sprite classImage;
        classImage = GameManager.Instance.characterDataDic[fireBaseCharacterData.classType].headSprite;
        return classImage;
    }

    private void FireButtonOnClick()
    {
        if (player != null && PhotonNetwork.IsMasterClient)
        {
            if (PartyManager.Instance.IsPartyLeader(PhotonNetwork.LocalPlayer))
            {
                if (player != PhotonNetwork.LocalPlayer)
                {
                    PhotonManager.Instance.photonView?.RPC("KickPlayerRPC", PhotonNetwork.CurrentRoom.GetPlayer(player.ActorNumber));
                }
                else
                {
                    PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Error", "You cannot kick yourself.");
                }
            }
            else
            {
                PanelManager.Instance.PopupOpen<PopupPanel>().SetPopup("Error", "Only party leader can kick members.");
            }
        }
    }

    public override void OnPlayerLeftRoom(PhotonRealtimePlayer otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        if(player == otherPlayer)
            DestroyImmediate(gameObject);
    }
}
