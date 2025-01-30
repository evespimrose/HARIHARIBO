using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EliteMonster : Monster
{
    [Header("紐ъ뒪???寃?諛?紐⑤뜽")]
    protected Collider col;
    public StateHandler<EliteMonster> eMHandler;

    [Tooltip("紐⑤뜽留곸쓽 ?좊땲硫붿씠??")]
    public Animator animator;
    [Tooltip("?щ쭩???뚰떚??")]
    public ParticleSystem dieParticle;

    [Header("?ㅽ궗2 愿??")]
    public GameObject skillBPrefab;
    [Header("?ㅽ궗3 愿??")]
    public GameObject skillCPrefab;

    protected override void Awake()
    {
        base.Awake();
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
        // ?곹깭???깅줉
        eMHandler.RegisterState(new EliteMonsterIdle(eMHandler));
        eMHandler.RegisterState(new EliteMonsterMove(eMHandler));
        eMHandler.RegisterState(new EliteMonsterDie(eMHandler));
        eMHandler.RegisterState(new EliteMonsterStun(eMHandler));
        //怨듦꺽 ?곹깭?⑦꽩
        eMHandler.RegisterState(new EliteMonsterSkillA(eMHandler));
        eMHandler.RegisterState(new EliteMonsterSkillB(eMHandler));
        eMHandler.RegisterState(new EliteMonsterSkillC(eMHandler));
        //// 珥덇린 ?곹깭 ?ㅼ젙
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
            photonView.RPC("DestroyMonster", RpcTarget.All);
        }
    }

    [PunRPC]
    public void DestroyMonster()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }

    public void AtkEnd()
    {
        StartCoroutine(AtkCoolTime());
    }

    public IEnumerator AtkCoolTime()
    {
        isAtk = true;
        Debug.Log("荑⑦????쒖옉");
        yield return new WaitForSeconds(atkDelay);
        Debug.Log("荑⑦???醫낅즺");
        isAtk = false;
    }

    [PunRPC]
    public override void DieStatChange()
    {
        base .DieStatChange();
        this.eMHandler.ChangeState(typeof(EliteMonsterDie));
    }

    [PunRPC]
    public void SyncStateChange(string stateName, PhotonMessageInfo info)
    {
        Type stateType = Type.GetType(stateName);
        if (stateType != null)
        {
            eMHandler.ChangeState(stateType);
        }
    }
}
