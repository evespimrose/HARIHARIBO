using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName =
    "Scriptable Object/Player", order = int.MaxValue)]
public class PlayerScroptableObjects : ScriptableObject
{
    [Tooltip("공격 데미지")]
    public float atkDamage;
    [Tooltip("공격 속도")]
    public float atkSpeed;
    [Tooltip("이동 속도")]
    public float moveSpeed;
    [Tooltip("치명타 확률")]
    public float criticalChance;
    [Tooltip("치병타 데미지")]
    public float criticalDamage;
    [Tooltip("스킬 쿨타임 감소율")]
    public float cooldownReduction;
    [Tooltip("현재 체력")]
    public float curHp;
}
