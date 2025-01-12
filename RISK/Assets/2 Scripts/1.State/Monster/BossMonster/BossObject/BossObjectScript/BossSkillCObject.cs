using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillCObject : MonoBehaviour
{
    public float moveSpeed = 5f;       // �̵� �ӵ�
    public float moveDistance = 20f;  // �̵��� �Ÿ�
    public int maxAtkCount = 1;    // ���ֺ� �ִ� ���� Ƚ�� (-1: ������)
    public float atkDamage;           // ���� ������

    private Vector3 startPos;    // ���� ��ġ
    private Dictionary<GameObject, int> atkCounts = new Dictionary<GameObject, int>(); // ���ֺ� ���� Ƚ�� ���
    private bool isSeting = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (isSeting == false) return;
        // �������� �̵�
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
        isSeting = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� ��� �ʱ�ȭ
            if (!atkCounts.ContainsKey(other.gameObject))
            {
                atkCounts[other.gameObject] = 0;
            }

            // ���ֺ� �ִ� ���� Ƚ�� �˻�
            if (maxAtkCount == -1 || atkCounts[other.gameObject] < maxAtkCount)
            {
                // ���� ����
                var target = other.GetComponent<ITakedamage>();
                if (target != null)
                {
                    target.Takedamage(atkDamage);
                    atkCounts[other.gameObject]++; // �ش� ������ ���� Ƚ�� ����
                    Debug.Log($"Player {other.name} attacked {atkCounts[other.gameObject]} times.");
                }
            }
            else
            {
                Debug.Log($"Player {other.name} already attacked {atkCounts[other.gameObject]} times (max: {maxAtkCount}).");
            }
        }
    }
}
