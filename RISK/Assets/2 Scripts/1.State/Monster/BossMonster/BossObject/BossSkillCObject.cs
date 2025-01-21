using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillCObject : MonoBehaviour
{
    public float moveSpeed = 10f;      
    public float moveDistance = 30f;   
    public int maxAtkCount = 1;        
    public float atkDamage;            

    private Vector3 startPos;   
    private List<GameObject> atkTargets = new List<GameObject>();
    private bool isSeting = false;

    public ParticleSystem[] particleSystems; 

    void Start()
    {
        moveSpeed = 5f;
        moveDistance = 30f;
        startPos = transform.position;
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (isSeting == false) return;
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * 10);
        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            Destroy(gameObject);
        }
    }

    public void Seting(float damage)
    {
        this.atkDamage = damage;
        isSeting = true;
        foreach (ParticleSystem particle in particleSystems)
        {
            if (!particle.isPlaying) 
            {
                particle.Play(); 
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!atkTargets.Contains(other.gameObject))
            {
                atkTargets.Add(other.gameObject); 
            }
            //유닛별 최대 공격 횟수 검사
            if (maxAtkCount == -1 || atkTargets.Count <= maxAtkCount)
            {
                //정면 범위 체크
                Vector3 directionToTarget = other.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToTarget);
                //정면 범위
                if (angle <= 90f)//90도 각도 내로만 공격을 인정
                {
                    other.GetComponent<ITakedamage>().Takedamage(atkDamage);
                    Debug.Log($"Player SkilC Hit");
                }
            }
        }
        else if (other.CompareTag("Wall"))
        {
            Destroy(this.gameObject);
        }
    }
}
