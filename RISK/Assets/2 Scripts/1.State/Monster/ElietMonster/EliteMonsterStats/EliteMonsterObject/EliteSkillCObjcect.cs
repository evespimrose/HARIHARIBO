using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteSkillCObjcect : MonoBehaviour
{
    public float bulletDamage = 10f;
    public float missileSpeed = 15f;
    public float missileDistance = 10f;

    private Vector3 targetPos;
    private bool isMoving = false;

    public PhotonView photonView;

    public void InitMissile(Vector3 direction, float distance)
    {
        targetPos = transform.position + direction * distance;
        targetPos.y = transform.position.y;
        isMoving = true;
    }

    public void Setting(float damage, PhotonView view)
    {
        photonView = view;
        bulletDamage = damage;
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveMissile();
        }
    }

    private void MoveMissile()
    {
        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Vector3 direction = targetPos - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, missileSpeed * Time.deltaTime * 2);
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LocalPlayer") || other.gameObject.CompareTag("RemotePlayer"))
        {
            //other.gameObject.GetComponent<ITakedamage>().Takedamage(bulletDamage);
            Atk(other.gameObject, bulletDamage);
            PhotonNetwork.Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
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
