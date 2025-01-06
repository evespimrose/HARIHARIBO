using UnityEngine;

[CreateAssetMenu(fileName = "EnemysStats", menuName =
    "Scriptable Object/Enemy", order = int.MaxValue)]
public class MonsterScriptableObjects : ScriptableObject
{
    [Tooltip("���ݵ�����")]
    public float atkDamage;
    [Tooltip("�̵��ӵ�")]
    public float moveSpeed;
    [Tooltip("���ݹ���")]
    public float atkRange;
    [Tooltip("����ü��")]
    public float curHp;
    [Tooltip("���ݵ�����")]
    public float atkDelay;
}
