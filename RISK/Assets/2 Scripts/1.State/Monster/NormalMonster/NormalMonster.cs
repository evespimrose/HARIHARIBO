using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonster : Monster
{
    protected Collider col;
    public StateHandler<NormalMonster> nMHandler;

    [Tooltip("애니메이터")]
    public Animator animator;
    [Tooltip("사망시 파티클")]
    public ParticleSystem dieParticle;

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
        if (!isDie && isAirborne == true && isAirborneAction == false)
        {
            nMHandler.ChangeState(typeof(NormalMonsterAirborne));
        }
        else if (!isDie && !isAirborne && isStun && !isStunAction)
        {
            isStunAction = true;
            nMHandler.ChangeState(typeof(NormalMonsterStun));
        }
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
        this.won = monsterState.won;
        this.exp = monsterState.exp;
    }

    protected void InitializeStateHandler()
    {
        nMHandler = new StateHandler<NormalMonster>(this);
        //상태등록
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
        //시작상태
        nMHandler.ChangeState(typeof(NormalMonsterIdle));
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

    public IEnumerator AtkCoolTime()
    {
        Debug.Log("쿨타임 시작");
        yield return new WaitForSeconds(atkDelay);
        Debug.Log("쿨타임 종료");
        isAtk = false;
    }

    public override void Takedamage(float damage)
    {
        base.Takedamage(damage);
        if (isHit == false)
        {
            isHit = true;
            this.nMHandler.ChangeState(typeof(NormalMonsterHit));
        }
    }

    public override void DieStatChange()
    {
        this.nMHandler.ChangeState(typeof(NormalMonsterDie));
    }
}
