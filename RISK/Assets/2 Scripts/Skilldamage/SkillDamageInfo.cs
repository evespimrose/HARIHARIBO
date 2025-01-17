using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillDamageInfo : MonoBehaviour
{
    [Header("스킬 기본 정보")]
    public string skillName;    
    [Range(0f, 500f)]
    public float damagePercent;  // 공격력 계수

    private Collider damageCollider;
    private Player ownerPlayer;
    private bool isActive = false;
    private bool isCritical = false;

    [Header("공격 타입")]
    public bool isBasicAttack = false;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;  // 트리거로 설정
            damageCollider.enabled = false;    // 시작시 비활성화
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 대상이 데미지를 받을 수 있는지 확인
        ITakedamage damageable = other.GetComponent<ITakedamage>();
        if (damageable != null && isActive)
        {
            float damage = GetDamage();
            damageable.Takedamage(damage);
        }
    }
    public void SetOwnerPlayer(Player player)
    {
        ownerPlayer = player;     
    }
    public float GetDamage()
    {
        if (ownerPlayer == null) return 0f;

        PlayerStats stats = ownerPlayer.Stats;
        float damage;

        Debug.Log($"[{skillName}] 현재 공격력: {stats.attackPower}");
        Debug.Log($"[{skillName}] 데미지 계수: {damagePercent}%");

        if (isBasicAttack)
        {
            damage = stats.attackPower;
            Debug.Log($"[{skillName}] 기본공격 데미지: {damage}");
        }
        else
        {
            damage = stats.attackPower * (damagePercent / 100f);
            Debug.Log($"[{skillName}] 스킬 기본 데미지: {damage} (공격력: {stats.attackPower} * 계수: {damagePercent}%)");
        }
        if (Random.value <= stats.criticalChance)
        {
            isCritical = true;
            float beforeCrit = damage;
            damage *= (1f + stats.criticalDamage);
            Debug.Log($"[{skillName}] 크리티컬! {beforeCrit} -> {damage} (크리티컬 데미지: {stats.criticalDamage})");
        }
        Debug.Log($"[{skillName}] 최종 데미지: {damage} {(isCritical ? "(크리티컬!)" : "")}");
        return damage;
      
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

    // 현재 콜라이더 활성화 상태 확인
    public bool IsActive() => isActive;
}
