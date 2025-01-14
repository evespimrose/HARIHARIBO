using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossMonster : MonoBehaviour, ITakedamage
{
    [Header("몬스터 타겟 및 모델")]
    [Tooltip("공격대상")]
    public Transform target;
    protected Collider col;
    public Rigidbody rb;
    public StateHandler<BossMonster> bMHandler;

    [Tooltip("모델링")]
    public GameObject model;
    [Tooltip("모델링의 애니메이터")]
    public Animator animator;
    [Header("스킬2 관련")]
    public GameObject skillBPrefab;
    [Header("스킬3 관련")]
    public GameObject skillCPrefab;
    [Header("스킬4 관련")]
    public GameObject skillDPrefab;
    [Header("스킬5 관련")]
    public GameObject skillEPrefab;
    [Header("스킬6 관련")]
    public bool isMoving = false;
    public bool isWall = false;
    public float skillFknockback = 50f; //skillF 넉백 거리
    public float skillFDamage = 10f; //skillF 공격 데미지
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();//SkillF 타격한 대상리스트

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
        Debug.Log("충돌 발생: " + other.gameObject.name);

        if (isMoving == false) return;
        if (other.gameObject.CompareTag("Wall")) isWall = true;
        else isWall = false;

        if (other.gameObject.CompareTag("Player") && !hitTargets.Contains(other.gameObject)) // 중복 오브젝트 체크
        {
            Debug.Log("SkillF 공격");
            other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);
            hitTargets.Add(other.gameObject);

            // 넉백 적용
            Vector3 knockbackDir = transform.position - other.transform.position;
            knockbackDir.y = 0f;

            // 넉백 힘 조정
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            float adjustedKnockback = skillFknockback * 4f;  // 넉백 배율을 키워서 더 강하게 적용
            playerRb.AddForce(-knockbackDir.normalized * adjustedKnockback, ForceMode.Impulse);

            Debug.Log("넉백 방향: " + knockbackDir.normalized + " 힘: " + skillFknockback);
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

        //상태들 등록
        bMHandler.RegisterState(new BossMonsterIdle(bMHandler));
        bMHandler.RegisterState(new BossMonsterMove(bMHandler));
        bMHandler.RegisterState(new BossMonsterDie(bMHandler));
        //공격 상태패턴
        bMHandler.RegisterState(new BossMonsterAtk(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillA(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillB(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillC(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillD(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillE(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillF(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillG(bMHandler));
        //초기 상태 설정
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
            this.bMHandler.ChangeState(typeof(BossMonsterDie));
        }
    }
}
