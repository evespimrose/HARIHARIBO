using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ITakedamage
{
    [Tooltip("���ֽ���")]
    public EnemyScriptableObjects enemyState;
    [Tooltip("�ִϸ�����")]
    public Animator animator;
    [Tooltip("���ݴ��")]
    public Transform target;
    [Tooltip("���ݵ�����")]
    public float atkDamage;
    [Tooltip("�̵��ӵ�")]
    public float moveSpeed;
    [Tooltip("���ݹ���")]
    public float atkRange;
    [Tooltip("����ü��")]
    public float curHp;
    [Tooltip("���ݵ�����")]
    public float atkDelay;

    protected Coroutine action;
    protected bool isAction = false;
    protected bool isHit = false;
    protected bool isDie = false;
    [SerializeField]
    protected bool isAirborne = false;
    protected bool isGround = false;

    protected float maxHp;
    protected Rigidbody rb;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        atkDamage = enemyState.atkDamage;
        moveSpeed = enemyState.moveSpeed;
        atkRange = enemyState.atkRange;
        curHp = enemyState.curHp;
        atkDelay = enemyState.atkDelay;
        maxHp = curHp;
    }

    public virtual void Start()
    {
        UnitManager.Instance.enemys.Add(this.gameObject);
        Targeting();
    }

    protected void Move()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 moveDir = transform.position + dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(moveDir);
    }

    protected void Targeting()
    {
        foreach(GameObject tr in UnitManager.Instance.players)
        {
            if (target == null) target = tr.transform;
            else if (target != null && 
                (Vector3.Distance(target.position, transform.position) 
                < Vector3.Distance(tr.transform.position, transform.position)))
            {
                target = tr.transform;
            }
        }
    }

    public void Takedamage(float damage)
    {
        if (isDie == true) return;
        isHit = true;
        curHp -= damage;
        if (curHp <= 0)
        {
            isDie = true;
            isAction = false;
        }
    }
}
