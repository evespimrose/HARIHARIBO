using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : Player
{
    [Header("????거?醫귣쐪??꿔꺂????븍짅??????욱룑??????ш낄援ο쭛????濚밸Ŧ???")]
    [SerializeField] public float baseMaxHealth;
    [SerializeField] private int baseHealthPerLevel;
    [SerializeField] public float baseAttackPower;
    [SerializeField] private int baseAttackPerLevel;
    [SerializeField] public float baseMoveSpeed;

    [Header("??ш끽維??λ궔?????& ?????")]
    [SerializeField, Range(0f, 1f)] public float baseDamageReduction;
    [SerializeField] public float baseHealthRegen;
    [SerializeField] public float baseRegenInterval;

    [Header("????살퓢?? ????ш낄援ο쭛?")]
    [SerializeField, Range(0f, 1f)] public float baseCriticalChance;
    [SerializeField] public float baseCriticalDamage;
    [SerializeField, Range(0f, 1f)] public float baseCooldownReduction;

    [Header("?????썹땟???")]
    [SerializeField] private AnimationEventEffects effectsHandler;

    protected override void Awake()
    {
        base.Awake();  // ????뉖??????????濚밸Ŧ援???逆???⑸걦????雅?퍔瑗ⓩ뤃?? ??????딅?

        // ?????썹땟?????꿔꺂???癰귥쥓夷???逆???⑸걦???
        if (effectsHandler == null)
        {
            effectsHandler = GetComponent<AnimationEventEffects>();
            if (effectsHandler == null)
            {
                Debug.LogError("AnimationEventEffects component is missing on Warrior!");
            }
        }
    }
    protected override void InitializeClassType()
    {
        ClassType = ClassType.Destroyer;
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
        var dungeonUI = FindObjectOfType<DungeonUIController>();
        Debug.Log($"DungeonUI found: {dungeonUI != null}");
        if (Input.GetKeyDown(KeyCode.A))
        {
            stateHandler.ChangeState(typeof(DestroyerAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W skill triggered");
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(DestroyerWSkill));
            if (dungeonUI != null)
            {
                Debug.Log("Starting W skill cooldown"); // ?遺얠쒔域?嚥≪뮄???곕떽?
                dungeonUI.StartPCCooldown(0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(DestroyerESkill));
            dungeonUI?.StartPCCooldown(1);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(DestroyerRSkill));
            dungeonUI?.StartPCCooldown(2);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(DestroyerTSkill));
            dungeonUI?.StartPCCooldown(3);
        }

        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.I))
        {
            UnitManager.Instance.DoomToMonsters();
        }

        stateHandler.Update();
    }
}
