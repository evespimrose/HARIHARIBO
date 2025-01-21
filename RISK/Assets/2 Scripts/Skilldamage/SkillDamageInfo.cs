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
            float beforeCrit = damage;
            damage *= (1f + stats.criticalDamage);
        }
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
