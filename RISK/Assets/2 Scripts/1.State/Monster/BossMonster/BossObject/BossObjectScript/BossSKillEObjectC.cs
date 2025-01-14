using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class BossSkillEObjectC : MonoBehaviour
{
    public float damagePerSecond = 5f; // 초당 데미지
    public float duration = 5f; // 불 장판 지속 시간

    private void Start()
    {
        // 일정 시간 후 불 장판 삭제
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 도트 데미지 처리 (플레이어가 불 장판에 있을 때)
            PlayerDebuff playerHealth = other.GetComponent<PlayerDebuff>();
            if (playerHealth != null)
            {
                playerHealth.Fire(damagePerSecond * Time.deltaTime);
            }
        }
    }

    // A 오브젝트의 이동 방향을 따라 회전하도록 추가
    public void RotateTowards(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
