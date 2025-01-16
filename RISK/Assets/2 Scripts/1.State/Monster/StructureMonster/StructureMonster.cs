using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class StructureMonster : MonoBehaviour, ITakedamage
{
    public float atkDamage;
    public float curHp;
    private float maxHp;
    public float moveSpeed;
    public float atkDuration = 1f;
    public GameObject model;
    public Transform target;
    public MonsterScriptableObjects monsterState;
    public StateHandler<StructureMonster> sMHandler;

    protected Vector3 movePos;
    protected bool isAtk = false;
    protected SphereCollider col;
    protected Rigidbody rb;

    public bool isAirborne = false;
    protected bool isAirborneAction = false;
    public bool isStun = false;
    public bool isStunAction = false;
    protected bool isDie = false;
    private bool isHit = false;
    private Coroutine hit = null;

    [Header("몬스터 디버프")]
    public Debuff monsterDebuff;
    public bool isSlow = false;
    public bool isBleeding = false;
    public bool isPoison = false;

    private void Awake()
    {
        InitializeComponents();
        InitializeStateHandler();
    }

    void Start()
    {
        UnitManager.Instance.monsters.Add(this.gameObject);
        Targeting();
    }

    void Update()
    {
        if (isDie == true)
        {
            sMHandler.ChangeState(typeof(StructureDie));
        }
        if (isAirborne == true)
        {
            sMHandler.ChangeState(typeof(StructureAirborne));
        }
        if (isAirborne == false && isStun == true)
        {
            sMHandler.ChangeState(typeof(StructureStun));
        }
        if (isHit == false) hit = null;
        sMHandler.Update();
    }

    public void Move()
    {
        Vector3 targetDirection = (target.position - transform.position).normalized;

        targetDirection.y = 0;
        transform.position += targetDirection * moveSpeed * Time.deltaTime;

        model.transform.Rotate(-10, 0, 0);
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
        this.atkDamage = monsterState.atkDamage;
        this.atkDuration = monsterState.atkDelay;
        this.moveSpeed = monsterState.moveSpeed;
        this.curHp = monsterState.curHp;
        this.maxHp = curHp;
    }

    protected void InitializeStateHandler()
    {
        sMHandler = new StateHandler<StructureMonster>(this);

        sMHandler.RegisterState(new StructureIdle(sMHandler));
        sMHandler.RegisterState(new StructureAirborne(sMHandler));
        sMHandler.RegisterState(new StructureStun(sMHandler));
        sMHandler.RegisterState(new StructureDie(sMHandler));
        sMHandler.RegisterState(new StructureMove(sMHandler));

        sMHandler.ChangeState(typeof(StructureIdle));
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
        if (target != null) movePos = (target.position - transform.position).normalized;
        if (target == null) sMHandler.ChangeState(typeof(StructureIdle));
    }

    public IEnumerator AtkCoolTime()
    {
        yield return new WaitForSeconds(atkDuration);
        isAtk = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isAirborne || isAirborneAction || isStun || isStunAction || isDie) return;
        if (isAtk == true || other.CompareTag("Player") == false) return;
        Atk();
    }

    private void Atk()
    {
        float sphereRadius = col.radius;
        Vector3 sphereCenter = col.center;

        Collider[] hitCol = Physics.OverlapSphere(transform.position + sphereCenter, sphereRadius);
        foreach (Collider hitCollider in hitCol)
        {
            if (hitCollider.CompareTag("Player"))
            {
                print("플레이어 타격");
                hitCollider.GetComponent<ITakedamage>().Takedamage(atkDamage);
            }
        }
        isAtk = true;
        StartCoroutine(AtkCoolTime());
    }

    public void Takedamage(float damage)
    {
        curHp -= Mathf.RoundToInt(damage);
        if (curHp <= 0)
        {
            isDie = true;
            this.sMHandler.ChangeState(typeof(StructureDie));
        }
        else
        {
            if (hit != null)
            {
                StopCoroutine(hit);
            }
            hit = StartCoroutine(Hit());
        }
    }

    private IEnumerator Hit()
    {
        isHit = true;
        yield return new WaitForSeconds(0.2f);
        isHit = false;
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

    public void Die()
    {
        if (isDie == true)
        {
            UnitManager.Instance.monsters.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}
