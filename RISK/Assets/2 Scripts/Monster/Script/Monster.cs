using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour, ITakedamage
{
    [Tooltip("유닛스텟")]
    public MonsterScriptableObjects monsterState;
    [Tooltip("애니메이터")]
    public Animator animator;
    [Tooltip("공격대상")]
    public Transform target;
    [Tooltip("공격데미지")]
    public float atkDamage;
    [Tooltip("이동속도")]
    public float moveSpeed;
    [Tooltip("공격범위")]
    public float atkRange;
    [Tooltip("현재체력")]
    public float curHp;
    [Tooltip("공격딜레이")]
    public float atkDelay;
    [Tooltip("모델링")]
    public GameObject model;
    [Tooltip("에어본")]
    public bool isAirborne = false;
    protected Coroutine action;
    protected bool isAction = false;
    protected bool isDie = false;
    protected bool isHit = false;
    protected bool isGround = false;

    protected float maxHp;
    protected Rigidbody rb;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        atkDamage = monsterState.atkDamage;
        moveSpeed = monsterState.moveSpeed;
        atkRange = monsterState.atkRange;
        curHp = monsterState.curHp;
        atkDelay = monsterState.atkDelay;
        maxHp = curHp;
    }

    public virtual void Start()
    {
        UnitManager.Instance.monsters.Add(this.gameObject);
        Targeting();
    }

    protected void Move()
    {
        if (target == null)
        {
            Targeting();
        }
        transform.LookAt(target);
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
