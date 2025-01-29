using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class BossMonster : Monster
{
    public string bossName = "KingJinGwang";

    [Header("紐ъ뒪???寃?諛?紐⑤뜽")]
    protected Collider col;
    public StateHandler<BossMonster> bMHandler;

    [Tooltip("紐⑤뜽留곸쓽 ?좊땲硫붿씠??")]
    public Animator animator;
    [Header("怨듦꺽 愿??")]
    public AudioClip[] atkSoundClips;
    [Header("?ㅽ궗1 愿??")]
    public AudioClip[] skillASoundClips;
    [Header("?ㅽ궗2 愿??")]
    public AudioClip[] skillBSoundClips;
    public GameObject[] skillBParticle;
    public GameObject[] skillBFieldParticle;
    [Header("?ㅽ궗3 愿??")]
    public AudioClip skillCSoundClips;
    public GameObject skillCPrefab;
    [Header("?ㅽ궗4 愿??")]
    public AudioClip skillDSoundClips;
    public GameObject skillDPrefab;
    [Header("?ㅽ궗5 愿??")]
    public AudioClip[] skillESoundClips;
    public GameObject skillEPrefab;
    [Header("?ㅽ궗6 愿??")]
    public AudioClip skillFSoundClips;
    public GameObject skillFPrefabA;
    public GameObject skillFPrefabB;
    [Header("?ㅽ궗7 愿??")]
    public AudioClip skillGSoundClips;
    public GameObject skillGPrefab;
    public bool isMoving = false;
    public bool isWall = false;
    public float skillFknockback = 10f;
    public float skillFDamage = 10f; //skillF 怨듦꺽 ?곕?吏
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();//SkillF ?寃⑺븳 ??곷━?ㅽ듃

    public float chaseTime = 1f;
    public bool isChase = false;
    public bool isAction = false;

    public Coroutine action;

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

    protected void Update()
    {
        RemoveBodyAtkHit();
        if (target == null) Targeting();
        bMHandler.Update();
    }

    public void SkillFReset()
    {
        hitTargets.Clear();
    }

    public void RBStop()
    {
        rb.velocity = Vector3.zero;
    }

    // 踰쎄낵??異⑸룎???쒖옉?섏뿀????
    protected void OnCollisionEnter(Collision other)
    {
        if (isMoving == false) return;
        if (other.gameObject.CompareTag("Wall"))
        {
            isWall = true;
        }
        if (other.gameObject.CompareTag("LocalPlayer") || other.gameObject.CompareTag("RemotePlayer") && !hitTargets.Contains(other.gameObject)) // 以묐났 ?ㅻ툕?앺듃 泥댄겕
        {
            Debug.Log("SkillF 怨듦꺽");
            other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);
            //CalculateAndSendDamage(other.gameObject, atkDamage);
            hitTargets.Add(other.gameObject);

            // ?됰갚 ?곸슜
            Vector3 knockbackDir = other.transform.position - transform.position.normalized;
            knockbackDir.y = 0f;

            // ?됰갚 ??議곗젙
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            float adjustedKnockback = skillFknockback * 2;  // ?됰갚 諛곗쑉???ㅼ썙????媛뺥븯寃??곸슜
            playerRb.AddForce(knockbackDir * adjustedKnockback, ForceMode.Impulse);

            Debug.Log("?됰갚 諛⑺뼢: " + knockbackDir + " ?? " + skillFknockback);
        }
    }

    // 踰쎄낵??異⑸룎??醫낅즺?섏뿀????
    protected void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            isWall = false; // 踰쎄낵??異⑸룎??醫낅즺?섏뿀?쇰?濡?isWall??false濡??ㅼ젙
        }
    }


    protected void InitializeComponents()
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
        foreach (GameObject particle in skillBParticle)
        {
            particle.SetActive(false);
        }

        foreach (GameObject fieldParticle in skillBFieldParticle)
        {
            fieldParticle.SetActive(false);
        }
        skillFPrefabA.SetActive(false);
        skillFPrefabB.SetActive(false);
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
            UnitManager.Instance.monsters.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    public override void DieStatChange()
    {
        this.bMHandler.ChangeState(typeof(BossMonsterDie));
    }

    public void AtkOff()
    {
        isAtk = false;
    }

    public IEnumerator Chase()
    {
        isChase = true;
        yield return new WaitForSeconds(chaseTime);
        isChase = false;
    }

    public IEnumerator AtkSet()
    {
        isAction = true;
        yield return null;

        //怨듦꺽遺꾨쪟1 以묒뿉?섎굹?ㅽ뀒?댄듃濡?蹂??
        AtkA();

        yield return null;
        yield return new WaitUntil(() => isAtk == false);
        yield return null;

        StartCoroutine(Chase());
        yield return null;
        yield return new WaitUntil(() => isChase == false);
        yield return null;

        //怨듦꺽遺꾨쪟2 以묒뿉?섎굹 ?ㅽ뀒?댄듃濡?蹂??
        AtkB();
        yield return null;
        yield return new WaitUntil(() => isAtk == false);
        yield return null;

        Targeting();
        yield return null;

        StartCoroutine(Chase());
        yield return null;
        yield return new WaitUntil(() => isChase == false);
        yield return null;

        bMHandler.ChangeState(typeof(BossMonsterAtk));
        yield return null;
        yield return new WaitUntil(() => isAtk == false);
        yield return null;

        StartCoroutine(Chase());
        yield return null;
        yield return new WaitUntil(() => isChase == false);
        yield return null;

        //?대룞湲?以묒뿉?섎굹 ?ㅽ뀒?댄듃濡?蹂??
        AtkC();
        yield return null;
        yield return new WaitUntil(() => isAtk == false);
        yield return null;

        StartCoroutine(Chase());
        yield return null;
        yield return new WaitUntil(() => isChase == false);
        yield return null;

        isAction = false;
        yield return null;
    }

    protected void AtkA()
    {
        int a = Random.Range(0, 3);
        switch (a)
        {
            case 0:
                bMHandler.ChangeState(typeof(BossMonsterSkillA));
                break;
            case 1:
                bMHandler.ChangeState(typeof(BossMonsterSkillD));
                break;
            case 2:
                bMHandler.ChangeState(typeof(BossMonsterSkillE));
                break;
        }
    }

    protected void AtkB()
    {
        int b = Random.Range(0, 2);
        switch (b)
        {
            case 0:
                bMHandler.ChangeState(typeof(BossMonsterSkillB));
                break;
            case 1:
                bMHandler.ChangeState(typeof(BossMonsterSkillC));
                break;
        }
    }

    protected void AtkC()
    {
        int c = Random.Range(0, 2);
        switch (c)
        {
            case 0:
                bMHandler.ChangeState(typeof(BossMonsterSkillF));
                break;
            case 1:
                bMHandler.ChangeState(typeof(BossMonsterSkillG));
                break;
        }
    }
}
