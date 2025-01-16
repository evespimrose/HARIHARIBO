using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDamageController : MonoBehaviour
{
    [Header("스킬 데미지 정보")]
    [SerializeField] private SkillDamageInfo[] skillInfos;  // Inspector에서 설정
    private Dictionary<string, SkillDamageInfo> skillDamageMap;

    private void Awake()
    {
        InitializeSkillMap();
    }

    private void InitializeSkillMap()
    {
        skillDamageMap = new Dictionary<string, SkillDamageInfo>();
        foreach (var info in skillInfos)
        {
            if (info != null)
            {
                skillDamageMap[info.skillName] = info;
            }
        }
    }
    public float GetSkillDamage(string skillName)
    {
        if (skillDamageMap.TryGetValue(skillName, out SkillDamageInfo info))
        {
            return info.GetDamage();
        }
        return 0f;
    }
    public void EnableDamage(string skillName)
    {
        if (skillDamageMap.TryGetValue(skillName, out SkillDamageInfo info))
        {
            info.EnableCollider();
        }
    }

    public void DisableDamage(string skillName)
    {
        if (skillDamageMap.TryGetValue(skillName, out SkillDamageInfo info))
        {
            info.DisableCollider();
        }
    }
}
