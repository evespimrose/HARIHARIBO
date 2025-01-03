using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITakedamage
{
    [Tooltip("플레이어 스텟")]
    public PlayerScroptableObjects playerStats;
    [Tooltip("애니메이터")]
    public Animator animator;
    [Tooltip("공격 데미지")]
    public float atkDamage;
    [Tooltip("공격 속도")]
    public float atkSpeed;
    [Tooltip("이동 속도")]
    public float moveSpeed;
    [Tooltip("치명타 확률")]
    public float criticalChance;
    [Tooltip("치병타 데미지")]
    public float criticalDamage;
    [Tooltip("스킬 쿨타임 감소율")]
    public float cooldownReduction;
    [Tooltip("현재 체력")]
    public float curHp;

    private float maxHp;
    private Rigidbody rb;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        atkDamage = playerStats.atkDamage;
        atkSpeed = playerStats.atkSpeed;
        moveSpeed = playerStats.moveSpeed;
        criticalChance = playerStats.criticalChance;
        criticalDamage = playerStats.criticalDamage;
        cooldownReduction = playerStats.cooldownReduction;
        curHp = playerStats.curHp;
        maxHp = curHp;
    }

    public virtual void Start()
    {
        UnitManager.Instance.players.Add(this.gameObject);
    }

    public void Move()
    {
        //이동, rb.MovePosition으로
    }

    public void Takedamage(float damage)
    {
        curHp -= damage;
    }
}
