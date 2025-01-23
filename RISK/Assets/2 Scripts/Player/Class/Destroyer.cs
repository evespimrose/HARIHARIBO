using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : Player
{
    [Header("???됰Ŋ?좂뜏?癲ル슢??遺룔뀋????ㅿ폎?????熬곥굥六????繹먮냱??")]
    [SerializeField] private float baseMaxHealth;
    [SerializeField] private int baseHealthPerLevel;
    [SerializeField] private float baseAttackPower;
    [SerializeField] private int baseAttackPerLevel;
    [SerializeField] private float baseMoveSpeed;

    [Header("?熬곣뫖?삥납??關??& ???雅?")]
    [SerializeField, Range(0f, 1f)] private float baseDamageReduction;
    [SerializeField] private float baseHealthRegen;
    [SerializeField] private float baseRegenInterval;

    [Header("???ㅻ쿋?? ???熬곥굥六?")]
    [SerializeField, Range(0f, 1f)] private float baseCriticalChance;
    [SerializeField] private float baseCriticalDamage;
    [SerializeField, Range(0f, 1f)] private float baseCooldownReduction;

    [Header("???ш끽維???")]
    [SerializeField] private AnimationEventEffects effectsHandler;

    protected override void Awake()
    {
        base.Awake();  // ???낇뀘??????????繹먮굛???潁??용끏????亦껋꼨援?? ?????덊떀

        // ???ш끽維????癲ル슢??蹂좊쨨???潁??용끏???
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
                Debug.Log("Starting W skill cooldown"); // ?붾쾭洹?濡쒓렇 異붽?
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

        stateHandler.Update();
    }
}
