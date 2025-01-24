using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Player
{
    [Header("?먮윭 ?ㅽ꺈 ?ㅼ젙")]
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

    [Header("怨듦꺽 ?댄럺??")]
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private Transform attackPoint;

    [Header("怨듦꺽 ?ㅼ젙")]
    [SerializeField] private float normalSpeed = 3f; // 湲곕낯 ?ъ궗泥??띾룄
    [SerializeField] private float finalSpeed = 3f;  // 留덉?留?肄ㅻ낫 ?ъ궗泥??띾룄
    [SerializeField] private float finalScale = 2f;   // 留덉?留?肄ㅻ낫 ?ъ궗泥??ш린
    [SerializeField] private float LifeTime = 5f;

    [Header("湲곕낯 怨듦꺽 ?곕?吏 怨꾩닔")]
    [SerializeField] private float normalAttackDamagePercent = 100f;
    [SerializeField] private float finalAttackDamagePercent = 150f;

    protected override void Awake()
    {   
        base.Awake();
        InitializeStats();

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
        Stats = stats;

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
        var dungeonUI = FindObjectOfType<DungeonUIController>();
        if (Input.GetKeyDown(KeyCode.A))
        {
            stateHandler.ChangeState(typeof(HealerAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(HealerWSkill));
            dungeonUI?.StartPCCooldown(0);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(HealerESkill));
            dungeonUI?.StartPCCooldown(1);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(HealerRSkill));
            dungeonUI?.StartPCCooldown(2);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(HealerTSkill));
            dungeonUI?.StartPCCooldown(3);
        }

        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.I))
        {
            UnitManager.Instance.DoomToMonsters();
        }

        stateHandler.Update();
    }
    public void ShootBall(int comboIndex)
    {
        if (ProjectilePrefab != null && attackPoint != null)
        {
            Vector3 shootDirection = transform.forward;
            GameObject card = Instantiate(ProjectilePrefab,
                                     attackPoint.position,
                                     Quaternion.identity);

            var skillDamage = card.GetComponent<SkillDamageInfo>();
            if (skillDamage != null)
            {
                skillDamage.SetOwnerPlayer(this);
                skillDamage.skillName = "HealerProjectile";
                if (comboIndex == 3)
                {
                    skillDamage.damagePercent = finalAttackDamagePercent;  // 留덉?留?肄ㅻ낫?????믪? ?곕?吏
                }
                else
                {
                    skillDamage.damagePercent = normalAttackDamagePercent;  // 湲곕낯 ?곕?吏
                }
            }

            var projectileMove = card.GetComponent<ProjectileMove>();
            if (projectileMove != null)
            {
                projectileMove.Initialize(shootDirection, this); // ?대룞 諛⑺뼢 ?ㅼ젙
                projectileMove.SetLifeTime(LifeTime);

                switch (comboIndex)
                {
                    case 1:
                    case 2:
                        projectileMove.speed = normalSpeed;
                        break;
                    case 3:
                        card.transform.localScale *= finalScale;
                        projectileMove.speed = finalSpeed;
                        break;
                }
            }
        }
    }
    public void EnsureStatsInitialized()
    {
        if (Stats == null)
        {
            Debug.Log($"[{gameObject.name}] Stats 초기화 시도");
            InitializeStats();
        }
    }
}
