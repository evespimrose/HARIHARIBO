using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TrapSaw : MonoBehaviour, ITakedamage
{
    public float damage;
    public float curHp;
    private float maxHp;
    public float atkDelay = 1f;
    public GameObject model;
    public Transform target;
    private Vector3 movePos;
    private bool isAtk = false;
    private SphereCollider col;
    private Rigidbody rb;
    public MonsterScriptableObjects monsterState;
    public StateHandler<TrapSaw> sMHandler;

    public float atkDamage;
    public float moveSpeed;
    public float atkDuration;

    private void Awake()
    {
        InitializeComponents();
        InitializeStateHandler();
    }

    void Start()
    {
        movePos = (target.position - transform.position).normalized;
        UnitManager.Instance.monsters.Add(this.gameObject);
        Targeting();
    }

    void Update()
    {
        if (target == null) Targeting();    
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
        this.atkDelay = monsterState.atkDelay;
        this.moveSpeed = monsterState.moveSpeed;
        this.curHp = monsterState.curHp;
        this.maxHp = curHp;
    }

    protected void InitializeStateHandler()
    {
        sMHandler = new StateHandler<TrapSaw>(this);

        //// ���µ� ���
        //nMHandler.RegisterState(new NormalMonsterIdle(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterMove(nMHandler));
        //switch (monsterType)
        //{
        //    case MonsterType.Melee:
        //        nMHandler.RegisterState(new NormalMonsterMeleeAtk(nMHandler));
        //        break;
        //    case MonsterType.Range:
        //        nMHandler.RegisterState(new NormalMonsterRangeAtk(nMHandler));
        //        break;
        //}
        //nMHandler.RegisterState(new NormalMonsterHit(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterStun(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterAirborne(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterDie(nMHandler));
        //// �ʱ� ���� ����
        //nMHandler.ChangeState(typeof(NormalMonsterIdle));
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

    public IEnumerator AtkCoolTime()
    {
        yield return new WaitForSeconds(atkDelay);
        isAtk = false;
    }


    private void OnTriggerStay(Collider other)
    {
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
                print("아야");
                //데미지
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
            //isDie = true;
            //this.nMHandler.ChangeState(typeof(NormalMonsterDie));
        }
        else
        {
            //isHit = true;
            //this.nMHandler.ChangeState(typeof(NormalMonsterHit));
        }
    }
}
