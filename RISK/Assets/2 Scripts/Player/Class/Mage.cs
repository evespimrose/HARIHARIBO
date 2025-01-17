using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Player
{
    [Header("筌띾뜄苡????쎄틛 ??쇱젟")]
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

    [Header("?⑤벀爰???꾨읃??")]
    [SerializeField] private GameObject ProjectilePrefab;

    [Header("?⑤벀爰??????")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform tSkillAttackPoint;

    [Header("?⑤벀爰???쇱젟")]
    [SerializeField] private float normalSpeed = 3f; // 疫꿸퀡????沅쀯㎗???얜즲
    [SerializeField] private float finalSpeed = 3f;  // 筌띾뜆?筌??꾠끇????沅쀯㎗???얜즲
    [SerializeField] private float finalScale = 2f;
    [SerializeField] private float LifeTime = 5f;

    [Header("기본 공격 데미지 계수")]
    [SerializeField] private float normalAttackDamagePercent = 100f;
    [SerializeField] private float finalAttackDamagePercent = 150f;

    [Header("W??쎄텢 ??쇱젟")]
    [SerializeField] private GameObject wSkillProjectilePrefab;
    [SerializeField] private float wSkillSpeed = 3f;
    [SerializeField] private float wSkillLifeTime = 3f;
    [SerializeField] private float wSkillScale = 2.5f;

    [Header("T ??쎄텢 ??쇱젟")]
    [SerializeField] private GameObject tSkillProjectilePrefab;
    [SerializeField] private float tSkillSpeed = 3f;
    [SerializeField] private float tSkillLifeTime = 3f;
    [SerializeField] private float tSkillRange = 15f;
    [SerializeField] private LayerMask monsterLayer;

    private Transform currentTSkillTarget;

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

        stateHandler.RegisterState(new MageIdleState(stateHandler));
        stateHandler.RegisterState(new MageMoveState(stateHandler));
        stateHandler.RegisterState(new MageAttackState(stateHandler));

        stateHandler.RegisterState(new MageWSkill(stateHandler));
        stateHandler.RegisterState(new MageESkill(stateHandler));
        stateHandler.RegisterState(new MageRSkill(stateHandler));
        stateHandler.RegisterState(new MageTSkill(stateHandler));

        stateHandler.ChangeState(typeof(MageIdleState));
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
            stateHandler.ChangeState(typeof(MageAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(MageWSkill));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(MageESkill));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(MageRSkill));
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(MageTSkill));
        }

        stateHandler.Update();
    }
    private Transform GetNearestMonster(Collider[] monsters)
    {
        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (Collider monster in monsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = monster.transform;
            }
        }

        return nearest;
    }

    public void ShootBall(int comboIndex)
    {
        if (ProjectilePrefab != null && attackPoint != null)
        {
            Vector3 shootDirection = transform.forward;
            GameObject Energyball = Instantiate(ProjectilePrefab,
                                     attackPoint.position,
                                     Quaternion.identity);

            Energyball.layer = LayerMask.NameToLayer("PlayerProjectile");

            var skillDamage = Energyball.GetComponent<SkillDamageInfo>();
            if (skillDamage != null)
            {
                skillDamage.skillName = "MageProjectile";
                if (comboIndex == 3)
                {
                    skillDamage.damagePercent = finalAttackDamagePercent;  // 마지막 콤보는 더 높은 데미지
                }
                else
                {
                    skillDamage.damagePercent = normalAttackDamagePercent;  // 기본 데미지
                }
            }

            var projectileMove = Energyball.GetComponent<ProjectileMove>();
            if (projectileMove != null)
            {
                projectileMove.Initialize(shootDirection, this); // ??猷?獄쎻뫚堉???쇱젟
                projectileMove.SetLifeTime(LifeTime);

                switch (comboIndex)
                {
                    case 1:
                    case 2:
                        projectileMove.speed = normalSpeed;
                        break;
                    case 3:
                        Energyball.transform.localScale *= finalScale;
                        projectileMove.speed = finalSpeed;
                        break;
                }
            }
        }
    }
    public void ShootWSkill()
    {
        if (wSkillProjectilePrefab != null && attackPoint != null)
        {
            Vector3 shootDirection = transform.forward;
            GameObject projectile = Instantiate(wSkillProjectilePrefab,
                                             attackPoint.position,
                                             Quaternion.identity);

            projectile.layer = LayerMask.NameToLayer("PlayerProjectile");

            var skillDamage = projectile.GetComponent<SkillDamageInfo>();
            if (skillDamage != null)
            {
                skillDamage.skillName = "MageWSkill";
            }

            var projectileMove = projectile.GetComponent<ProjectileMove>();
            if (projectileMove != null)
            {
                projectileMove.Initialize(shootDirection,this);
                projectileMove.speed = wSkillSpeed;
                projectileMove.SetLifeTime(wSkillLifeTime);
            }
        }
    }

    public void ShootTSkill()
    {
        if (currentTSkillTarget != null && tSkillProjectilePrefab != null && attackPoint != null)
        {
            Vector3 targetPoint = currentTSkillTarget.position;
            Vector3 shootDirection = (targetPoint - tSkillAttackPoint.position).normalized;

            GameObject projectile = Instantiate(tSkillProjectilePrefab,
                                             tSkillAttackPoint.position,
                                             Quaternion.identity);

            projectile.layer = LayerMask.NameToLayer("PlayerProjectile");

            var skillDamage = projectile.GetComponent<SkillDamageInfo>();
            if (skillDamage != null)
            {
                skillDamage.skillName = "MageTSkill";
            }

            var projectileMove = projectile.GetComponent<ProjectileMove>();
            if (projectileMove != null)
            {
                projectileMove.Initialize(shootDirection,this);
                projectileMove.speed = tSkillSpeed;
                projectileMove.SetLifeTime(tSkillLifeTime);
            }
        }
    }
    public void SetTSkillTarget()
    {
        Collider[] monsters = Physics.OverlapSphere(transform.position, tSkillRange, monsterLayer);

        Debug.DrawLine(transform.position, transform.position + Vector3.forward * tSkillRange, Color.yellow, 2f);
        Debug.DrawLine(transform.position, transform.position - Vector3.forward * tSkillRange, Color.yellow, 2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * tSkillRange, Color.yellow, 2f);
        Debug.DrawLine(transform.position, transform.position - Vector3.right * tSkillRange, Color.yellow, 2f);
        currentTSkillTarget = GetNearestMonster(monsters);
        if (currentTSkillTarget != null)
        {
            Debug.Log($"Found nearest target: {currentTSkillTarget.name}");
            Debug.DrawLine(transform.position, currentTSkillTarget.position, Color.green, 2f);
        }
        else
        {
            Debug.Log("No monsters found in range");
        }
    }
}
