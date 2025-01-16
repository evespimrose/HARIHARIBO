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

    [Header("스킬2 관련")]
    public GameObject skillBPrefab;
    [Header("스킬3 관련")]
    public GameObject skillCPrefab;

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
    protected bool isDieAction = false;
    public bool isAtk = false;
    public bool isStun = false;
    public bool isStunAction = false;

    [Header("디버프 상태이상 체크")]
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

    void Update()
    {
        if (target == null) Targeting();
        monsterDebuff.DebuffCheck(this);
        if (isDie == true && isDieAction == false)
        {
            monsterDebuff.DebuffAllOff();
            eMHandler.ChangeState(typeof(EliteMonsterDie));
            isDieAction = true;
        }
        else if (isStun == true && isStunAction == false)
        {
            isStunAction = true;
            eMHandler.ChangeState(typeof(EliteMonsterStun));
        }
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

    public GameObject ObjSpwan(GameObject obj, Vector3 pos)
    {
        GameObject gameObject = Instantiate(obj);
        gameObject.transform.position = pos;
        return gameObject;
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

    public void AtkEnd()
    {
        StartCoroutine(AtkCoolTime());
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
        curHp -= Mathf.RoundToInt(damage);
        if (curHp <= 0)
        {
            isDie = true;
            this.eMHandler.ChangeState(typeof(EliteMonsterDie));
        }
    }
}
