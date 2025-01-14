using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuff : MonoBehaviour
{
    public float health = 100f; // 플레이어 체력
    private float dotTimer = 0f; // 도트 타이머
    private bool inFireField = false; // 불 장판에 있는지 여부

    private Player player;

    public void Fire(float damage)
    {
        player.GetComponent<ITakedamage>().Takedamage(damage);
    }

    public void EnterFireField()
    {
        inFireField = true;
        dotTimer = 5f; // 도트 지속 시간 갱신
    }

    public void ExitFireField()
    {
        inFireField = false;
    }

    private void Update()
    {
        if (inFireField)
        {
            // 도트 데미지 처리: 도트 지속 시간 갱신
            dotTimer -= Time.deltaTime;
            if (dotTimer <= 0)
            {
                inFireField = false;
            }
        }
    }
}
