using UnityEngine;

[CreateAssetMenu(fileName = "EnemysStats", menuName =
    "Scriptable Object/Enemy", order = int.MaxValue)]
public class MonsterScriptableObjects : ScriptableObject
{
    [Tooltip("공격데미지")]
    public float atkDamage;
    [Tooltip("이동속도")]
    public float moveSpeed;
    [Tooltip("공격범위")]
    public float atkRange;
    [Tooltip("현재체력")]
    public float curHp;
    [Tooltip("공격딜레이")]
    public float atkDelay;

    [Tooltip("드랍 걍험치")]
    public int exp;
    [Tooltip("드랍 돈")]
    public int won;
}
