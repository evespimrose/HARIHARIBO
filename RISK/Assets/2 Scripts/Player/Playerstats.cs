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
    public int healthPerLevel = 10;
    public int attackPerLevel = 2;
    public float maxExp = 100f;
    public float _currentExp = 0f;

    [Header("default")]
    public float maxHealth = 100;
    private float _currentHealth;
    public float attackPower = 10;
    public float moveSpeed;
    [Range(0f, 1f)]
    public float damageReduction = 0.1f;

    [Header("regen")]
    public float healthRegen = 1f;
    public float regenInterval = 1f;
    private float regenTimer = 0f;

    [Header("critical")]
    [Range(0f, 1f)]
    public float criticalChance = 0.05f;
    public float criticalDamage = 1.5f;
    [Range(0f, 1f)]
    public float cooldownReduction = 0f;

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

    public PlayerStats()
    {
        InitializeStats();
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
