using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDamageInfo : MonoBehaviour
{
    [Header("��ų �⺻ ����")]
    public string skillName;    // ��ų �ĺ��� (��: "BasicAttack", "ESkill")
    public float damage;        // ��ų ������

    private Collider damageCollider;
    private bool isActive = false;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;  // Ʈ���ŷ� ����
            damageCollider.enabled = false;    // ���۽� ��Ȱ��ȭ
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

    // ���� �ݶ��̴� Ȱ��ȭ ���� Ȯ��
    public bool IsActive() => isActive;
}
