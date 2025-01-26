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

    private void Start()
    {
        StartCoroutine(BulletMove());
        StartCoroutine(BulletLifeTime());
    }

    public void Seting(Vector3 pos, float damage)
    {
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
            CalculateAndSendDamage(other.gameObject, atkDamage);
            Destroy(this.gameObject);
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
