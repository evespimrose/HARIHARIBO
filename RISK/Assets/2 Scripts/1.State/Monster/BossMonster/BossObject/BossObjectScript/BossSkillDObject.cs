using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillDObject : MonoBehaviour
{
    public float moveSpeed = 5f;       // �̵� �ӵ�
    public float moveDistance = 20f;  // �̵��� �Ÿ�
    public float atkDamage;           // ���� ������

    private Vector3 startPos;    // ���� ��ġ
    private bool isSeting = false;

    void Update()
    {
        if (!isSeting) return;

        // ������Ʈ�� ���� �������� �̵�
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // �̵� �Ÿ� Ȯ��
        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            Destroy(gameObject); // �̵� �Ÿ� ���� �� ������Ʈ ����
        }
    }

    public void Seting(float damage)
    {
        this.atkDamage = damage;
        startPos = transform.position; // �ʱ� ��ġ ����
        isSeting = true;               // �̵� Ȱ��ȭ
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<ITakedamage>()?.Takedamage(atkDamage);
            Destroy(gameObject);
        }
    }
}
