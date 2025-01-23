using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SkillType
{
    Damage,
    DamageAndHeal,
    DamageAndBuff,  
    Knockback        
}
public class SkillDamageInfo : MonoBehaviour
{
    [Header("스킬 기본 정보")]
    public string skillName;
    public SkillType skillType = SkillType.Damage;

    [Header("데미지 정보")]
    [Range(0f, 500f)]
    public float damagePercent;  // 공격력 계수

    [Header("공격 타입")]
    public bool isBasicAttack = false;
    public bool isAirborneAttack = false;
    public bool isStunAttack = false;

    [Header("추가 효과 정보")]
    public float healPercent;    // 힐링 계수
    public float attackBuffPercent;    // 공격력 증가량
    public float buffDuration;   // 버프 지속시간
    public float knockbackForce; // 넉백 힘
    public float knockbackDuration; // 넉백 지속시간

    private Collider damageCollider;
    private Player ownerPlayer;
    private bool isActive = false;
    private bool isCritical = false;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }
        if (ownerPlayer == null)
        {
            ownerPlayer = GetComponentInParent<Player>();
            if (ownerPlayer != null)
            {
                Debug.Log($"[{skillName}] Player를 부모에서 찾음: {ownerPlayer.name}");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || ownerPlayer == null) return;

        Player targetPlayer = other.GetComponent<Player>();
        bool isAlly = targetPlayer != null && targetPlayer.TeamID == ownerPlayer.TeamID;

        switch (skillType)
        {
            case SkillType.Damage:
                if (!isAlly)
                {
                    HandleDamageAndEffects(other);
                }
                break;

            case SkillType.DamageAndHeal:
                if (isAlly)
                {
                    // 아군이면 힐
                    float healAmount = GetHealAmount();
                    targetPlayer.Heal(healAmount);
                }
                else
                {
                    // 적군이면 데미지
                    HandleDamageAndEffects(other);
                }
                break;

            case SkillType.DamageAndBuff:
                if (isAlly)
                {
                    // 아군이면 공격력 버프
                    targetPlayer.ApplyAttackBuff(attackBuffPercent, buffDuration);
                }
                else
                {
                    // 적군이면 데미지
                    HandleDamageAndEffects(other);
                }
                break;

            case SkillType.Knockback:
                if (!isAlly)
                {
                    HandleDamageAndEffects(other);
                    if (other.TryGetComponent<Rigidbody>(out var rb))
                    {
                        Vector3 direction = (other.transform.position - transform.position).normalized;
                        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);
                        StartCoroutine(ResetKnockback(rb));
                    }
                }
                break;
        }
    }

    private void HandleDamageAndEffects(Collider other)
    {
        // 데미지 처리
        HandleDamage(other);

        // 몬스터인 경우 에어본/스턴 효과 적용
        Monster monster = other.GetComponent<Monster>();
        if (monster != null)
        {
            if (isAirborneAttack)
            {
                monster.isAirborne = true;
            }

            if (isStunAttack)
            {
                monster.isStun = true;
            }
        }
    }

    private void HandleDamage(Collider other)
    {
        ITakedamage damageable = other.GetComponent<ITakedamage>();
        if (damageable != null)
        {
            float damage = GetDamage();
            damageable.Takedamage(damage);
        }
    }

    private IEnumerator ResetKnockback(Rigidbody rb)
    {
        yield return new WaitForSeconds(knockbackDuration);
        rb.velocity = Vector3.zero;
    }

    public float GetDamage()
    {
        if (ownerPlayer == null) return 0f;

        PlayerStats stats = ownerPlayer.Stats;
        float damage;

        if (isBasicAttack)
        {
            damage = stats.attackPower;
        }
        else
        {
            damage = stats.attackPower * (damagePercent / 100f);
        }
        if (Random.value <= stats.criticalChance)
        {
            isCritical = true;
            damage *= (1f + stats.criticalDamage);
        }
        return damage;
    }

    public float GetHealAmount()
    {
        if (ownerPlayer == null) return 0f;
        return ownerPlayer.Stats.attackPower * (healPercent / 100f);
    }

    public void SetOwnerPlayer(Player player)
    {
        ownerPlayer = player;
    }

    public void EnableCollider()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = true;
            isActive = true;
        }
    }

    public void DisableCollider()
    {
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
            isActive = false;
        }
    }

    public bool IsActive() => isActive;
}
