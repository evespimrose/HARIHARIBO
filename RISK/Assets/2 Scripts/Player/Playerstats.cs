using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Playerstats 
{
    public event Action<int> OnHealthChanged;
    public event Action<int> OnLevelUp;

    [Header("Ã¼·Â")]
    public int level = 1;
    public int healthPerLevel = 10;
    public int attackPerLevel = 2;

    [Header("±âº» ½ºÅÝ")]
    public int maxHealth = 100;
    private int _currentHealth;
    public float attackPower = 10;          
    public float moveSpeed = 2f;

    [Header("Ãß°¡ ½ºÅÝ")]
    [Range(0f, 1f)]
    public float criticalChance = 0.05f;
    public float criticalDamage = 1.5f;
    [Range(0f, 1f)]
    public float cooldownReduction = 0f;

    public int currentHealth { get => _currentHealth; 
        set{
            _currentHealth = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke(_currentHealth);
        }
    }

    public Playerstats() 
    {
        InitializeStats();
    }
    public void InitializeStats()
    {
        currentHealth = maxHealth;       
    }

    public void LevelUp()
    {
        level++;
        maxHealth += healthPerLevel;
        attackPower += attackPerLevel;
        currentHealth = maxHealth;

        OnLevelUp?.Invoke(level);
    }
}
