using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuff : MonoBehaviour
{
    public float fireDamage = 1f;
    public float fireInterval = 1f;
    public float fireDuration = 5f;

    private float fireTimer = 0f;
    private float damageTimer = 0f;
    private bool isFire = false;
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (isFire)
        {
            fireTimer -= Time.deltaTime;
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f)
            {
                FireDamage();
                damageTimer = fireInterval; // 화상 데미지 주기
            }
            if (fireTimer <= 0)
            {
                EndFire();
            }
        }
    }

    private void FireDamage()
    {
        player.Stats.currentHealth -= Mathf.RoundToInt(fireDamage); // 화상 데미지 적용
    }

    public void StartFire()
    {
        if (isFire == false)
        {
            isFire = true;
            fireTimer = fireDuration;
            damageTimer = 0f;
        }
        else
        {
            fireTimer = fireDuration;
        }
    }

    public void EndFire()
    {
        isFire = false;
    }

    public void Fire(float damage, float interval, float duration)
    {
        fireDamage = damage;
        fireInterval = interval;
        fireDuration = duration;
        StartFire();
    }
}
