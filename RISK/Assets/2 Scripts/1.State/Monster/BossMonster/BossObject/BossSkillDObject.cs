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

    public PhotonView view;

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
            PhotonNetwork.Destroy(gameObject); // 이동 거리 도달 시 오브젝트 삭제
        }
    }

    public void Seting(float damage, PhotonView photonView)
    {
        this.atkDamage = damage;
        this.view = photonView;
        startPos = transform.position; // 초기 위치 저장
        isSeting = true;               // 이동 활성화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LocalPlayer") || other.gameObject.CompareTag("RemotePlayer"))
        {
            //other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);
            Atk(other.gameObject, atkDamage);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    // ✅ 공격 처리 함수 (화살 등은 PhotonView 필요 없음)
    private void Atk(GameObject target, float damage)
    {
        // 공격 시 자신이 가진 PhotonView를 사용해서 RPC 호출
        if (view != null && target != null)
        {
            PhotonView targetView = target.GetComponent<PhotonView>();
            if (targetView != null)
            {
                int targetID = targetView.ViewID;

                if (PhotonNetwork.IsMasterClient) // ✅ 파티장만 데미지 계산
                {
                    // 파티장에서만 RPC 호출해서 데미지 전파
                    view.RPC("ApplyDamageRPC", RpcTarget.All, targetID, damage);
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
