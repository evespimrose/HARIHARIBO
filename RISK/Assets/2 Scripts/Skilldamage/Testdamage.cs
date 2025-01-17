using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testdamage : MonoBehaviour,ITakedamage
{

    private void OnTriggerEnter(Collider other)
    {
        SkillDamageInfo skillInfo = other.GetComponent<SkillDamageInfo>();
        if (skillInfo != null && skillInfo.IsActive())
        {
            float damage = skillInfo.GetDamage();
            Debug.Log($"받은 데미지: {damage} from {skillInfo.skillName}");
        }
        print("아야");
        var projectile = other.GetComponent<ProjectileMove>();
        if (projectile != null)
        {
            projectile.OnHit(other.ClosestPoint(transform.position));
        }
    }
    public void Takedamage(float damage)
    {
        // TODO: HP 감소 등 실제 데미지 처리 로직 구현
        Debug.Log($"데미지 받음: {damage}");
    }

}
