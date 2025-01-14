using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class BossSkillEObjectC : MonoBehaviour
{
    public float damagePerSecond = 5f; // �ʴ� ������
    public float duration = 5f; // �� ���� ���� �ð�

    private void Start()
    {
        // ���� �ð� �� �� ���� ����
        Destroy(gameObject, duration);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ��Ʈ ������ ó�� (�÷��̾ �� ���ǿ� ���� ��)
            PlayerDebuff playerHealth = other.GetComponent<PlayerDebuff>();
            if (playerHealth != null)
            {
                playerHealth.Fire(damagePerSecond * Time.deltaTime);
            }
        }
    }

    // A ������Ʈ�� �̵� ������ ���� ȸ���ϵ��� �߰�
    public void RotateTowards(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
