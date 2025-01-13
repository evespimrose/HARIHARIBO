using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : MonoBehaviour, ITakedamage
{
    [Header("몬스터 타겟 및 모델")]
    [Tooltip("공격대상")]
    public Transform target;
    protected Collider col;
    protected Rigidbody rb;
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
    [Header("스킬4 관련")]
    public bool isMoving = false;
    public float skillFknockback = 5f; //skillF 넉백 거리
    public float skillFDamage = 10f; //skillF 공격 데미지
    public Vector3 skillFTargetPos;
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
            //타겟 방향 계산
            Vector3 direction = (skillFTargetPos - transform.position).normalized;

            //리지드바디를 사용해 힘을 가해 이동
            rb.MovePosition(transform.position + direction * moveSpeed  * 10f * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMoving) return;

        if (other.CompareTag("Player") && !hitTargets.Contains(other.gameObject))//중복오브젝트인지 체크
        {
            Debug.Log("SkillF 공격");

            //공격
            other.GetComponent<ITakedamage>().Takedamage(atkDamage);
            hitTargets.Add(other.gameObject);

            //플레이어 넉백 처리
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
        hitTargets.Clear(); //리스트 내용 초기화
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
        //공격 상태패턴
        bMHandler.RegisterState(new BossMonsterAtk(bMHandler));
        //사망
        bMHandler.RegisterState(new BossMonsterDie(bMHandler));
        //스킬
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
        //타겟과의 차이 벡터를 계산
        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;  //y 값 고정
        //타겟 방향으로 회전
        Vector3 direction = targetPosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        //회전된 값을 적용
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
