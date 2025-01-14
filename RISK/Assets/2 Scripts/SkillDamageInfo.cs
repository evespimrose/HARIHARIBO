using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDamageInfo : MonoBehaviour
{
    [Header("스킬 기본 정보")]
    public string skillName;    // 스킬 식별자 (예: "BasicAttack", "ESkill")
    public float damage;        // 스킬 데미지

    private Collider damageCollider;
    private bool isActive = false;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;  // 트리거로 설정
            damageCollider.enabled = false;    // 시작시 비활성화
        }
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
