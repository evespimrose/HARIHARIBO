using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testdamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //SkillDamageInfo skillInfo = other.GetComponent<SkillDamageInfo>();
        //if (skillInfo != null && skillInfo.IsActive())
        //{
        //    TakeDamage(skillInfo.damage);
        //}
        print("¾Æ¾ß");
        var projectile = other.GetComponent<ProjectileMove>();
        if (projectile != null)
        {
            projectile.OnHit(other.ClosestPoint(transform.position));
        }
    }

}
