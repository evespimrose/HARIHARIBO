using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeBullet : MonoBehaviour
{
    private Vector3 targetPos;
    private bool isSeting = false;
    private float atkDamage;
    private float yPos;

    public float moveSpeed;
    public float lifeTime;

    private void Start()
    {
        StartCoroutine(BulletMove());
        StartCoroutine(BulletLifeTime());
    }

    public void Seting(Vector3 pos, float damage)
    {
        targetPos = pos;
        atkDamage = damage;
        isSeting = true;
    }

    private IEnumerator BulletMove()
    {
        yield return new WaitUntil(() => isSeting == true);
        Vector3 dir = (targetPos - transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
        yPos = transform.position.y;
        while (true)
        {
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            yield return null;
        }
    }

    private IEnumerator BulletLifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<ITakedamage>().Takedamage(atkDamage);
            Destroy(this.gameObject);
        }
    }
}
