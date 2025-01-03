using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName =
    "Scriptable Object/Player", order = int.MaxValue)]
public class PlayerScroptableObjects : ScriptableObject
{
    [Tooltip("���� ������")]
    public float atkDamage;
    [Tooltip("���� �ӵ�")]
    public float atkSpeed;
    [Tooltip("�̵� �ӵ�")]
    public float moveSpeed;
    [Tooltip("ġ��Ÿ Ȯ��")]
    public float criticalChance;
    [Tooltip("ġ��Ÿ ������")]
    public float criticalDamage;
    [Tooltip("��ų ��Ÿ�� ������")]
    public float cooldownReduction;
    [Tooltip("���� ü��")]
    public float curHp;
}
