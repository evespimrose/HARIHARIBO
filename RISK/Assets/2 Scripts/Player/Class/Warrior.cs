using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Warrior : Player
{
    [Header("전사 스탯 설정")]
    [SerializeField] private float baseMaxHealth = 150f;
    [SerializeField] private int baseHealthPerLevel = 15;
    [SerializeField] private float baseAttackPower = 15f;
    [SerializeField] private int baseAttackPerLevel = 3;
    [SerializeField] private float baseMoveSpeed =1f;

    [Header("방어 & 회복")]
    [SerializeField, Range(0f, 1f)] private float baseDamageReduction = 0.2f;
    [SerializeField] private float baseHealthRegen = 2f;
    [SerializeField] private float baseRegenInterval = 1f;

    [Header("추가 스탯")]
    [SerializeField, Range(0f, 1f)] private float baseCriticalChance = 0.1f;
    [SerializeField] private float baseCriticalDamage = 1.5f;
    [SerializeField, Range(0f, 1f)] private float baseCooldownReduction = 0.1f;

    protected override void InitializeStats()
    {
        stats = new Playerstats();

        stats.maxHealth = baseMaxHealth;            // 체력
        stats.healthPerLevel = baseHealthPerLevel;         // 레벨당 체력 증가량
        stats.attackPower = baseAttackPower;           // 기본 공격력
        stats.attackPerLevel = baseAttackPerLevel;          // 레벨당 공격력 증가량
        stats.moveSpeed = baseMoveSpeed;              // 기본 이동속도
        stats.damageReduction = baseDamageReduction;      // 높은 피해감소 (20%)
        stats.healthRegen = baseHealthRegen;            // 체력 회복량
        stats.regenInterval = baseRegenInterval;          // 회복 주기

        
        stats.criticalChance = baseCriticalChance;       // 크리티컬 확률 10%
        stats.criticalDamage = baseCriticalDamage;       // 크리티컬 데미지 150%
        stats.cooldownReduction = baseCooldownReduction;    // 쿨타임 감소 10%

        stats.InitializeStats();     // 현재 체력을 최대체력으로 설정
    }

    protected override void InitializeStateHandler()
    {
        stateHandler = new StateHandler<Player>(this);

        stateHandler.RegisterState(new WarriorIdleState(stateHandler));
        stateHandler.RegisterState(new WarriorMoveState(stateHandler));
        stateHandler.RegisterState(new WarriorAttackState(stateHandler));

        stateHandler.RegisterState(new WarriorWSkillState(stateHandler));
        stateHandler.RegisterState(new WarriorESkillState(stateHandler));
        stateHandler.RegisterState(new WarriorRSkillState(stateHandler));

        stateHandler.ChangeState(typeof(WarriorIdleState));
    }

    protected override void HandleSkillInput()
    {
        stats.UpadateHealthRegen(Time.deltaTime);

        if (isSkillInProgress)
        {
            stateHandler.Update();
            return;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            stateHandler.ChangeState(typeof(WarriorAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorWSkillState));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorESkillState));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorRSkillState));
        }

        stateHandler.Update();
    }

    
}
