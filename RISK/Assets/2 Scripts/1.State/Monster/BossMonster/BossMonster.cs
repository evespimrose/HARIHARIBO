using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossMonster : MonoBehaviour, ITakedamage
{
    [Header("���� Ÿ�� �� ��")]
    [Tooltip("���ݴ��")]
    public Transform target;
    protected Collider col;
    public Rigidbody rb;
    public StateHandler<BossMonster> bMHandler;

    [Tooltip("�𵨸�")]
    public GameObject model;
    [Tooltip("�𵨸��� �ִϸ�����")]
    public Animator animator;
    [Header("��ų2 ����")]
    public GameObject skillBPrefab;
    [Header("��ų3 ����")]
    public GameObject skillCPrefab;
    [Header("��ų4 ����")]
    public GameObject skillDPrefab;
    [Header("��ų5 ����")]
    public GameObject skillEPrefab;
    [Header("��ų6 ����")]
    public bool isMoving = false;
    public bool isWall = false;
    public float skillFknockback = 50f; //skillF �˹� �Ÿ�
    public float skillFDamage = 10f; //skillF ���� ������
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();//SkillF Ÿ���� ��󸮽�Ʈ

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
        if (isDie == true)
        {
            monsterDebuff.DebuffAllOff();
            bMHandler.ChangeState(typeof(NormalMonsterDie));
        }
        monsterDebuff.DebuffCheck(this);
        if (isAtk == true && isDie == false) return;
        bMHandler.Update();
    }

    public void SkillFReset()
    {
        //rb.velocity = Vector3.zero;
        hitTargets.Clear();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("�浹 �߻�: " + other.gameObject.name);

        if (isMoving == false) return;
        if (other.gameObject.CompareTag("Wall")) isWall = true;
        else isWall = false;

        if (other.gameObject.CompareTag("Player") && !hitTargets.Contains(other.gameObject)) // �ߺ� ������Ʈ üũ
        {
            Debug.Log("SkillF ����");
            other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);
            hitTargets.Add(other.gameObject);

            // �˹� ����
            Vector3 knockbackDir = transform.position - other.transform.position;
            knockbackDir.y = 0f;

            // �˹� �� ����
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            float adjustedKnockback = skillFknockback * 4f;  // �˹� ������ Ű���� �� ���ϰ� ����
            playerRb.AddForce(-knockbackDir.normalized * adjustedKnockback, ForceMode.Impulse);

            Debug.Log("�˹� ����: " + knockbackDir.normalized + " ��: " + skillFknockback);
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

        //���µ� ���
        bMHandler.RegisterState(new BossMonsterIdle(bMHandler));
        bMHandler.RegisterState(new BossMonsterMove(bMHandler));
        bMHandler.RegisterState(new BossMonsterDie(bMHandler));
        //���� ��������
        bMHandler.RegisterState(new BossMonsterAtk(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillA(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillB(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillC(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillD(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillE(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillF(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillG(bMHandler));
        //�ʱ� ���� ����
        bMHandler.ChangeState(typeof(BossMonsterIdle));
    }

    public void StartSkillCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    public GameObject ObjSpwan(GameObject obj, Vector3 pos)
    {
        GameObject gameObject = Instantiate(obj);
        gameObject.transform.position = pos;
        return gameObject;
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

    public void TargetLook(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            direction.y = 0;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
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

    public void AtkEnd()
    {
        StartCoroutine(AtkCoolTime());
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
    }
}
