using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillCObject : MonoBehaviour
{
    public float moveSpeed = 20f;      
    public float moveDistance = 20f;   
    public int maxAtkCount = 1;        
    public float atkDamage;            

    private Vector3 startPos;   
    private List<GameObject> atkTargets = new List<GameObject>();
    private bool isSeting = false;

    public ParticleSystem[] particleSystems; 

    void Start()
    {
        startPos = transform.position;
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (isSeting == false) return;
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * 2);
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
            //���ֺ� �ִ� ���� Ƚ�� �˻�
            if (maxAtkCount == -1 || atkTargets.Count <= maxAtkCount)
            {
                //���� ���� üũ
                Vector3 directionToTarget = other.transform.position - transform.position;
                float angle = Vector3.Angle(transform.forward, directionToTarget);
                //���� ����
                if (angle <= 90f)//90�� ���� ���θ� ������ ����
                {
                    other.GetComponent<ITakedamage>().Takedamage(atkDamage);
                    Debug.Log($"Player SkilC Hit");
                }
            }
        }
    }
}
