using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ITakedamage
{
    [Tooltip("�÷��̾� ����")]
    public PlayerScroptableObjects playerStats;
    [Tooltip("�ִϸ�����")]
    public Animator animator;
    [Tooltip("���� ������")]
    public float atkDamage;
    [Tooltip("���� �ӵ�")]
    public float atkSpeed;
    [Tooltip("�̵� �ӵ�")]
    public float moveSpeed;
    [Tooltip("ġ��Ÿ Ȯ��")]
    public float criticalChance;
    [Tooltip("ġ��Ÿ ������")]
    public float criticalDamage;
    [Tooltip("��ų ��Ÿ�� ������")]
    public float cooldownReduction;
    [Tooltip("���� ü��")]
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
        //�̵�, rb.MovePosition����
    }

    public void Takedamage(float damage)
    {
        curHp -= damage;
    }
}
