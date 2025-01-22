using System;
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
    [SerializeField] private GameObject joystickUI;

    [Header("Combat UI")]
    [SerializeField] private GameObject combatPanel;
    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button[] skillButtons;// W, E, R, T 순서
    [SerializeField] private Image[] skillCooldowns;

    [Header("Skill Icons & Cooldowns")]
    // 각 직업별 스킬 아이콘 (기본공격, W, E, R, T 순서로 5개)
    [SerializeField] private Sprite[] warriorIcons;    
    [SerializeField] private Sprite[] mageIcons;       
    [SerializeField] private Sprite[] healerIcons;     
    [SerializeField] private Sprite[] destroyerIcons;  

    // 각 직업별 스킬 쿨타임 (W, E, R, T 순서로 4개)
    [SerializeField] private float[] warriorCooldowns;   
    [SerializeField] private float[] mageCooldowns;     
    [SerializeField] private float[] healerCooldowns;   
    [SerializeField] private float[] destroyerCooldowns;

    [Header("PC UI Elements")]
    [SerializeField] private GameObject pcSkillPanel;     
    [SerializeField] private Image[] pcSkillIcons;        
    [SerializeField] private Image[] pcCooldownOverlays;  
    [SerializeField] private TMP_Text[] pcKeyBindTexts;


    private bool[] isSkillInCooldown = new bool[4]; // W, E, R, T 스킬의 쿨타임 상태
    private float[] currentCooldowns;  // 현재 선택된 직업의 쿨타임 저장
    private Player localPlayer; 

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
        // Firebase 초기화 이후에 실행
        SetupPCSkillUI();
#endif
    }

    // 추가된 메서드: 플레이어 참조 설정
    public void SetPlayer(Player player)
    {
        localPlayer = player;
    }

    // PC 버전 UI 설정
    private void SetupPCSkillUI()
    {
        ClassType currentClass = FirebaseManager.Instance.currentCharacterData.classType;
        Sprite[] selectedIcons = null;
        float[] selectedCooldowns = null;

        // 직업에 따른 아이콘과 쿨타임 선택 (기존 switch문과 동일)
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
                    // 스킬 아이콘 설정
                    pcSkillIcons[i].sprite = selectedIcons[i + 1];

                    // 쿨다운 오버레이 설정
                    if (i < pcCooldownOverlays.Length && pcCooldownOverlays[i] != null)
                    {
                        pcCooldownOverlays[i].gameObject.SetActive(true);
                        pcCooldownOverlays[i].fillAmount = 0f;

                        // 오버레이 이미지 색상 설정
                        Color overlayColor = new Color(0, 0, 0, 0.7f);
                        pcCooldownOverlays[i].color = overlayColor;
                    }
                }
            }
        
        }
        if (pcKeyBindTexts.Length >= 4)
        {
            pcKeyBindTexts[0].text = "W";   // W 스킬
            pcKeyBindTexts[1].text = "E";   // E 스킬
            pcKeyBindTexts[2].text = "R";   // R 스킬
            pcKeyBindTexts[3].text = "T";   // T 스킬
        }

        currentCooldowns = selectedCooldowns;
    }

    // PC 버전 쿨다운 시작 (Player 클래스에서 호출)
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
    // PC 버전 쿨다운 처리
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

        // 스킬 버튼들 설정
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
        skillButtons[index].onClick.AddListener(() => {
            if (localPlayer != null && localPlayer.photonView.IsMine)
            {
                localPlayer.SetSkillInProgress(true);

                // 메이지 T스킬 특수 처리
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
