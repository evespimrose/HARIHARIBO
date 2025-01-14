using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static NormalMonster;

public class EliteMonster : MonoBehaviour, ITakedamage
{
    [Header("몬스터 타겟 및 모델")]
    [Tooltip("공격대상")]
    public Transform target;
    protected Collider col;
    protected Rigidbody rb;
    public StateHandler<EliteMonster> eMHandler;

    [Tooltip("모델링")]
    public GameObject model;
    [Tooltip("모델링의 애니메이터")]
    public Animator animator; 
    [Tooltip("사망시 파티클")]
    public ParticleSystem dieParticle;

    [Header("몬스터 스텟")]
    [Tooltip("유닛스텟")]
    public MonsterScriptableObjects monsterState;
    [Tooltip("공격데미지")]
    public float atkDamage;
    [Tooltip("이동속도")]
    public float moveSpeed;
    [Tooltip("공격범위")]
    public float atkRange;
    [Tooltip("공격딜레이")]
    public float atkDelay;
    [Tooltip("현재체력")]
    public float curHp;
    [Tooltip("최대체력")]
    protected float maxHp;

    protected bool isDie = false;
    public bool isAtk = false;

    [Header("디버프 상태이상 체크")]
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

        // 상태들 등록
        eMHandler.RegisterState(new EliteMonsterIdle(eMHandler));
        eMHandler.RegisterState(new EliteMonsterMove(eMHandler));
        //공격 상태패턴
        //nMHandler.RegisterState(new NormalMonsterHit(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterStun(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterAirborne(nMHandler));
        //nMHandler.RegisterState(new NormalMonsterDie(nMHandler));
        //// 초기 상태 설정
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
        Debug.Log("공격쿨타임 시작");
        yield return new WaitForSeconds(atkDelay);
        Debug.Log("공격쿨타임 종료");
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
