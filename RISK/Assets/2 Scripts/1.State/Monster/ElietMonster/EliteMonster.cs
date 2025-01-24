using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EliteMonster : Monster
{
    [Header("몬스터 타겟 및 모델")]
    protected Collider col;
    public StateHandler<EliteMonster> eMHandler;

    [Tooltip("모델링의 애니메이터")]
    public Animator animator;
    [Tooltip("사망시 파티클")]
    public ParticleSystem dieParticle;

    [Header("스킬2 관련")]
    public GameObject skillBPrefab;
    [Header("스킬3 관련")]
    public GameObject skillCPrefab;

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
        RemoveBodyAtkHit();
        if (target == null) Targeting();
        monsterDebuff.DebuffCheck(this);
        eMHandler.Update();
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
        this.won = monsterState.won;
        this.exp = monsterState.exp;
    }

    protected void InitializeStateHandler()
    {
        eMHandler = new StateHandler<EliteMonster>(this);
        // 상태들 등록
        eMHandler.RegisterState(new EliteMonsterIdle(eMHandler));
        eMHandler.RegisterState(new EliteMonsterMove(eMHandler));
        eMHandler.RegisterState(new EliteMonsterDie(eMHandler));
        eMHandler.RegisterState(new EliteMonsterStun(eMHandler));
        //공격 상태패턴
        eMHandler.RegisterState(new EliteMonsterSkillA(eMHandler));
        eMHandler.RegisterState(new EliteMonsterSkillB(eMHandler));
        eMHandler.RegisterState(new EliteMonsterSkillC(eMHandler));
        //// 초기 상태 설정
        eMHandler.ChangeState(typeof(EliteMonsterIdle));
    }

    public override void Takedamage(float damage)
    {
        base.Takedamage(damage);
        if (!isDie && isStun && isStunAction == false)
        {
            isStunAction = true;
            eMHandler.ChangeState(typeof(EliteMonsterStun));
        }
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
            UnitManager.Instance.monsters.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    public void AtkEnd()
    {
        StartCoroutine(AtkCoolTime());
    }

    public IEnumerator AtkCoolTime()
    {
        isAtk = true;
        Debug.Log("쿨타임 시작");
        yield return new WaitForSeconds(atkDelay);
        Debug.Log("쿨타임 종료");
        isAtk = false;
    }

    public override void DieStatChange()
    {
        this.eMHandler.ChangeState(typeof(EliteMonsterDie));
    }
}
