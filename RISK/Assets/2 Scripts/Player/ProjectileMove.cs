using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class ProjectileMove : MonoBehaviour
{
    public float speed;
    public float fireRate;
    public float lifeTime = 5f;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    private Vector3 moveDirection;
    private float currentLifeTime;
    private SkillDamageInfo skillDamageInfo;
    private Player ownerPlayer;

    public PhotonView photonView;

    public void Initialize(Vector3 direction, Player owner)
    {
        photonView = owner.photonView;
        moveDirection = direction.normalized;
        transform.forward = moveDirection; // 발사체를 이동 방향으로 회전
        currentLifeTime = lifeTime;
        ownerPlayer = owner;

        skillDamageInfo = GetComponent<SkillDamageInfo>();
        if (skillDamageInfo != null)
        {
            skillDamageInfo.SetOwnerPlayer(ownerPlayer);
            skillDamageInfo.EnableCollider();
        }
    }


    void Start()
    {
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();
            if (psMuzzle != null)
            {
                Destroy(muzzleVFX, psMuzzle.main.duration);
            }
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
        currentLifeTime = lifeTime;// 호출이 안됬을경우
    }

    void Update()
    {
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            Destroy(gameObject);
            return;
        }
        if (speed != 0)
        {
            transform.position += moveDirection * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    void OnCollisionEnter (Collision co)
    {
        if (!co.gameObject.CompareTag("Monster")) return;
        if (skillDamageInfo != null)
        {
            float damage = skillDamageInfo.GetDamage();
            //co.gameObject.GetComponent<ITakedamage>().Takedamage(damage);
            Atk(co.gameObject, damage);
        }

        speed = 0;

        ContactPoint contact = co.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if(hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            var psHit = hitVFX.GetComponent<ParticleSystem>();
            if (psHit != null) 
            {
                Destroy(hitVFX, psHit.main.duration);
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }
        Destroy(gameObject);
    }
    public void SetLifeTime(float time)
    {
        lifeTime = time;
        currentLifeTime = time;
    }
    public void OnHit(Vector3 hitPosition)
    {
        if (hitPrefab != null)
        {
            Instantiate(hitPrefab, hitPosition, Quaternion.identity);
        }
        Destroy(gameObject);
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
