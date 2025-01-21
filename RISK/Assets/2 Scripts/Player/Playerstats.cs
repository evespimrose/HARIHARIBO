using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public event Action<float> OnHealthChanged;
    public event Action<int> OnLevelUp;
    public event Action<float> OnExpChanged;

    [Header("nickname")]
    public string nickName = null;

    [Header("level")]
    public int level = 1;
    public int healthPerLevel;
    public int attackPerLevel;
    public float maxExp = 100f;
    public float _currentExp = 0f;

    [Header("default")]
    public float maxHealth;
    private float _currentHealth;
    public float attackPower;
    public float moveSpeed;
    [Range(0f, 1f)]
    public float damageReduction;

    [Header("regen")]
    public float healthRegen;
    public float regenInterval;
    private float regenTimer = 0f;

    [Header("critical")]
    [Range(0f, 1f)]
    public float criticalChance;
    public float criticalDamage;
    [Range(0f, 1f)]
    public float cooldownReduction;

    public float currentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(_currentHealth);
        }
    }
    public float currentExp
    {
        get => _currentExp;
        set
        {
            _currentExp = value;
            OnExpChanged?.Invoke(_currentExp);
            if (_currentExp >= maxExp)
            {
                _currentExp -= maxExp;
                LevelUp();
            }
        }
    }

    public void InitializeStats()
    {
        currentHealth = maxHealth;
        currentExp = 0f;
    }

    public void LevelUp()
    {
        level++;
        maxHealth += healthPerLevel;
        attackPower += attackPerLevel;
        currentHealth = maxHealth;
        maxExp *= 1.2f;

        OnLevelUp?.Invoke(level);
    }
    public float CalculateDamage(float incomingDamage)
    {
        return incomingDamage * (1f - damageReduction);
    }

    public void UpadateHealthRegen(float deltaTime)
    {

        regenTimer += deltaTime;
        if (regenTimer >= regenInterval)
        {
            regenTimer = 0f;
            currentHealth += Mathf.RoundToInt(healthRegen);
        }
    }
}
