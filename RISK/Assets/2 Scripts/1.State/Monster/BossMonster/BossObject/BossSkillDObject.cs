using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillDObject : MonoBehaviour
{
    public float moveSpeed = 5f;       // 이동 속도
    public float moveDistance = 30f;  // 이동할 거리
    public float atkDamage;           // 공격 데미지

    private Vector3 startPos;    // 시작 위치
    private bool isSeting = false;

    private void Start()
    {
        moveSpeed = 5f;
        moveDistance = 30f;
    }

    void Update()
    {
        if (!isSeting) return;

        // 오브젝트의 로컬 전방으로 이동
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * 2);

        // 이동 거리 확인
        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            Destroy(gameObject); // 이동 거리 도달 시 오브젝트 삭제
        }
    }

    public void Seting(float damage)
    {
        this.atkDamage = damage;
        startPos = transform.position; // 초기 위치 저장
        isSeting = true;               // 이동 활성화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LocalPlayer") || other.gameObject.CompareTag("RemotePlayer"))
        {
            CalculateAndSendDamage(other.gameObject, atkDamage);
            Destroy(gameObject);
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
