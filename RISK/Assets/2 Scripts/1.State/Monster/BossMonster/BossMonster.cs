using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossMonster : MonoBehaviour, ITakedamage
{
    [Header("紐ъ뒪???寃?諛?紐⑤뜽")]
    [Tooltip("怨듦꺽???")]
    public Transform target;
    protected Collider col;
    private Rigidbody rb;
    public StateHandler<BossMonster> bMHandler;

    [Tooltip("紐⑤뜽留?")]
    public GameObject model;
    [Tooltip("紐⑤뜽留곸쓽 ?좊땲硫붿씠??")]
    public Animator animator;
    [Header("?ㅽ궗2 愿??")]
    public GameObject[] skillBParticle;
    public GameObject[] skillBFieldParticle;
    [Header("?ㅽ궗3 愿??")]
    public GameObject skillCPrefab;
    [Header("?ㅽ궗4 愿??")]
    public GameObject skillDPrefab;
    [Header("?ㅽ궗5 愿??")]
    public GameObject skillEPrefab;
    [Header("?ㅽ궗6 愿??")]
    public GameObject skillFPrefab;
    [Header("?ㅽ궗7 愿??")]
    public GameObject skillGPrefab;
    public bool isMoving = false;
    public bool isWall = false;
    public float skillFknockback = 50f; //skillF ?됰갚 嫄곕━
    public float skillFDamage = 10f; //skillF 怨듦꺽 ?곕?吏
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();//SkillF ?寃⑺븳 ??곷━?ㅽ듃

    [Header("紐ъ뒪???ㅽ뀩")]
    [Tooltip("?좊떅?ㅽ뀩")]
    public MonsterScriptableObjects monsterState;
    [Tooltip("怨듦꺽?곕?吏")]
    public float atkDamage;
    [Tooltip("?대룞?띾룄")]
    public float moveSpeed;
    [Tooltip("怨듦꺽踰붿쐞")]
    public float atkRange;
    [Tooltip("怨듦꺽?쒕젅??")]
    public float atkDelay;
    [Tooltip("?꾩옱泥대젰")]
    public float curHp;
    [Tooltip("理쒕?泥대젰")]
    protected float maxHp;

    protected bool isDie = false;
    public bool isAtk = false;

    [Header("?붾쾭???곹깭?댁긽 泥댄겕")]
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
        if (target == null) Targeting();
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
        Debug.Log("異⑸룎 諛쒖깮: " + other.gameObject.name);

        if (isMoving == false) return;
        if (other.gameObject.CompareTag("Wall")) isWall = true;
        else isWall = false;

        if (other.gameObject.CompareTag("Player") && !hitTargets.Contains(other.gameObject)) // 以묐났 ?ㅻ툕?앺듃 泥댄겕
        {
            Debug.Log("SkillF 怨듦꺽");
            other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);
            hitTargets.Add(other.gameObject);

            // ?됰갚 ?곸슜
            Vector3 knockbackDir = transform.position - other.transform.position;
            knockbackDir.y = 0f;

            // ?됰갚 ??議곗젙
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            float adjustedKnockback = skillFknockback * 4f;  // ?됰갚 諛곗쑉???ㅼ썙????媛뺥븯寃??곸슜
            playerRb.AddForce(-knockbackDir.normalized * adjustedKnockback, ForceMode.Impulse);

            Debug.Log("?됰갚 諛⑺뼢: " + knockbackDir.normalized + " ?? " + skillFknockback);
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
        foreach (GameObject particle in skillBParticle)
        {
            particle.SetActive(false);
        }

        foreach (GameObject fieldParticle in skillBFieldParticle)
        {
            fieldParticle.SetActive(false);
        }
        skillFPrefab.SetActive(false);
        skillGPrefab.SetActive(false);
    }

    protected void InitializeStateHandler()
    {
        bMHandler = new StateHandler<BossMonster>(this);

        //?곹깭???깅줉
        bMHandler.RegisterState(new BossMonsterIdle(bMHandler));
        bMHandler.RegisterState(new BossMonsterMove(bMHandler));
        bMHandler.RegisterState(new BossMonsterDie(bMHandler));
        //怨듦꺽 ?곹깭?⑦꽩
        bMHandler.RegisterState(new BossMonsterAtk(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillA(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillB(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillC(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillD(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillE(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillF(bMHandler));
        bMHandler.RegisterState(new BossMonsterSkillG(bMHandler));
        //珥덇린 ?곹깭 ?ㅼ젙
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
            if (target == null) target = tr.Value.transform;
            else if (target != null &&
                (Vector3.Distance(target.position, transform.position)
                < Vector3.Distance(tr.Value.transform.position, transform.position)))
            {
                target = tr.Value.transform;
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
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0f, currentRotation.y, 0f);
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
        Debug.Log("怨듦꺽荑⑦????쒖옉");
        yield return new WaitForSeconds(atkDelay);
        Debug.Log("怨듦꺽荑⑦???醫낅즺");
        isAtk = false;
    }

    public void Takedamage(float damage)
    {
        curHp -= Mathf.RoundToInt(damage);
        if (curHp <= 0)
        {
            isDie = true;
            this.bMHandler.ChangeState(typeof(BossMonsterDie));
        }
    }
}
