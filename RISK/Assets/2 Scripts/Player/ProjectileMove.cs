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

    public void Initialize(Vector3 direction, Player owner)
    {
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
        if (skillDamageInfo != null)
        {
            float damage = skillDamageInfo.GetDamage();
            CalculateAndSendDamage(co.gameObject, damage);
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

    // 데미지 계산 후 전송하는 메서드
    public void CalculateAndSendDamage(GameObject target, float dmg)
    {
        // 방장에서 데미지 계산 (여기서는 단순히 공격력으로 계산)
        float damage = dmg;

        // 방장만 데미지를 전송
        if (PhotonNetwork.IsMasterClient)
        {
            // PhotonView 컴포넌트를 명시적으로 가져옴
            PhotonView photonView = GetComponent<PhotonView>();

            if (photonView != null)
            {
                // photonView를 통해 RPC 호출
                photonView.RPC("ApplyDamageToClient", RpcTarget.All, target.GetPhotonView().ViewID, damage);
            }
        }
    }

    // RPC로 다른 클라이언트에 데미지 적용
    [PunRPC]
    public void ApplyDamageToClient(int targetPhotonViewID, float damage)
    {
        // PhotonView ID로 대상 객체 찾기
        PhotonView targetView = PhotonView.Find(targetPhotonViewID);

        // 대상 객체가 존재하면, ITakedamage 인터페이스를 통해 데미지를 적용
        if (targetView != null)
        {
            targetView.gameObject.GetComponent<ITakedamage>()?.Takedamage(damage);
        }
    }
}
