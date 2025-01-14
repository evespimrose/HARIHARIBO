using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuff : MonoBehaviour
{
    public float health = 100f; // �÷��̾� ü��
    private float dotTimer = 0f; // ��Ʈ Ÿ�̸�
    private bool inFireField = false; // �� ���ǿ� �ִ��� ����

    private Player player;

    public void Fire(float damage)
    {
        player.GetComponent<ITakedamage>().Takedamage(damage);
    }

    public void EnterFireField()
    {
        inFireField = true;
        dotTimer = 5f; // ��Ʈ ���� �ð� ����
    }

    public void ExitFireField()
    {
        inFireField = false;
    }

    private void Update()
    {
        if (inFireField)
        {
            // ��Ʈ ������ ó��: ��Ʈ ���� �ð� ����
            dotTimer -= Time.deltaTime;
            if (dotTimer <= 0)
            {
                inFireField = false;
            }
        }
    }
}
