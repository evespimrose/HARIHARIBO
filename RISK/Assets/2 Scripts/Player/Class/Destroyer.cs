using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : Player
{
    [Header("?遺용뮞?紐껋쨮??곷선 ??쎄틛 ??쇱젟")]
    [SerializeField] private float baseMaxHealth = 150f;
    [SerializeField] private int baseHealthPerLevel = 15;
    [SerializeField] private float baseAttackPower = 15f;
    [SerializeField] private int baseAttackPerLevel = 3;
    [SerializeField] private float baseMoveSpeed = 1f;

    [Header("獄쎻뫗堉?& ???궗")]
    [SerializeField, Range(0f, 1f)] private float baseDamageReduction = 0.2f;
    [SerializeField] private float baseHealthRegen = 2f;
    [SerializeField] private float baseRegenInterval = 1f;

    [Header("?곕떽? ??쎄틛")]
    [SerializeField, Range(0f, 1f)] private float baseCriticalChance = 0.1f;
    [SerializeField] private float baseCriticalDamage = 1.5f;
    [SerializeField, Range(0f, 1f)] private float baseCooldownReduction = 0.1f;

    [Header("??꾨읃??")]
    [SerializeField] private AnimationEventEffects effectsHandler;

    protected override void Awake()
    {
        base.Awake();  // ?봔筌??????쇱벥 ?λ뜃由???믪눘? ??쎈뻬

        // ??꾨읃???紐껊굶???λ뜃由??
        if (effectsHandler == null)
        {
            effectsHandler = GetComponent<AnimationEventEffects>();
            if (effectsHandler == null)
            {
                Debug.LogError("AnimationEventEffects component is missing on Warrior!");
            }
        }
    }

    protected override void InitializeStats()
    {
        stats = new PlayerStats();

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

        stateHandler.RegisterState(new DestroyerIdleState(stateHandler));
        stateHandler.RegisterState(new DestroyerMoveState(stateHandler));
        stateHandler.RegisterState(new DestroyerAttackState(stateHandler));

        stateHandler.RegisterState(new DestroyerWSkill(stateHandler));
        stateHandler.RegisterState(new DestroyerESkill(stateHandler));
        stateHandler.RegisterState(new DestroyerRSkill(stateHandler));
        stateHandler.RegisterState(new DestroyerTSkill(stateHandler));

        stateHandler.ChangeState(typeof(DestroyerIdleState));
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
            stateHandler.ChangeState(typeof(DestroyerAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(DestroyerWSkill));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(DestroyerESkill));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(DestroyerRSkill));
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(DestroyerTSkill));
        }

        stateHandler.Update();
    }
}
