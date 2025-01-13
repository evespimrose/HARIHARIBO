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
    [Header("��ų2 ����")]
    public GameObject skillBPrefab;
    [Header("��ų3 ����")]
    public GameObject skillCPrefab;
    [Header("��ų4 ����")]
    public GameObject skillDPrefab;
    [Header("��ų4 ����")]
    public bool isMoving = false;
    public float skillFknockback = 5f; //skillF �˹� �Ÿ�
    public float skillFDamage = 10f; //skillF ���� ������
    public Vector3 skillFTargetPos;
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
        //monsterDebuff.DebuffCheck(this);
        bMHandler.Update();
        if (isDie == true)
        {
            //monsterDebuff.DebuffAllOff();
            bMHandler.ChangeState(typeof(NormalMonsterDie));
        }
    }

    void FixedUpdate()
    {
        if (isMoving && target != null)
        {
            //Ÿ�� ���� ���
            Vector3 direction = (skillFTargetPos - transform.position).normalized;

            //������ٵ� ����� ���� ���� �̵�
            rb.MovePosition(transform.position + direction * moveSpeed  * 10f * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMoving) return;

        if (other.CompareTag("Player") && !hitTargets.Contains(other.gameObject))//�ߺ�������Ʈ���� üũ
        {
            Debug.Log("SkillF ����");

            //����
            other.GetComponent<ITakedamage>().Takedamage(atkDamage);
            hitTargets.Add(other.gameObject);

            //�÷��̾� �˹� ó��
            Vector3 knockbackDir = (other.transform.position - transform.position).normalized * -1;
            Rigidbody otherRb = other.GetComponent<Rigidbody>();
            if (otherRb != null)
            {
                otherRb.AddForce(knockbackDir * skillFknockback, ForceMode.Impulse);
            }
        }
    }

    public void SkillFReset()
    {
        rb.velocity = Vector3.zero;
        hitTargets.Clear(); //����Ʈ ���� �ʱ�ȭ
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
        bMHandler.RegisterState(new BossMonsterSkillG(bMHandler));
        //�ʱ� ���� ����
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

    public void TargetLook()
    {
        //Ÿ�ٰ��� ���� ���͸� ���
        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;  //y �� ����
        //Ÿ�� �������� ȸ��
        Vector3 direction = targetPosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        //ȸ���� ���� ����
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
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

    public GameObject ObjSpwan(GameObject obj, Vector3 pos)
    {
        GameObject gameObject = Instantiate(obj);
        gameObject.transform.position = pos;
        return gameObject;
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
