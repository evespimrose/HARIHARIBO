using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Player
{
    [Header("?꾩궗 ?ㅽ꺈 ?ㅼ젙")]
    [SerializeField] public float baseMaxHealth;
    [SerializeField] private int baseHealthPerLevel;
    [SerializeField] public float baseAttackPower;
    [SerializeField] private int baseAttackPerLevel;
    [SerializeField] public float baseMoveSpeed;

    [Header("諛⑹뼱 & ?뚮났")]
    [SerializeField, Range(0f, 1f)] public float baseDamageReduction;
    [SerializeField] public float baseHealthRegen;
    [SerializeField] public float baseRegenInterval;

    [Header("異붽? ?ㅽ꺈")]
    [SerializeField, Range(0f, 1f)] public float baseCriticalChance;
    [SerializeField] public float baseCriticalDamage;
    [SerializeField, Range(0f, 1f)] public float baseCooldownReduction;

    [Header("?댄럺??")]
    [SerializeField] private AnimationEventEffects effectsHandler;

    protected override void Awake()
    {
        base.Awake();  // 遺紐??대옒?ㅼ쓽 珥덇린??癒쇱? ?ㅽ뻾

        // ?댄럺???몃뱾??珥덇린??
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

        stateHandler.RegisterState(new WarriorIdleState(stateHandler));
        stateHandler.RegisterState(new WarriorMoveState(stateHandler));
        stateHandler.RegisterState(new WarriorAttackState(stateHandler));

        stateHandler.RegisterState(new WarriorWSkill(stateHandler));
        stateHandler.RegisterState(new WarriorESkill(stateHandler));
        stateHandler.RegisterState(new WarriorRSkill(stateHandler));
        stateHandler.RegisterState(new WarriorTSkill(stateHandler));

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

        var dungeonUI = FindObjectOfType<DungeonUIController>();
        if (Input.GetKeyDown(KeyCode.A))
        {
            stateHandler.ChangeState(typeof(WarriorAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorWSkill));
            dungeonUI?.StartPCCooldown(0);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorESkill));
            dungeonUI?.StartPCCooldown(1);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorRSkill));
            dungeonUI?.StartPCCooldown(2);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorTSkill));
            dungeonUI?.StartPCCooldown(3);
        }

        stateHandler.Update();
    }


}
