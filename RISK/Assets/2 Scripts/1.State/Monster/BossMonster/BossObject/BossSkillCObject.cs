using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillCObject : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float moveDistance = 30f;
    public int maxAtkCount = 1;
    public float atkDamage;

    private Vector3 startPos;
    private List<GameObject> atkTargets = new List<GameObject>();
    private bool isSeting = false;

    public ParticleSystem[] particleSystems;

    public PhotonView photonView;

    void Start()
    {
        moveSpeed = 5f;
        moveDistance = 30f;
        startPos = transform.position;
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (isSeting == false) return;
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * 10);
        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void Seting(float damage, PhotonView photonView)
    {
        this.atkDamage = damage;
        this.photonView = photonView;
        isSeting = true;
        foreach (ParticleSystem particle in particleSystems)
        {
            if (!particle.isPlaying)
            {
                particle.Play();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LocalPlayer") || other.gameObject.CompareTag("RemotePlayer"))
        {
            if (!atkTargets.Contains(other.gameObject))
            {
                atkTargets.Add(other.gameObject);
            }
            //유닛별 최대 공격 횟수 검사
            if (maxAtkCount == -1 || atkTargets.Count <= maxAtkCount)
            {
                //정면 범위 체크
                Vector3 directionToTarget = other.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToTarget);
                //정면 범위
                if (angle <= 90f)//90도 각도 내로만 공격을 인정
                {
                    //other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);

                    Debug.Log($"Player SkilC Hit");
                }
            }
        }
        else if (other.CompareTag("Wall"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // ✅ 공격 처리 함수 (화살 등은 PhotonView 필요 없음)
    private void Atk(GameObject target, float damage)
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
    public void ApplyDamageRPC(int targetID, float damage)
    {
        // 타겟을 찾아서 데미지 적용
        PhotonView targetView = PhotonView.Find(targetID);
        if (targetView != null)
        {
            targetView.GetComponent<ITakedamage>().Takedamage(damage);
        }
    }
}
