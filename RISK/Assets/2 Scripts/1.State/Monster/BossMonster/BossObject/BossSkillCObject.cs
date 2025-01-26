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
            Destroy(gameObject);
        }
    }

    public void Seting(float damage)
    {
        this.atkDamage = damage;
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
                    CalculateAndSendDamage(other.gameObject, atkDamage);
                    Debug.Log($"Player SkilC Hit");
                }
            }
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
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
