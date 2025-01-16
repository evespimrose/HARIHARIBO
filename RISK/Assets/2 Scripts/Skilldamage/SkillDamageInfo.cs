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
    bool isCritical = false;

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
        ownerPlayer = GetComponentInParent<Player>();
    }
    public float GetDamage()
    {
        if (ownerPlayer == null) return 0f;

        PlayerStats stats = ownerPlayer.Stats;
        float damage;

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
