using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDamageController : MonoBehaviour
{
    [Header("��ų ������ ����")]
    [SerializeField] private SkillDamageInfo[] skillInfos;  // Inspector���� ����
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
