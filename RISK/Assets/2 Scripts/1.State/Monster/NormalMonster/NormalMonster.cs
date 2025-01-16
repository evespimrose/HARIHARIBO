using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonster : MonoBehaviour, ITakedamage
{
    public enum MonsterType
    {
        Melee,
        Range
    }
    [Header("����Ÿ��")]
    public MonsterType monsterType;
    [Header("���� Ÿ�� �� ��")]
    [Tooltip("���ݴ��")]
    public Transform target;
    protected Collider col;
    protected Rigidbody rb;
    public StateHandler<NormalMonster> nMHandler;

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
    [Tooltip("���ݵ����")]
    public float atkDelay;
    [Tooltip("����ü��")]
    public float curHp;
    [Tooltip("�ִ�ü��")]
    protected float maxHp;

    [Header("����ȭ �����̻� üũ")]
    [Tooltip("���")]
    public bool isDie = false;
    protected bool isDieAction = false;
    protected bool isHit = false;
    public bool isAirborne = false;
    protected bool isAirborneAction = false;
    public bool isAtk = false;
    [Tooltip("����")]
    public bool isStun = false;
    public bool isStunAction = false;

    [Header("����� �����̻� üũ")]
    public Debuff monsterDebuff;
    public bool isSlow = false;
    public bool isBleeding = false;
    public bool isPoison = false;

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
        if (target == null) Targeting();
        monsterDebuff.DebuffCheck(this);
        nMHandler.Update();
        if (isDie == true && isDieAction == false)
        {
            monsterDebuff.DebuffAllOff();
            nMHandler.ChangeState(typeof(NormalMonsterDie));
            isDieAction = true;
        }
        else if (isAirborne == true && isAirborneAction == false)
        {
            nMHandler.ChangeState(typeof(NormalMonsterAirborne));
        }
        else if (isAirborne == false && isStun == true && isStunAction == false)
        {
            nMHandler.ChangeState(typeof(NormalMonsterStun));
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
        nMHandler.RegisterState(new NormalMonsterHit(nMHandler));
        nMHandler.RegisterState(new NormalMonsterStun(nMHandler));
        nMHandler.RegisterState(new NormalMonsterAirborne(nMHandler));
        nMHandler.RegisterState(new NormalMonsterDie(nMHandler));
        // �ʱ� ���� ����
        nMHandler.ChangeState(typeof(NormalMonsterIdle));
    }

    public void Targeting()
    {
        foreach (var tr in UnitManager.Instance.players)
        {
            if (target == null) target = tr.Value.transform;
            else if (target != null &&
                (Vector3.Distance(target.position, transform.position)
                < Vector3.Distance(tr.Value.transform.position, transform.position)))
            {
                target = tr.Value.transform;
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

    public void StartAirborne()
    {
        if (isAirborneAction == false)
        {
            StartCoroutine(Airborne());
        }
    }

    private IEnumerator Airborne()
    {
        this.isAirborneAction = true;

        float airborneTime = 2f;//������ӽð�
        float airborneDelay = airborneTime / 2f;
        float upDuration = airborneTime * 0.2f;//�ö󰡴½ð�
        float downDuration = airborneTime * 0.3f;//�������½ð�
        float startY = this.model.transform.position.y;//���ƿ���ġ
        float targetY = startY + 5f;//�ö���ġ
        float timer = 0f;

        // ��� �ܰ� (0.4�� ���� ���� �̵�)
        while (timer < upDuration)
        {
            float newY = Mathf.Lerp(startY, targetY, timer / upDuration);  // ��� ������ �ð��� �°� ���
            this.model.transform.position = new Vector3(
                this.model.transform.position.x,
                newY,
                this.model.transform.position.z
            );
            timer += Time.deltaTime; // �ð� ����
            yield return null; // �����Ӹ��� ������Ʈ
        }

        // �ϰ� �ܰ� (0.6�� ���� ���� ��ġ�� ������)
        while (timer < airborneTime)
        {
            float newY = Mathf.Lerp(targetY, startY, (timer - upDuration) / downDuration);  // �ϰ� ������ �ð��� �°� ���
            this.model.transform.position = new Vector3(
                this.model.transform.position.x,
                newY,
                this.model.transform.position.z
            );
            timer += Time.deltaTime; // �ð� ����
            yield return null; // �����Ӹ��� ������Ʈ
        }

        yield return new WaitForSeconds(airborneDelay);
        //����� �Ͼ�� �ð�
        this.isAirborneAction = false;
        this.isAirborne = false;
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
        curHp -= Mathf.RoundToInt(damage);
        if (curHp <= 0)
        {
            isDie = true;
            this.nMHandler.ChangeState(typeof(NormalMonsterDie));
        }
        else
        {
            isHit = true;
            this.nMHandler.ChangeState(typeof(NormalMonsterHit));
        }
    }
}
