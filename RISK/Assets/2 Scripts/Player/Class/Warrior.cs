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

        stats.maxHealth = baseMaxHealth;            
        stats.healthPerLevel = baseHealthPerLevel;  
        stats.attackPower = baseAttackPower;        
        stats.attackPerLevel = baseAttackPerLevel;  
        stats.moveSpeed = baseMoveSpeed;            
        stats.damageReduction = baseDamageReduction;
        stats.healthRegen = baseHealthRegen;        
        stats.regenInterval = baseRegenInterval;    

        
        stats.criticalChance = baseCriticalChance;      
        stats.criticalDamage = baseCriticalDamage;      
        stats.cooldownReduction = baseCooldownReduction;

        stats.InitializeStats();     
    }

    protected override void InitializeStateHandler()
    {
        stateHandler = new StateHandler<Player>(this);

        stateHandler.RegisterState(new WarriorIdleState(stateHandler));
        stateHandler.RegisterState(new WarriorMoveState(stateHandler));
        stateHandler.RegisterState(new WarriorAttackState(stateHandler));

        stateHandler.RegisterState(new WarriorWSkill(stateHandler));
        stateHandler.RegisterState(new WarriorESkill(stateHandler));
        stateHandler.RegisterState(new WarriorRSkill(stateHandler));

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
            stateHandler.ChangeState(typeof(WarriorWSkill));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorESkill));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorRSkill));
        }

        stateHandler.Update();
    }

    
}
