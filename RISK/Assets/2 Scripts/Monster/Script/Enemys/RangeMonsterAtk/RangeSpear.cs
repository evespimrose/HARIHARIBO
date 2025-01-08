using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangeSpear : MonoBehaviour
{
    public float moveSpeed;
    public float bulletLifeTime;
    private float atkDamage;
    private Transform target;
    private Vector3 targetPos;
    private Collider col;
    private bool isSeting = false;
    private float yPos;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        StartCoroutine(BulletDestroy());
        StartCoroutine(BulletMove());
    }

    private void Update()
    {
        if (isSeting == false) return;
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    private IEnumerator BulletMove()
    { 
        yield return new WaitUntil(() => isSeting == true);
        while (true)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public void BulletSeting(Transform tr, float Damage)
    {
        target = tr;
        atkDamage = Damage;
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
        yPos = transform.position.y;
        isSeting = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isSeting == false) return;
        if (other.gameObject.CompareTag("Player"))
        {
            print($"{other.name}À» °ø°Ý");
            other.gameObject.GetComponent<ITakedamage>().Takedamage(atkDamage);
            BulletHit();
        }
    }

    private IEnumerator BulletDestroy()
    {
        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(this.gameObject);
    }

    private void BulletHit()
    {
        Destroy(this.gameObject);
    }
}
