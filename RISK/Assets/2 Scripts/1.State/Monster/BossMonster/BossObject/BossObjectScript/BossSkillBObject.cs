using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossSkillBObject : MonoBehaviour
{
    public enum AtkType
    {
        Melee,
        Range
    }

    public float atkDamage;          // ���� ������
    public float moveDelay = 1f;  // �ʱ� ��� �ð�
    public float moveDuration = 2f;  // �̵� �ð�
    public float atkRange = 20f;     // ���� ����
    public float innerRadius = 15f;  // ���� �ݰ� (������ ����)

    public GameObject model;         // �� ��ü

    private Vector3 modelStartPos;   // �� ���� ��ġ
    private Vector3 modelTargetPos;  // �� ��ǥ ��ġ
    private float elapsedTime = 0f;  // ��� �ð�
    private bool isMoving = false;   // �� �̵� �� ����
    private bool isAtk = false; // ���� ���� ����

    void Start()
    {
        // �� ���� ��ġ�� ��ǥ ��ġ ����
        modelStartPos = transform.position + new Vector3(0, 10f, 0); // ���� ��ġ (y=10)
        modelTargetPos = transform.position;                         // ��ǥ ��ġ (y=0)
        model.transform.position = modelStartPos;                    // ���� ���� ��ġ�� ����
        StartCoroutine(MoveDelay());
    }

    private IEnumerator MoveDelay()
    {
        // �ʱ� ��� �ð�
        yield return new WaitForSeconds(moveDelay);
        isMoving = true; // �� �̵� ����
    }

    void Update()
    {
        if (!isMoving || isAtk) return; // �̵� ���� �ƴϰų� �̹� ������ ����� ��� ����

        // ���� y=10���� y=0���� �ε巴�� �̵�
        elapsedTime += Time.deltaTime;
        if (elapsedTime <= moveDuration)
        {
            float t = elapsedTime / moveDuration; // 0~1�� ����
            model.transform.position = Vector3.Lerp(modelStartPos, modelTargetPos, t);
        }
        else
        {
            // �̵��� �Ϸ�Ǹ� Atk ����
            isMoving = false;
            isAtk = true;
            SkillAAtk();
        }
    }

    public void Seting(Vector3 spwanPos, float damage)
    {
        this.transform.position = spwanPos; // ������Ʈ�� ��ġ ����
        this.atkDamage = damage;
    }

    private void SkillAAtk()
    {
        // ���� ����
        int num = Random.Range(0, 2);
        switch (num)
        {
            case 0:
                MeleeAtk();
                break;
            case 1:
                RangeAtk();
                break;
        }
    }

    public void MeleeAtk()
    {
        Vector3 atkCenter = transform.position; // ���� �߽���
        Collider[] cols = Physics.OverlapSphere(atkCenter, atkRange); // �ܺ� �ݰ� �� ��ü Ž��
        Debug.Log("���� ����");

        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<ITakedamage>()?.Takedamage(atkDamage);
                Debug.Log("���� ���� ����");
            }
        }
        Destroy(this.gameObject);
    }

    public void RangeAtk()
    {
        Vector3 atkCenter = transform.position; // ���� �߽���
        Collider[] cols = Physics.OverlapSphere(atkCenter, atkRange); // �ܺ� �ݰ� �� ��ü Ž��
        Debug.Log("���� ����");

        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                Vector3 dirToTarget = (col.transform.position - atkCenter).normalized;
                float dirTarget = Vector3.Distance(atkCenter, col.transform.position);

                // ���� �ݰ�� �ܺ� �ݰ� ������ ��ü�� �˻�
                if (dirTarget >= innerRadius && dirTarget <= atkRange)
                {
                    col.gameObject.GetComponent<ITakedamage>()?.Takedamage(atkDamage);
                    Debug.Log("���� ���� ����");
                }
            }
        }
        Destroy(this.gameObject);
    }
}
