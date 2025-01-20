using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteSkillCObjcect : MonoBehaviour
{
    public float bulletDamage = 10f;
    public float missileSpeed = 15f;
    public float missileDistance = 10f;

    private Vector3 targetPos;
    private bool isMoving = false;

    public void InitMissile(Vector3 direction, float distance)
    {
        targetPos = transform.position + direction * distance;
        targetPos.y = transform.position.y;
        isMoving = true;
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
            Destroy(gameObject);
        }
        else
        {
            Vector3 direction = targetPos - transform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, missileSpeed * Time.deltaTime * 2);
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<ITakedamage>().Takedamage(bulletDamage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
