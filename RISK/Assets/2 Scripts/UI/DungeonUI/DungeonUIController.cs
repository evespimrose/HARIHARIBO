using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private Image bossHPBar;
    [SerializeField] private Image[] partyHPBars;
    [SerializeField] private Image playerHPBar;

    [Header("Combat UI")]
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button[] skillButtons;// W, E, R, T 순서
    [SerializeField] private Image[] skillCooldowns;

    [Header("Skill Icons & Cooldowns")]
    // 각 직업별 스킬 아이콘 (기본공격, W, E, R, T 순서로 5개)
    [SerializeField] private Sprite[] warriorIcons;    // Size: 5
    [SerializeField] private Sprite[] mageIcons;       // Size: 5
    [SerializeField] private Sprite[] healerIcons;     // Size: 5
    [SerializeField] private Sprite[] destroyerIcons;  // Size: 5

    // 각 직업별 스킬 쿨타임 (W, E, R, T 순서로 4개)
    [SerializeField] private float[] warriorCooldowns;   // Size: 4
    [SerializeField] private float[] mageCooldowns;     // Size: 4
    [SerializeField] private float[] healerCooldowns;   // Size: 4
    [SerializeField] private float[] destroyerCooldowns; // Size: 4

    private float[] currentCooldowns;  // 현재 선택된 직업의 쿨타임 저장
    private Player localPlayer; // 로컬 플레이어 참조 추가

    private void Awake()
    {
#if UNITY_ANDROID       
        combatPanel?.SetActive(true);
        SetupSkillIconsByClass();  
        SetupButtons();            
#else
        combatPanel?.SetActive(false);
#endif
    }

    // 추가된 메서드: 플레이어 참조 설정
    public void SetPlayer(Player player)
    {
        localPlayer = player;
    }

    // FirebaseManager의 캐릭터 데이터를 기반으로 스킬 아이콘과 쿨타임 설정
    private void SetupSkillIconsByClass()
    {
        // 현재 선택된 캐릭터의 직업 확인
        ClassType currentClass = FirebaseManager.Instance.currentCharacterData.classType;

        Sprite[] selectedIcons = null;
        float[] selectedCooldowns = null;

        // 직업에 따라 아이콘과 쿨타임 선택
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

        // 선택된 아이콘 설정
        if (selectedIcons != null)
        {
            // 기본 공격 아이콘 (0번 인덱스)
            basicAttackButton.image.sprite = selectedIcons[0];

            // 스킬 아이콘들 (1~4번 인덱스)
            for (int i = 0; i < skillButtons.Length; i++)
            {
                skillButtons[i].image.sprite = selectedIcons[i + 1];
            }
        }

        // 현재 직업의 쿨타임 저장
        currentCooldowns = selectedCooldowns;
    }

    // 버튼 기능 설정
    private void SetupButtons()
    {
        // 기본 공격 버튼
        basicAttackButton?.onClick.AddListener(() => {
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

        // 스킬 버튼들 (W, E, R, T)
        if (skillButtons.Length >= 4)
        {
            // W 스킬
            skillButtons[0].onClick.AddListener(() => {
                if (localPlayer != null && localPlayer.photonView.IsMine)
                {
                    localPlayer.SetSkillInProgress(true);
                    if (localPlayer is Warrior)
                        localPlayer.MobileChangeState(typeof(WarriorWSkill));
                    else if (localPlayer is Mage)
                        localPlayer.MobileChangeState(typeof(MageWSkill));
                    else if (localPlayer is Healer)
                        localPlayer.MobileChangeState(typeof(HealerWSkill));
                    else if (localPlayer is Destroyer)
                        localPlayer.MobileChangeState(typeof(DestroyerWSkill));
                    StartCooldown(0);
                }
            });

            // E 스킬
            skillButtons[1].onClick.AddListener(() => {
                if (localPlayer != null && localPlayer.photonView.IsMine)
                {
                    localPlayer.SetSkillInProgress(true);
                    if (localPlayer is Warrior)
                        localPlayer.MobileChangeState(typeof(WarriorESkill));
                    else if (localPlayer is Mage)
                        localPlayer.MobileChangeState(typeof(MageESkill));
                    else if (localPlayer is Healer)
                        localPlayer.MobileChangeState(typeof(HealerESkill));
                    else if (localPlayer is Destroyer)
                        localPlayer.MobileChangeState(typeof(DestroyerESkill));
                    StartCooldown(1);
                }
            });

            // R 스킬
            skillButtons[2].onClick.AddListener(() => {
                if (localPlayer != null && localPlayer.photonView.IsMine)
                {
                    localPlayer.SetSkillInProgress(true);
                    if (localPlayer is Warrior)
                        localPlayer.MobileChangeState(typeof(WarriorRSkill));
                    else if (localPlayer is Mage)
                        localPlayer.MobileChangeState(typeof(MageRSkill));
                    else if (localPlayer is Healer)
                        localPlayer.MobileChangeState(typeof(HealerRSkill));
                    else if (localPlayer is Destroyer)
                        localPlayer.MobileChangeState(typeof(DestroyerRSkill));
                    StartCooldown(2);
                }
            });

            // T 스킬
            skillButtons[3].onClick.AddListener(() => {
                if (localPlayer != null && localPlayer.photonView.IsMine)
                {
                    localPlayer.SetSkillInProgress(true);
                    // 메이지의 경우 특수 처리
                    if (localPlayer is Mage)
                        ((Mage)localPlayer).SetTSkillTarget();

                    if (localPlayer is Warrior)
                        localPlayer.MobileChangeState(typeof(WarriorTSkill));
                    else if (localPlayer is Mage)
                        localPlayer.MobileChangeState(typeof(MageTSkill));
                    else if (localPlayer is Healer)
                        localPlayer.MobileChangeState(typeof(HealerTSkill));
                    else if (localPlayer is Destroyer)
                        localPlayer.MobileChangeState(typeof(DestroyerTSkill));
                    StartCooldown(3);
                }
            });
        }
    }

    // 쿨다운 시작
    private void StartCooldown(int buttonIndex)
    {
        if (skillButtons[buttonIndex] != null)
        {
            skillButtons[buttonIndex].interactable = false;
            StartCoroutine(CooldownRoutine(buttonIndex));
        }
    }

    // 쿨다운 처리
    private IEnumerator CooldownRoutine(int buttonIndex)
    {
        // 현재 직업의 스킬별 쿨타임 적용
        float cooldownTime = currentCooldowns[buttonIndex];
        // 쿨타임 감소 스탯 적용
        float cooldownReduction = FirebaseManager.Instance.currentCharacterData.coolRed;
        cooldownTime *= (1 - (cooldownReduction * 0.01f)); // 1% 단위로 감소

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

    // UI 업데이트 메서드들
    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void UpdateBossInfo(string bossName, float hpRatio)
    {
        bossNameText.text = bossName;
        bossHPBar.fillAmount = hpRatio;
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
}
