using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Player
{
    [Header("힐러 스탯 설정")]
    [SerializeField] private float baseMaxHealth = 150f;
    [SerializeField] private int baseHealthPerLevel = 15;
    [SerializeField] private float baseAttackPower = 15f;
    [SerializeField] private int baseAttackPerLevel = 3;
    [SerializeField] private float baseMoveSpeed = 1f;

    [Header("방어 & 회복")]
    [SerializeField, Range(0f, 1f)] private float baseDamageReduction = 0.2f;
    [SerializeField] private float baseHealthRegen = 2f;
    [SerializeField] private float baseRegenInterval = 1f;

    [Header("추가 스탯")]
    [SerializeField, Range(0f, 1f)] private float baseCriticalChance = 0.1f;
    [SerializeField] private float baseCriticalDamage = 1.5f;
    [SerializeField, Range(0f, 1f)] private float baseCooldownReduction = 0.1f;

    [Header("이펙트")]
    [SerializeField] private AnimationEventEffects effectsHandler;

    [Header("공격 이펙트")]
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private Transform attackPoint;

    [Header("공격 설정")]
    [SerializeField] private float normalSpeed = 3f; // 기본 투사체 속도
    [SerializeField] private float finalSpeed = 3f;  // 마지막 콤보 투사체 속도
    [SerializeField] private float finalScale = 2f;   // 마지막 콤보 투사체 크기
    [SerializeField] private float LifeTime = 5f;

    [Header("기본 공격 데미지 계수")]
    [SerializeField] private float normalAttackDamagePercent = 100f;
    [SerializeField] private float finalAttackDamagePercent = 150f;

    protected override void Awake()
    {
        base.Awake();

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
                skillDamage.skillName = "HealerProjectile";
                if (comboIndex == 3)
                {
                    skillDamage.damagePercent = finalAttackDamagePercent;  // 마지막 콤보는 더 높은 데미지
                }
                else
                {
                    skillDamage.damagePercent = normalAttackDamagePercent;  // 기본 데미지
                }
            }

            var projectileMove = card.GetComponent<ProjectileMove>();
            if (projectileMove != null)
            {
                projectileMove.Initialize(shootDirection, this); // 이동 방향 설정
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
}
