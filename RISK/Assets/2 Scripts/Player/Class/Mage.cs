using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Player
{
    [Header("嶺뚮씭?꾥떋?????꾪떅 ???깆젧")]
    [SerializeField] public float baseMaxHealth;
    [SerializeField] private int baseHealthPerLevel;
    [SerializeField] public float baseAttackPower;
    [SerializeField] private int baseAttackPerLevel;
    [SerializeField] public float baseMoveSpeed;

    [Header("?꾩렮維쀥젆?& ???沅?")]
    [SerializeField, Range(0f, 1f)] public float baseDamageReduction;
    [SerializeField] public float baseHealthRegen;
    [SerializeField] public float baseRegenInterval;

    [Header("?怨뺣뼺? ???꾪떅")]
    [SerializeField, Range(0f, 1f)] public float baseCriticalChance;
    [SerializeField] public float baseCriticalDamage;
    [SerializeField, Range(0f, 1f)] public float baseCooldownReduction;

    [Header("??袁⑥쓢??")]
    [SerializeField] private AnimationEventEffects effectsHandler;

    [Header("??ㅻ?????袁⑥쓢??")]
    [SerializeField] private GameObject ProjectilePrefab;

    [Header("??ㅻ????????")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform tSkillAttackPoint;

    [Header("??ㅻ??????깆젧")]
    [SerializeField] private float normalSpeed = 3f; // ?リ옇?????亦낆?럸????쒖┣
    [SerializeField] private float finalSpeed = 3f;  // 嶺뚮씭??嶺??袁좊걞????亦낆?럸????쒖┣
    [SerializeField] private float finalScale = 2f;
    [SerializeField] private float LifeTime = 5f;

    [Header("湲곕낯 怨듦꺽 ?곕?吏 怨꾩닔")]
    [SerializeField] private float normalAttackDamagePercent = 100f;
    [SerializeField] private float finalAttackDamagePercent = 150f;

    [Header("W???꾪뀬 ???깆젧")]
    [SerializeField] private GameObject wSkillProjectilePrefab;
    [SerializeField] private float wSkillSpeed = 3f;
    [SerializeField] private float wSkillLifeTime = 3f;
    [SerializeField] private float wSkillScale = 2.5f;

    [Header("T ???꾪뀬 ???깆젧")]
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
    protected override void InitializeClassType()
    {
        ClassType = ClassType.Mage;
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
        var dungeonUI = FindObjectOfType<DungeonUIController>();
        if (Input.GetKeyDown(KeyCode.A))
        {
            stateHandler.ChangeState(typeof(MageAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(MageWSkill));
            dungeonUI?.StartPCCooldown(0);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(MageESkill));
            dungeonUI?.StartPCCooldown(1);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(MageRSkill));
            dungeonUI?.StartPCCooldown(2);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(MageTSkill));
            dungeonUI?.StartPCCooldown(3);
        }

        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.I))
        {
            UnitManager.Instance.DoomToMonsters();
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
                    skillDamage.damagePercent = finalAttackDamagePercent;  // 留덉?留?肄ㅻ낫?????믪? ?곕?吏
                }
                else
                {
                    skillDamage.damagePercent = normalAttackDamagePercent;  // 湲곕낯 ?곕?吏
                }
            }

            var projectileMove = Energyball.GetComponent<ProjectileMove>();
            if (projectileMove != null)
            {
                projectileMove.Initialize(shootDirection, this); // ??????꾩렮維싧젆????깆젧
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
                projectileMove.Initialize(shootDirection, this);
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
                projectileMove.Initialize(shootDirection, this);
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
