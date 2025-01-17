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
    public float missileDistance = 10f;

    private Vector3 targetPos;
    private bool isMoving = false;

    public void SetMissileProperties(float bulletDamage, float fireDamage, float fireInterval, float fireDuration)
    {
        this.bulletDamage = bulletDamage;
        this.fireDamage = fireDamage;
        this.fireInterval = fireInterval;
        this.fireDuration = fireDuration;
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
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<ITakedamage>().Takedamage(bulletDamage);
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
}

