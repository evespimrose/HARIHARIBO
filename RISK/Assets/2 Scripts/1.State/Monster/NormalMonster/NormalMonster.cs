using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class NormalMonster : MonoBehaviour, ITakedamage
{
    public enum MonsterType
    {
        Melee,
        Range
    }
    [Header("몬스터타입")]
    public MonsterType monsterType;
    [Header("몬스터 타겟 및 모델")]
    [Tooltip("공격대상")]
    public Transform target;
    protected Collider col;
    protected Rigidbody rb;
    public StateHandler<NormalMonster> nMHandler;

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

    [Header("무력화 상태이상 체크")]
    [Tooltip("에어본")]
    public bool isDie = false;
    protected bool isDieAction = false;
    protected bool isHit = false;
    public bool isAirborne = false;
    protected bool isAirborneAction = false;
    public bool isAtk = false;
    [Tooltip("스턴")]
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

    private void Update()
    {
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

        // 상태들 등록
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
        // 초기 상태 설정
        nMHandler.ChangeState(typeof(NormalMonsterIdle));
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

        float airborneTime = 2f;//에어본지속시간
        float airborneDelay = airborneTime / 2f;
        float upDuration = airborneTime * 0.2f;//올라가는시간
        float downDuration = airborneTime * 0.3f;//내려가는시간
        float startY = this.model.transform.position.y;//돌아올위치
        float targetY = startY + 5f;//올라갈위치
        float timer = 0f;

        // 상승 단계 (0.4초 동안 위로 이동)
        while (timer < upDuration)
        {
            float newY = Mathf.Lerp(startY, targetY, timer / upDuration);  // 상승 비율을 시간에 맞게 계산
            this.model.transform.position = new Vector3(
                this.model.transform.position.x,
                newY,
                this.model.transform.position.z
            );
            timer += Time.deltaTime; // 시간 누적
            yield return null; // 프레임마다 업데이트
        }

        // 하강 단계 (0.6초 동안 원래 위치로 내려옴)
        while (timer < airborneTime)
        {
            float newY = Mathf.Lerp(targetY, startY, (timer - upDuration) / downDuration);  // 하강 비율을 시간에 맞게 계산
            this.model.transform.position = new Vector3(
                this.model.transform.position.x,
                newY,
                this.model.transform.position.z
            );
            timer += Time.deltaTime; // 시간 누적
            yield return null; // 프레임마다 업데이트
        }

        yield return new WaitForSeconds(airborneDelay);
        //에어본후 일어나는 시간
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
            this.nMHandler.ChangeState(typeof(NormalMonsterDie));
        }
        else
        {
            isHit = true;
            this.nMHandler.ChangeState(typeof(NormalMonsterHit));
        }
    }
}
