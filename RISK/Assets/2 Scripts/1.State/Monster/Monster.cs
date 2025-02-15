using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
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
    [Tooltip("사운드")]
    public AudioClip hitSoundClips;
    public AudioClip dieSoundClips;

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
    public float maxHp { get; protected set; }

    [Tooltip("드랍 걍험치")]
    public int exp;
    [Tooltip("드랍 돈")]
    public float won;

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
    public MonsterDebuff monsterDebuff;
    public bool isSlow = false;
    public bool isBleeding = false;
    public bool isPoison = false;

    //몸박데미지 쿨타임을 위한 부분
    [Tooltip("접촉공격데미지")]
    public float bodyAtkDamage = 10;
    protected Dictionary<GameObject, float> bodyAtkHit = new Dictionary<GameObject, float>();
    protected float bodyAtkCoolTime = 1f;

    public PhotonView photonView;

    protected virtual void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    protected void OnTriggerStay(Collider other)
    {
        if (isAirborne || isAirborneAction || isStun || isStunAction || isDie || isHit || isHitAction) return;
        if (other.gameObject.CompareTag("LocalPlayer") || other.gameObject.CompareTag("RemotePlayer"))
        {
            GameObject player = other.gameObject;
            float currentTime = Time.time;
            if (!bodyAtkHit.ContainsKey(player) || currentTime - bodyAtkHit[player] >= bodyAtkCoolTime)
            {
                //other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);
                Atk(other.gameObject, atkDamage);
                bodyAtkHit[player] = currentTime;
            }
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    var projectile = other.GetComponent<ProjectileMove>();
    //    if (projectile != null)
    //    {
    //        projectile.OnHit(other.ClosestPoint(transform.position));
    //    }
    //}

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

    public GameObject ObjSpwan(GameObject obj, Vector3 pos, Vector3 rot)
    {
        Quaternion rotation = Quaternion.LookRotation(rot);
        GameObject gameObject = PhotonNetwork.Instantiate(obj.name, pos, rotation);
        return gameObject;
    }

    public virtual void Takedamage(float damage)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        GameSoundManager.Instance.PlayMonsterEffectSound(hitSoundClips);
        curHp -= Mathf.RoundToInt(damage);
        photonView.RPC("SyncHealth", RpcTarget.All, curHp);
        if (curHp <= 0 && !isDie)
        {
            photonView.RPC("DieStatChange", RpcTarget.All);
        }
    }

    [PunRPC]
    public void SyncHealth(float newHp)
    {
        curHp = newHp;
    }

    [PunRPC]
    public virtual void DieStatChange()
    {
        isDie = true;
        GameManager.Instance.WhenMonsterDies?.Invoke(won);
        GameSoundManager.Instance.PlayMonsterEffectSound(dieSoundClips);
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

    // ✅ 공격 처리 함수 (화살 등은 PhotonView 필요 없음)
    public void Atk(GameObject target, float damage)
    {
        // 공격 시 자신이 가진 PhotonView를 사용해서 RPC 호출
        if (photonView != null && target != null)
        {
            PhotonView targetView = target.GetComponent<PhotonView>();
            if (targetView != null)
            {
                int targetID = targetView.ViewID;

                if (PhotonNetwork.IsMasterClient) // ✅ 파티장만 데미지 계산
                {
                    // 파티장에서만 RPC 호출해서 데미지 전파
                    photonView.RPC("ApplyDamageRPC", RpcTarget.All, targetID, damage);
                }
            }
        }
    }

    [PunRPC]
    protected void ApplyDamageRPC(int targetID, float damage)
    {
        // 타겟을 찾아서 데미지 적용
        PhotonView targetView = PhotonView.Find(targetID);
        if (targetView != null)
        {
            targetView.GetComponent<ITakedamage>().Takedamage(damage);
        }
    }
}
