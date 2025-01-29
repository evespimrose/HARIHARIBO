using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RangeBullet : MonoBehaviour
{
    private Vector3 targetPos;
    private bool isSeting = false;
    private float atkDamage;
    private float yPos;

    public float moveSpeed;
    public float lifeTime;

    public PhotonView photonView;

    private void Start()
    {
        StartCoroutine(BulletMove());
        StartCoroutine(BulletLifeTime());
    }

    public void Seting(Vector3 pos, float damage, PhotonView view)
    {
        photonView = view;
        targetPos = pos;
        atkDamage = damage;
        isSeting = true;
    }

    private IEnumerator BulletMove()
    {
        yield return new WaitUntil(() => isSeting == true);
        Vector3 dir = (targetPos - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
        yPos = transform.position.y;
        while (true)
        {
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            yield return null;
        }
    }

    private IEnumerator BulletLifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LocalPlayer") || other.gameObject.CompareTag("RemotePlayer"))
        {
            //other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);
            Atk(other.gameObject, atkDamage);
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
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
