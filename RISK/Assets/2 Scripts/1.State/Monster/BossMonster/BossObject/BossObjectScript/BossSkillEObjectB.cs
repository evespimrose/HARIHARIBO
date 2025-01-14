using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillEObjectB : MonoBehaviour
{
    private Vector3 direction; // �̵� ����
    private float moveSpeed = 15f; // �ӵ�
    private float maxDistance = 10f; // �ִ� �̵� �Ÿ�
    public GameObject fireFieldPrefab; // �� ���� ������
    public float fireFieldDuration = 5f; // �� ���� ���� �ð�

    // �ʱ�ȭ �Լ� (���� �� �̵� �Ÿ� ����)
    public void Initialize(Vector3 dir, float damage)
    {
        direction = dir;
        Destroy(gameObject, maxDistance / moveSpeed); // �ִ� �̵� �Ÿ��� �ٵǸ� ����
    }

    private void Update()
    {
        // �̵� ó��
        transform.position += direction * moveSpeed * Time.deltaTime;

        // �� ���� ���� (����ü�� �̵��ϴ� ����)
        CreateFireField();
    }

    // �� ���� ���� �Լ�
    private void CreateFireField()
    {
        if (fireFieldPrefab != null)
        {
            // ����ü�� �̵��� ��ġ�� �� ���� ����
            GameObject fireField = Instantiate(fireFieldPrefab, transform.position, Quaternion.identity);
            Destroy(fireField, fireFieldDuration); // ���� �ð� �� �� ���� ����
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerDebuff>().Fire(10f); // ������ ó��
            Destroy(gameObject); // ����ü 2 ����
        }
    }
}
