using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Warrior : Player
{
    [Header("���� ���� ����")]
    [SerializeField] private float baseMaxHealth = 150f;
    [SerializeField] private int baseHealthPerLevel = 15;
    [SerializeField] private float baseAttackPower = 15f;
    [SerializeField] private int baseAttackPerLevel = 3;
    [SerializeField] private float baseMoveSpeed =1f;

    [Header("��� & ȸ��")]
    [SerializeField, Range(0f, 1f)] private float baseDamageReduction = 0.2f;
    [SerializeField] private float baseHealthRegen = 2f;
    [SerializeField] private float baseRegenInterval = 1f;

    [Header("�߰� ����")]
    [SerializeField, Range(0f, 1f)] private float baseCriticalChance = 0.1f;
    [SerializeField] private float baseCriticalDamage = 1.5f;
    [SerializeField, Range(0f, 1f)] private float baseCooldownReduction = 0.1f;

    protected override void InitializeStats()
    {
        stats = new Playerstats();

        stats.maxHealth = baseMaxHealth;            // ü��
        stats.healthPerLevel = baseHealthPerLevel;         // ������ ü�� ������
        stats.attackPower = baseAttackPower;           // �⺻ ���ݷ�
        stats.attackPerLevel = baseAttackPerLevel;          // ������ ���ݷ� ������
        stats.moveSpeed = baseMoveSpeed;              // �⺻ �̵��ӵ�
        stats.damageReduction = baseDamageReduction;      // ���� ���ذ��� (20%)
        stats.healthRegen = baseHealthRegen;            // ü�� ȸ����
        stats.regenInterval = baseRegenInterval;          // ȸ�� �ֱ�

        
        stats.criticalChance = baseCriticalChance;       // ũ��Ƽ�� Ȯ�� 10%
        stats.criticalDamage = baseCriticalDamage;       // ũ��Ƽ�� ������ 150%
        stats.cooldownReduction = baseCooldownReduction;    // ��Ÿ�� ���� 10%

        stats.InitializeStats();     // ���� ü���� �ִ�ü������ ����
    }

    protected override void InitializeStateHandler()
    {
        stateHandler = new StateHandler<Player>(this);

        stateHandler.RegisterState(new WarriorIdleState(stateHandler));
        stateHandler.RegisterState(new WarriorMoveState(stateHandler));
        stateHandler.RegisterState(new WarriorAttackState(stateHandler));

        stateHandler.RegisterState(new WarriorWSkillState(stateHandler));
        stateHandler.RegisterState(new WarriorESkillState(stateHandler));
        stateHandler.RegisterState(new WarriorRSkillState(stateHandler));

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
            stateHandler.ChangeState(typeof(WarriorWSkillState));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorESkillState));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            isSkillInProgress = true;
            stateHandler.ChangeState(typeof(WarriorRSkillState));
        }

        stateHandler.Update();
    }

    
}
