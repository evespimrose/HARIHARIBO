using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class NormalMonster : BaseCharacter
{
    public enum MonsterType
    {
        Melee,
        Range
    }
    public MonsterType monsterType;
    [Tooltip("���ݴ��")]
    public Transform target { get; private set; }
    private Collider col;

    [Tooltip("���ֽ���")]
    public MonsterScriptableObjects monsterState;
    [Tooltip("���ݹ���")]
    public float atkRange;
    [Tooltip("���ݵ�����")]
    public float atkDelay;
    [Tooltip("�𵨸�")]
    public GameObject model;

    public bool isGround = false;
    [Tooltip("���")]
    public bool isAirborne = false;
    protected bool isDie = false;
    protected bool isHit = false;
    public bool isAtk = false;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        this.atkDamage = monsterState.atkDamage;
        this.atkRange = monsterState.atkRange;
        this.atkDelay = monsterState.atkDelay;
        this.moveSpeed = monsterState.moveSpeed;
        this.curHp = monsterState.curHp;
        this.maxHp = curHp;
        animator = GetComponentInChildren<Animator>();
        InitializeState();
    }

    protected virtual void Start()
    {
        UnitManager.Instance.monsters.Add(this.gameObject);
        Targeting();
        InitializeState();
    }

    protected override void InitializeState()
    {
        this.ChangeState(new MonsterIdle());
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    public void Targeting()
    {
        foreach (GameObject tr in UnitManager.Instance.players)
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

    protected override void Die()
    {
        base.Die();
    }

    public IEnumerator AtkCoolTime()
    {
        Debug.Log("������Ÿ�� ����");
        yield return new WaitForSeconds(atkDelay);
        Debug.Log("������Ÿ�� ����");
        isAtk = false;
    }    
}
