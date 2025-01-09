using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSaw : MonoBehaviour
{
    public GameObject model;
    public Transform target;
    private Vector3 movePos;
    private bool isAtk = false;
    private SphereCollider col;

    public float atkDamage;
    public float moveSpeed;
    public float atkDuration;

    private void Awake()
    {
        col = GetComponent<SphereCollider>();
    }

    void Start()
    {
        movePos = (target.position - transform.position).normalized;
    }

    void Update()
    {
        Vector3 targetDirection = (target.position - transform.position).normalized;

        targetDirection.y = 0;
        transform.position += targetDirection * moveSpeed * Time.deltaTime;

        model.transform.Rotate(-10, 0, 0);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isAtk == true || other.CompareTag("Player") == false) return;
        Atk();
    }

    private void Atk()
    {
        float sphereRadius = col.radius;
        Vector3 sphereCenter = col.center;

        Collider[] hitCol = Physics.OverlapSphere(transform.position + sphereCenter, sphereRadius);
        foreach (Collider hitCollider in hitCol)
        {
            if (hitCollider.CompareTag("Player"))
            {
                print("아야");
                //데미지
            }
        }
        isAtk = true;
        Invoke("AtkCollTime", atkDuration);
    }

    private void AtkCollTime()
    {
        isAtk = false;
    }
}
