using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class NormalMonster : MonoBehaviour, ITakedamage
{
    public enum MonsterType
    {
        Melee,
        Range
    }
    public MonsterType monsterType;
    [Tooltip("���ݴ��")]
    public Transform target;
    protected Collider col;
    protected Rigidbody rb;
    public StateHandler<NormalMonster> nMHandler;

    [Tooltip("�𵨸�")]
    public GameObject model;
    [Tooltip("�𵨸��� �ִϸ�����")]
    public Animator animator;
    [Tooltip("���ֽ���")]
    public MonsterScriptableObjects monsterState;
    [Tooltip("���ݵ�����")]
    public float atkDamage;
    [Tooltip("�̵��ӵ�")]
    public float moveSpeed;
    [Tooltip("���ݹ���")]
    public float atkRange;
    [Tooltip("���ݵ�����")]
    public float atkDelay;
    [Tooltip("����ü��")]
    public float curHp;
    [Tooltip("�ִ�ü��")]
    protected float maxHp;

    public bool isGround = false;
    [Tooltip("���")]
    public bool isAirborne = false;
    protected bool isDie = false;
    protected bool isHit = false;
    public bool isAtk = false;

    protected void Awake()
    {
        InitializeComponents();
        InitializeStateHandler();
    }

    protected virtual void Start()
    {
        UnitManager.Instance.monsters.Add(this.gameObject);
        Targeting();
    }

    private void Update()
    {
        nMHandler.Update();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
        this.atkDamage = monsterState.atkDamage;
        this.atkRange = monsterState.atkRange;
        this.atkDelay = monsterState.atkDelay;
        this.moveSpeed = monsterState.moveSpeed;
        this.curHp = monsterState.curHp;
        this.maxHp = curHp;
    }

    protected void InitializeStateHandler()
    {
        nMHandler = new StateHandler<NormalMonster>(this);

        // ���µ� ���
        nMHandler.RegisterState(new NormalMonsterIdle(nMHandler));
        nMHandler.RegisterState(new NormalMonsterMove(nMHandler));
        switch (monsterType)
        {
            case MonsterType.Melee:
                nMHandler.RegisterState(new NormalMonsterMeleeAtk(nMHandler));
                break;
            case MonsterType.Range:
                nMHandler.RegisterState(new NormalMonsterRangeAtk(nMHandler));
                break;
        }
        // �ʱ� ���� ����
        nMHandler.ChangeState(typeof(NormalMonsterIdle));
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

    public void Move()
    {
        transform.LookAt(target);
        Vector3 dir = (target.position - transform.position).normalized;
        Vector3 moveDir = transform.position + dir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(moveDir);
    }

    protected void Die()
    {
        Destroy(this.gameObject);
    }

    public IEnumerator AtkCoolTime()
    {
        Debug.Log("������Ÿ�� ����");
        yield return new WaitForSeconds(atkDelay);
        Debug.Log("������Ÿ�� ����");
        isAtk = false;
    }

    public void Takedamage(float damage)
    {
        curHp -= damage;

    }
}
