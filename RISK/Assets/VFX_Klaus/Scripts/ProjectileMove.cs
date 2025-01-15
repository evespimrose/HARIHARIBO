using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    public float speed;
    public float fireRate;
    public float lifeTime = 5f;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    private Vector3 moveDirection;
    private float currentLifeTime;

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
        transform.forward = moveDirection; // 발사체를 이동 방향으로 회전
        currentLifeTime = lifeTime;
    }


    void Start()
    {
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();
            if (psMuzzle != null)
            {
                Destroy(muzzleVFX, psMuzzle.main.duration);
            }
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
        currentLifeTime = lifeTime;// 호출이 안됬을경우
    }

    void Update()
    {
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            Destroy(gameObject);
            return;
        }
        if (speed != 0)
        {
            transform.position += moveDirection * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    void OnCollisionEnter (Collision co)
    {
        speed = 0;

        ContactPoint contact = co.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if(hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            var psHit = hitVFX.GetComponent<ParticleSystem>();
            if (psHit != null) 
            {
                Destroy(hitVFX, psHit.main.duration);
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }
        Destroy(gameObject);
    }
    public void SetLifeTime(float time)
    {
        lifeTime = time;
        currentLifeTime = time;
    }
    public void OnHit(Vector3 hitPosition)
    {
        if (hitPrefab != null)
        {
            Instantiate(hitPrefab, hitPosition, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}