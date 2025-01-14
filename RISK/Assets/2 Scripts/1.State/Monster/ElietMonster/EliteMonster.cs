using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static NormalMonster;

public class EliteMonster : MonoBehaviour, ITakedamage
{
    [Header("���� Ÿ�� �� ��")]
    [Tooltip("���ݴ��")]
    public Transform target;
    protected Collider col;
    protected Rigidbody rb;
    public StateHandler<EliteMonster> eMHandler;

    [Tooltip("�𵨸�")]
    public GameObject model;
    [Tooltip("�𵨸��� �ִϸ�����")]
    public Animator animator; 
    [Tooltip("����� ��ƼŬ")]
    public ParticleSystem dieParticle;

    [Header("���� ����")]
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

    protected bool isDie = false;
    public bool isAtk = false;

    [Header("����� �����̻� üũ")]
    public Debuff monsterDebuff;
    public bool isSlow = false;
    public bool isBleeding = false;
    public bool isPoison = false;

    protected virtual void Start()
    {
        UnitManager.Instance.monsters.Add(this.gameObject);
        Targeting();
    }

    void Update()
    {
        
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
        eMHandler = new StateHandler<EliteMonster>(this);

        // ���µ� ���
        eMHandler.RegisterState(new EliteMonsterIdle(eMHandler));
        eMHandler.RegisterState(new EliteMonsterMove(eMHandler));
        //���� ��������
        //nMHandler.RegisterState(new NormalMonsterHit(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterStun(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterAirborne(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterDie(nMHandler));
        //// �ʱ� ���� ����
        eMHandler.ChangeState(typeof(EliteMonsterIdle));
    }

    public void Targeting()
    {
        foreach (var tr in UnitManager.Instance.players)
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

    public void DieParticle()
    {
        if (isDie == true)
        {
            ParticleSystem die = Instantiate(dieParticle, transform);
            die.Play();
        }
    }

    public void Die()
    {
        if (isDie == true)
        {
            Destroy(this.gameObject);
        }
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
        if (curHp <= 0)
        {
            isDie = true;
            this.eMHandler.ChangeState(typeof(EliteMonsterDie));
        }
        else
        {
            //isHit = true;
            //this.nMHandler.ChangeState(typeof(NormalMonsterHit));
        }
    }
}
