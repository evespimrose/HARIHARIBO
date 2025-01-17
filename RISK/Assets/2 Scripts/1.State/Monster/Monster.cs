using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour, ITakedamage
{
    public enum MonsterType
    {
        Melee,
        Range,
        Structure,
        Elite,
        Boss
    }
    [Header("몬스터 타입")]
    public MonsterType monsterType;

    [Tooltip("공격대상")]
    public Transform target;
    protected Rigidbody rb;
    public GameObject model;

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

    [Tooltip("���")]
    public bool isAtk = false;
    protected bool isDie = false;
    protected bool isDieAction = false;
    public bool isAirborne = false;
    protected bool isAirborneAction = false;
    public bool isStun = false;
    public bool isStunAction = false;
    public bool isHit = false;
    public bool isHitAction = false;

    [Header("디버프 상태이상 체크")]
    public Debuff monsterDebuff;
    public bool isSlow = false;
    public bool isBleeding = false;
    public bool isPoison = false;

    //몸박데미지 쿨타임을 위한 부분
    [Tooltip("접촉공격데미지")]
    public float bodyAtkDamage = 1;
    protected Dictionary<GameObject, float> bodyAtkHit = new Dictionary<GameObject, float>();
    protected float bodyAtkCoolTime = 1f; 

    protected void OnTriggerStay(Collider other)
    {
        if (isAirborne || isAirborneAction || isStun || isStunAction || isDie || isHit || isHitAction) return;
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            float currentTime = Time.time;
            if (!bodyAtkHit.ContainsKey(player) || currentTime - bodyAtkHit[player] >= bodyAtkCoolTime)
            {
                player.GetComponent<ITakedamage>().Takedamage(bodyAtkDamage);
                bodyAtkHit[player] = currentTime;
            }
        }
    }

    //업데이트에서 돌리기
    protected void RemoveBodyAtkHit()
    {
        List<GameObject> playersToRemove = new List<GameObject>();
        float currentTime = Time.time;
        foreach (var entry in bodyAtkHit)
        {
            GameObject player = entry.Key;
            float lastDamageTime = entry.Value;
            if (currentTime - lastDamageTime >= bodyAtkCoolTime)
            {
                playersToRemove.Add(player);
            }
        }
        foreach (var player in playersToRemove)
        {
            bodyAtkHit.Remove(player);
        }
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

    public GameObject ObjSpwan(GameObject obj, Vector3 pos)
    {
        GameObject gameObject = Instantiate(obj);
        gameObject.transform.position = pos;
        return gameObject;
    }

    public virtual void Takedamage(float damage)
    {
        curHp -= Mathf.RoundToInt(damage);
        if (curHp <= 0)
        {
            DieStatChange();
        }
    }

    public virtual void DieStatChange()
    {

    }

    public void StartAirborne()
    {
        if (isAirborneAction == false)
        {
            StartCoroutine(Airborne());
        }
    }

    protected IEnumerator Airborne()
    {
        this.isAirborneAction = true;
        float airborneTime = 2f;
        float airborneDelay = airborneTime / 2f;
        float upDuration = airborneTime * 0.2f;
        float downDuration = airborneTime * 0.3f;
        float startY = this.model.transform.position.y;
        float targetY = startY + 5f;
        float timer = 0f;

        while (timer < upDuration)
        {
            float newY = Mathf.Lerp(startY, targetY, timer / upDuration);  
            this.model.transform.position = new Vector3(
                this.model.transform.position.x,
                newY,
                this.model.transform.position.z
            );
            timer += Time.deltaTime; 
            yield return null; 
        }

        while (timer < airborneTime)
        {
            float newY = Mathf.Lerp(targetY, startY, (timer - upDuration) / downDuration); 
            this.model.transform.position = new Vector3(
                this.model.transform.position.x,
                newY,
                this.model.transform.position.z
            );
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(airborneDelay);
        this.isAirborneAction = false;
        this.isAirborne = false;
    }
}
