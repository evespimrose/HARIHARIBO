using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : MonoBehaviour, ITakedamage
{
    [Header("���� Ÿ�� �� ��")]
    [Tooltip("���ݴ��")]
    public Transform target;
    protected Collider col;
    protected Rigidbody rb;
    public StateHandler<BossMonster> bMHandler;

    [Tooltip("�𵨸�")]
    public GameObject model;
    [Tooltip("�𵨸��� �ִϸ�����")]
    public Animator animator;

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

    //[Header("����� �����̻� üũ")]
    //public Debuff monsterDebuff;
    //public bool isSlow = false;
    //public bool isBleeding = false;
    //public bool isPoison = false;

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
        //monsterDebuff.DebuffCheck(this);
        bMHandler.Update();
        if (isDie == true)
        {
            //monsterDebuff.DebuffAllOff();
            bMHandler.ChangeState(typeof(NormalMonsterDie));
        }
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
        bMHandler = new StateHandler<BossMonster>(this);

        // ���µ� ���
        bMHandler.RegisterState(new BossMonsterIdle(bMHandler));
        bMHandler.RegisterState(new BossMonsterMove(bMHandler));
        //���� ��������
        bMHandler.RegisterState(new BossMonsterAtk(bMHandler));
        //���
        bMHandler.RegisterState(new BossMonsterDie(bMHandler));
        //��ų
        bMHandler.RegisterState(new BossMonsterSkillA(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillB(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillC(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillD(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillE(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillF(bMHandler));
        //// �ʱ� ���� ����
        bMHandler.ChangeState(typeof(BossMonsterIdle));
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
    public void DieParticle()
    {
        if (isDie == true)
        {
            //ParticleSystem die = Instantiate(dieParticle, transform);
            //die.Play();
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
            this.bMHandler.ChangeState(typeof(BossMonsterDie));
        }
        else
        {
            //isHit = true;
            //this.nMHandler.ChangeState(typeof(NormalMonsterHit));
        }
    }
}
