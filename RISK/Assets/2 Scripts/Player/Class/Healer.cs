using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Player
{
    [Header("Èú·¯ ½ºÅÈ ¼³Á¤")]
    [SerializeField] private float baseMaxHealth = 150f;
    [SerializeField] private int baseHealthPerLevel = 15;
    [SerializeField] private float baseAttackPower = 15f;
    [SerializeField] private int baseAttackPerLevel = 3;
    [SerializeField] private float baseMoveSpeed = 1f;

    [Header("¹æ¾î & È¸º¹")]
    [SerializeField, Range(0f, 1f)] private float baseDamageReduction = 0.2f;
    [SerializeField] private float baseHealthRegen = 2f;
    [SerializeField] private float baseRegenInterval = 1f;

    [Header("Ãß°¡ ½ºÅÈ")]
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

        stateHandler.RegisterState(new HealerIdleState(stateHandler));
        stateHandler.RegisterState(new HealerMoveState(stateHandler));
        stateHandler.RegisterState(new HealerAttackState(stateHandler));

        stateHandler.RegisterState(new HealerWSkill(stateHandler));
        stateHandler.RegisterState(new HealerESkill(stateHandler));
        stateHandler.RegisterState(new HealerRSkill(stateHandler));
        stateHandler.RegisterState(new HealerTSkill(stateHandler));

        stateHandler.ChangeState(typeof(HealerIdleState));
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
            stateHandler.ChangeState(typeof(HealerAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(HealerWSkill));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(HealerESkill));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(HealerRSkill));
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(HealerTSkill));
        }

        stateHandler.Update();
    }
}
