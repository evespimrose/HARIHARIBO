using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUIController : MonoBehaviourPun, IPunObservable
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image[] partyHPBars;
    [SerializeField] private Image playerHPBar;
    [SerializeField] private GameObject joystickUI;
    [SerializeField] private TMP_Text playerNameText;

    [Header("Combat UI")]
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button[] skillButtons;// W, E, R, T ?쒖꽌
    [SerializeField] private Image[] skillCooldowns;

    [Header("Skill Icons & Cooldowns")]
    // 媛?吏곸뾽蹂??ㅽ궗 ?꾩씠肄?(湲곕낯怨듦꺽, W, E, R, T ?쒖꽌濡?5媛?
    [SerializeField] private Sprite[] warriorIcons;
    [SerializeField] private Sprite[] mageIcons;
    [SerializeField] private Sprite[] healerIcons;
    [SerializeField] private Sprite[] destroyerIcons;

    // 媛?吏곸뾽蹂??ㅽ궗 荑⑦???(W, E, R, T ?쒖꽌濡?4媛?
    [SerializeField] private float[] warriorCooldowns;
    [SerializeField] private float[] mageCooldowns;
    [SerializeField] private float[] healerCooldowns;
    [SerializeField] private float[] destroyerCooldowns;

    [Header("PC UI Elements")]
    [SerializeField] private GameObject pcSkillPanel;
    [SerializeField] private Image[] pcSkillIcons;
    [SerializeField] private Image[] pcCooldownOverlays;
    [SerializeField] private TMP_Text[] pcKeyBindTexts;

    [Header("Party UI")]
    [SerializeField] private GameObject partyMemberPrefab;
    [SerializeField] private Transform partyContainer;
    private Dictionary<string, DungeonPartyMemberUI> partyMembers = new Dictionary<string, DungeonPartyMemberUI>();


    private bool[] isSkillInCooldown = new bool[4]; // W, E, R, T ?ㅽ궗??荑⑦????곹깭
    private float[] currentCooldowns;  // ?꾩옱 ?좏깮??吏곸뾽??荑⑦??????
    private Player localPlayer;

    [Serializable]
    private class PartyMemberData
    {
        public string playerName;
        public float healthRatio;
        public ClassType classType;
    }
    private Dictionary<string, PartyMemberData> syncedPartyData = new Dictionary<string, PartyMemberData>();


    private void Awake()
    {
#if UNITY_ANDROID
        joystickUI?.SetActive(true);
        combatPanel?.SetActive(true);
        pcSkillPanel?.SetActive(false);
        SetupSkillIconsByClass();
        SetupButtons();
#else
        joystickUI?.SetActive(false);
        combatPanel?.SetActive(false);
        pcSkillPanel?.SetActive(true);
#endif
    }
    private void Start()
    {
#if !UNITY_ANDROID
        SetupPCSkillUI();
#endif
    }

    private void Update()
    {
        if (localPlayer != null && localPlayer.Stats != null)
        {
            float healthRatio = localPlayer.Stats.currentHealth / localPlayer.Stats.maxHealth;
            UpdatePlayerInfo(healthRatio);
            if (string.IsNullOrEmpty(playerNameText.text))
                playerNameText.text = localPlayer.Stats.nickName;

            UpdatePartyMembersHP();
        }
    }
    public void SetPlayer(Player player)
    {
        localPlayer = player;
        if (player != null)
        {
            float healthRatio = player.Stats.currentHealth / player.Stats.maxHealth;
            UpdatePlayerInfo(healthRatio);
            InitializePartyUI();
        }
    }
    public void SetSkillPanelsActive(bool active)
    {
#if UNITY_ANDROID
        if (combatPanel != null)
            combatPanel.SetActive(active);
#else
        if (pcSkillPanel != null)
            pcSkillPanel.SetActive(active);
#endif
    }
    private void OnEnable()
    {
        if (PhotonNetwork.IsConnected)
        {
            StartCoroutine(WaitForUnitManagerAndInitialize());
        }
    }

    private IEnumerator WaitForUnitManagerAndInitialize()
    {
        yield return new WaitUntil(() => UnitManager.Instance != null && UnitManager.Instance.players.Count >= PhotonNetwork.CurrentRoom.PlayerCount);
        InitializePartyUI();
    }
    public void InitializePartyUI()
    {
        if (UnitManager.Instance == null) return;

        // 기존 파티원 UI 모두 제거
        foreach (var memberUI in partyMembers.Values)
        {
            Destroy(memberUI.gameObject);
        }
        partyMembers.Clear();

        // 새로운 파티원 UI 생성
        foreach (var playerObj in UnitManager.Instance.players.Values)
        {
            if (playerObj != null && playerObj != localPlayer.gameObject)
            {
                if (playerObj.TryGetComponent<Player>(out var player))
                {
                    CreatePartyMemberUI(player);
                }
            }
        }
    }
    private void CreatePartyMemberUI(Player player)
    {
        if (partyMemberPrefab == null || partyContainer == null)
        {
            Debug.LogError("Party member prefab or container is not assigned!");
            return;
        }

        // 이미 존재하는 UI면 스킵
        if (partyMembers.ContainsKey(player.name)) return;

        // UI 프리팹 생성
        GameObject uiObj = Instantiate(partyMemberPrefab, partyContainer);
        var memberUI = uiObj.GetComponent<DungeonPartyMemberUI>();
        memberUI.Initialize(player);  // ClassType 파라미터 제거
        partyMembers.Add(player.name, memberUI);
    }
    public void OnPlayerEnteredParty(Player newPlayer)
    {
        CreatePartyMemberUI(newPlayer);
    }

    public void RemovePartyMemberUI(string playerName)
    {
        if (partyMembers.TryGetValue(playerName, out var memberUI))
        {
            Destroy(memberUI.gameObject);
            partyMembers.Remove(playerName);
        }
    }

    private void UpdatePartyMembersHP()
    {
        foreach (var memberUI in partyMembers.Values)
        {
            memberUI.UpdateLocalHP();
        }
    }

   

    // PC 踰꾩쟾 UI ?ㅼ젙
    private void SetupPCSkillUI()
    {
        ClassType currentClass = FirebaseManager.Instance.currentCharacterData.classType;
        Sprite[] selectedIcons = null;
        float[] selectedCooldowns = null;

        // 吏곸뾽???곕Ⅸ ?꾩씠肄섍낵 荑⑦????좏깮 (湲곗〈 switch臾멸낵 ?숈씪)
        switch (currentClass)
        {
            case ClassType.Warrior:
                selectedIcons = warriorIcons;
                selectedCooldowns = warriorCooldowns;
                break;
            case ClassType.Mage:
                selectedIcons = mageIcons;
                selectedCooldowns = mageCooldowns;
                break;
            case ClassType.Healer:
                selectedIcons = healerIcons;
                selectedCooldowns = healerCooldowns;
                break;
            case ClassType.Destroyer:
                selectedIcons = destroyerIcons;
                selectedCooldowns = destroyerCooldowns;
                break;
        }

        if (selectedIcons != null && pcSkillIcons.Length >= 4)
        {
            for (int i = 0; i < pcSkillIcons.Length; i++)
            {
                if (pcSkillIcons[i] != null)
                {
                    // ?ㅽ궗 ?꾩씠肄??ㅼ젙
                    pcSkillIcons[i].sprite = selectedIcons[i + 1];

                    // 荑⑤떎???ㅻ쾭?덉씠 ?ㅼ젙
                    if (i < pcCooldownOverlays.Length && pcCooldownOverlays[i] != null)
                    {
                        pcCooldownOverlays[i].gameObject.SetActive(true);
                        pcCooldownOverlays[i].fillAmount = 0f;

                        // ?ㅻ쾭?덉씠 ?대?吏 ?됱긽 ?ㅼ젙
                        Color overlayColor = new Color(0, 0, 0, 0.7f);
                        pcCooldownOverlays[i].color = overlayColor;
                    }
                }
            }

        }
        if (pcKeyBindTexts.Length >= 4)
        {
            pcKeyBindTexts[0].text = "W";   // W ?ㅽ궗
            pcKeyBindTexts[1].text = "E";   // E ?ㅽ궗
            pcKeyBindTexts[2].text = "R";   // R ?ㅽ궗
            pcKeyBindTexts[3].text = "T";   // T ?ㅽ궗
        }

        currentCooldowns = selectedCooldowns;
    }

    // PC 踰꾩쟾 荑⑤떎???쒖옉 (Player ?대옒?ㅼ뿉???몄텧)
    public void StartPCCooldown(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < pcCooldownOverlays.Length)
        {
            isSkillInCooldown[skillIndex] = true;
            StartCoroutine(PCCooldownRoutine(skillIndex));
        }
    }

    public bool IsSkillInCooldown(int skillIndex)
    {
        if (skillIndex >= 0 && skillIndex < isSkillInCooldown.Length)
        {
            return isSkillInCooldown[skillIndex];
        }
        return false;
    }
    // PC 踰꾩쟾 荑⑤떎??泥섎━
    private IEnumerator PCCooldownRoutine(int skillIndex)
    {
        float cooldownTime = currentCooldowns[skillIndex];
        float cooldownReduction = FirebaseManager.Instance.currentCharacterData.coolRed;
        cooldownTime *= (1 - (cooldownReduction * 0.01f));

        float elapsed = 0;
        pcCooldownOverlays[skillIndex].gameObject.SetActive(true);
        pcCooldownOverlays[skillIndex].fillAmount = 1f;

        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            pcCooldownOverlays[skillIndex].fillAmount = 1 - (elapsed / cooldownTime);
            yield return null;
        }
        isSkillInCooldown[skillIndex] = false;
        pcCooldownOverlays[skillIndex].fillAmount = 0;
        pcCooldownOverlays[skillIndex].gameObject.SetActive(false);
    }

    // FirebaseManager??罹먮┃???곗씠?곕? 湲곕컲?쇰줈 ?ㅽ궗 ?꾩씠肄섍낵 荑⑦????ㅼ젙
    private void SetupSkillIconsByClass()
    {
        // ?꾩옱 ?좏깮??罹먮┃?곗쓽 吏곸뾽 ?뺤씤
        ClassType currentClass = FirebaseManager.Instance.currentCharacterData.classType;

        Sprite[] selectedIcons = null;
        float[] selectedCooldowns = null;

        // 吏곸뾽???곕씪 ?꾩씠肄섍낵 荑⑦????좏깮
        switch (currentClass)
        {
            case ClassType.Warrior:
                selectedIcons = warriorIcons;
                selectedCooldowns = warriorCooldowns;
                break;
            case ClassType.Mage:
                selectedIcons = mageIcons;
                selectedCooldowns = mageCooldowns;
                break;
            case ClassType.Healer:
                selectedIcons = healerIcons;
                selectedCooldowns = healerCooldowns;
                break;
            case ClassType.Destroyer:
                selectedIcons = destroyerIcons;
                selectedCooldowns = destroyerCooldowns;
                break;
        }

        // ?좏깮???꾩씠肄??ㅼ젙
        if (selectedIcons != null)
        {
            // ?ㅽ궗 ?꾩씠肄섎뱾 (1~4踰??몃뜳??
            for (int i = 0; i < skillButtons.Length; i++)
            {
                skillButtons[i].image.sprite = selectedIcons[i + 1];
            }
        }

        // ?꾩옱 吏곸뾽??荑⑦??????
        currentCooldowns = selectedCooldowns;
    }

    // 踰꾪듉 湲곕뒫 ?ㅼ젙
    private void SetupButtons()
    {
        // 湲곕낯 怨듦꺽 踰꾪듉
        basicAttackButton?.onClick.AddListener(() =>
        {
            if (localPlayer != null && localPlayer.photonView.IsMine)
            {
                if (localPlayer is Warrior)
                    localPlayer.MobileChangeState(typeof(WarriorAttackState));
                else if (localPlayer is Mage)
                    localPlayer.MobileChangeState(typeof(MageAttackState));
                else if (localPlayer is Healer)
                    localPlayer.MobileChangeState(typeof(HealerAttackState));
                else if (localPlayer is Destroyer)
                    localPlayer.MobileChangeState(typeof(DestroyerAttackState));
            }
        });

        // ?ㅽ궗 踰꾪듉???ㅼ젙
        if (skillButtons.Length >= 4)
        {
            SetupSkillButton(0, "W");
            SetupSkillButton(1, "E");
            SetupSkillButton(2, "R");
            SetupSkillButton(3, "T");
        }
    }

    private void SetupSkillButton(int index, string skillKey)
    {
        skillButtons[index].onClick.AddListener(() =>
        {
            if (localPlayer != null && localPlayer.photonView.IsMine)
            {
                localPlayer.SetSkillInProgress(true);

                // 硫붿씠吏 T?ㅽ궗 ?뱀닔 泥섎━
                if (skillKey == "T" && localPlayer is Mage)
                {
                    ((Mage)localPlayer).SetTSkillTarget();
                }

                string skillTypeName = $"{localPlayer.GetType().Name}{skillKey}Skill";
                Type skillType = Type.GetType(skillTypeName);
                if (skillType != null)
                {
                    localPlayer.MobileChangeState(skillType);
                    StartCooldown(index);
                }
            }
        });
    }

    // 荑⑤떎???쒖옉
    private void StartCooldown(int buttonIndex)
    {
        if (skillButtons[buttonIndex] != null)
        {
            skillButtons[buttonIndex].interactable = false;
            StartCoroutine(CooldownRoutine(buttonIndex));
        }
    }

    // 荑⑤떎??泥섎━
    private IEnumerator CooldownRoutine(int buttonIndex)
    {
        // ?꾩옱 吏곸뾽???ㅽ궗蹂?荑⑦????곸슜
        float cooldownTime = currentCooldowns[buttonIndex];
        // 荑⑦???媛먯냼 ?ㅽ꺈 ?곸슜
        float cooldownReduction = FirebaseManager.Instance.currentCharacterData.coolRed;
        cooldownTime *= (1 - (cooldownReduction * 0.01f)); // 1% ?⑥쐞濡?媛먯냼

        float elapsed = 0;

        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            if (skillCooldowns[buttonIndex] != null)
            {
                skillCooldowns[buttonIndex].fillAmount = 1 - (elapsed / cooldownTime);
            }
            yield return null;
        }

        if (skillButtons[buttonIndex] != null)
        {
            skillButtons[buttonIndex].interactable = true;
            skillCooldowns[buttonIndex].fillAmount = 0;
        }
    }

    // UI ?낅뜲?댄듃 硫붿꽌?쒕뱾
    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void UpdatePartyHP(int memberIndex, float hpRatio)
    {
        if (memberIndex < partyHPBars.Length)
        {
            partyHPBars[memberIndex].fillAmount = hpRatio;
        }
    }

    public void UpdatePlayerInfo(float hpRatio)
    {
        playerHPBar.fillAmount = hpRatio;
    }

    private string syncedTimerText;

    public void UpdateTimerText(string text)
    {
        if (timerText != null)
        {
            timerText.text = text;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 파티 멤버 데이터 전송
            foreach (var member in partyMembers)
            {
                if (member.Value.Player != null && member.Value.Player.Stats != null)
                {
                    float healthRatio = member.Value.Player.Stats.currentHealth / member.Value.Player.Stats.maxHealth;
                    stream.SendNext(member.Key); // 플레이어 이름
                    stream.SendNext(healthRatio); // HP 비율
                    stream.SendNext((int)member.Value.Player.ClassType); // 클래스 타입
                    stream.SendNext(timerText.text);
                }
            }
        }
        else
        {
            // 파티 멤버 데이터 수신
            syncedPartyData.Clear();
            while (stream.Count > 0 && stream.PeekNext() != null)
            {
                string playerName = (string)stream.ReceiveNext();
                float healthRatio = (float)stream.ReceiveNext();
                ClassType classType = (ClassType)stream.ReceiveNext();
                syncedTimerText = (string)stream.ReceiveNext();


                syncedPartyData[playerName] = new PartyMemberData
                {
                    playerName = playerName,
                    healthRatio = healthRatio,
                    classType = classType
                };
            }
            UpdatePartyMembersFromSyncedData();
            UpdateTimerText(syncedTimerText);
        }
    }
    private void UpdatePartyMembersFromSyncedData()
    {
        foreach (var syncedData in syncedPartyData)
        {
            if (partyMembers.TryGetValue(syncedData.Key, out var memberUI))
            {
                memberUI.UpdateHP(syncedData.Value.healthRatio);
                memberUI.UpdateClassIcon(syncedData.Value.classType);
            }
        }
    }
}
