using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillEObject : MonoBehaviour
{
    public enum SkillType
    {
        First,
        Second
    }
    public SkillType skillType;
    public float bulletDamage = 10f;
    public float fireDamage = 5f;
    public float fireInterval = 1f;
    public float fireDuration = 5f;
    public float missileSpeed = 15f;
    public float missileDistance = 20f;

    private Vector3 targetPos;
    private bool isMoving = false;

    public void SetMissileProperties(float bulletDamage, float fireDamage, float fireInterval, float fireDuration)
    {
        this.bulletDamage = bulletDamage;
        this.fireDamage = fireDamage;
        this.fireInterval = fireInterval;
        this.fireDuration = fireDuration;
        missileSpeed = 15f;
        missileDistance = 20f;
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
            if (skillType == SkillType.First)
            {
                FireMissiles();
            }
            Destroy(gameObject);
        }
        else
        {
            Vector3 direction = targetPos - transform.position;
            direction.y = 1;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, missileSpeed * Time.deltaTime * 1.5f);
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
    }

    private void FireMissiles()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            GameObject missile = Instantiate(gameObject, transform.position, Quaternion.LookRotation(direction));
            missile.GetComponent<BossSkillEObject>().SetMissileType(2);
            missile.GetComponent<BossSkillEObject>().InitMissile(direction, missileDistance);
        }
    }

    public void SetMissileType(int type)
    {
        if (type == 1)
        {
            skillType = SkillType.First;
            targetPos = Vector3.zero;
            targetPos.y = transform.position.y;
        }
        else if (type == 2)
        {
            skillType = SkillType.Second;
            targetPos = transform.position + transform.forward * missileDistance;
            targetPos.y = transform.position.y;
        }
        isMoving = true;
    }

    public void InitMissile(Vector3 direction, float distance)
    {
        targetPos = transform.position + direction * distance;
        targetPos.y = transform.position.y;
        isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LocalPlayer") || other.gameObject.CompareTag("RemotePlayer"))
        {
            CalculateAndSendDamage(other.gameObject, bulletDamage);
            if (skillType == SkillType.First)
            {
                FireMissiles();
            }
            if (skillType == SkillType.Second)
            {
                other.gameObject.GetComponent<PlayerDebuff>().Fire(bulletDamage, fireInterval, fireDuration); // 화상 데미지
            }
            Destroy(gameObject);
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

