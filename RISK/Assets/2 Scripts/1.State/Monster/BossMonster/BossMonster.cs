using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class BossMonster : Monster
{
    public string bossName = "진광대왕";
    
    [Header("몬스터 타겟 및 모델")]
    protected Collider col;
    public StateHandler<BossMonster> bMHandler;

    [Tooltip("모델링의 애니메이터")]
    public Animator animator;
    [Header("공격 관련")]
    public AudioClip[] atkSoundClips;
    [Header("스킬1 관련")]
    public AudioClip[] skillASoundClips;
    [Header("스킬2 관련")]
    public AudioClip[] skillBSoundClips;
    public GameObject[] skillBParticle;
    public GameObject[] skillBFieldParticle;
    [Header("스킬3 관련")]
    public AudioClip skillCSoundClips;
    public GameObject skillCPrefab;
    [Header("스킬4 관련")]
    public AudioClip skillDSoundClips;
    public GameObject skillDPrefab;
    [Header("스킬5 관련")]
    public AudioClip[] skillESoundClips;
    public GameObject skillEPrefab;
    [Header("스킬6 관련")]
    public AudioClip skillFSoundClips;
    public GameObject skillFPrefabA;
    public GameObject skillFPrefabB;
    [Header("스킬7 관련")]
    public AudioClip skillGSoundClips;
    public GameObject skillGPrefab;
    public bool isMoving = false;
    public bool isWall = false;
    public float skillFknockback = 50f; //skillF 넉백 거리
    public float skillFDamage = 10f; //skillF 공격 데미지
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();//SkillF 타격한 대상리스트

    public float chaseTime = 1f;
    public bool isChase = false;
    public bool isAction = false;

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

    // 벽과의 충돌이 시작되었을 때
    protected void OnCollisionEnter(Collision other)
    {
        if (isMoving == false) return;
        if (other.gameObject.CompareTag("Wall"))
        {
            isWall = true;
        }
        if (other.gameObject.CompareTag("LocalPlayer") || other.gameObject.CompareTag("RemotePlayer") && !hitTargets.Contains(other.gameObject)) // 중복 오브젝트 체크
        {
            Debug.Log("SkillF 공격");
            other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);
            hitTargets.Add(other.gameObject);

            // 넉백 적용
            Vector3 knockbackDir = transform.position - other.transform.position;
            knockbackDir.y = 0f;

            // 넉백 힘 조정
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            float adjustedKnockback = skillFknockback * 10f;  // 넉백 배율을 키워서 더 강하게 적용
            playerRb.AddForce(-knockbackDir.normalized * adjustedKnockback, ForceMode.Impulse);

            Debug.Log("넉백 방향: " + knockbackDir.normalized + " 힘: " + skillFknockback);
        }
    }

    // 벽과의 충돌이 종료되었을 때
    protected void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            isWall = false; // 벽과의 충돌이 종료되었으므로 isWall을 false로 설정
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

        //공격분류1 중에하나스테이트로 변환
        AtkA();

        yield return null;
        yield return new WaitUntil(() =>  isAtk == false);
        yield return null;

        StartCoroutine(Chase());
        yield return null;
        yield return new WaitUntil(() => isChase == false);
        yield return null;

        //공격분류2 중에하나 스테이트로 변환
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

        //이동기 중에하나 스테이트로 변환
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
