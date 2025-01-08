using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterMeleeAtk : BaseState<NormalMonster>
{
    public NormalMonsterMeleeAtk(StateHandler<NormalMonster> handler) : base(handler) { }


    public float atkDuration = 5f;
    public float atkHitTime = 0.4f;
    private float curTime = 0;
    private bool isAtk = false;
    public override void Enter(NormalMonster monster)
    {
        Debug.Log("MeleeAtk���� ����");
        monster.animator.SetTrigger("Atk");
        curTime = 0;
        isAtk = false;
    }

    public override void Update(NormalMonster monster)
    {
        if (atkDuration - curTime < 0.1f)
        {
            //��������
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
        }
        if (atkHitTime - curTime < 0.1f && isAtk == false)
        {
            Atk(monster);
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(NormalMonster monster)
    {
        Debug.Log("MeleeAtk���� ����");
    }

    private void Atk(NormalMonster monster)
    {
        Debug.Log("MeleeAtk����");
        monster.isAtk = true;
        isAtk = true;
        //monster.transform.position = ������������ �߽�
        //monster.atkRange = ���������� ����(����)
        Collider[] cols = Physics.OverlapSphere(monster.transform.position, monster.atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<ITakedamage>().Takedamage(monster.atkDamage);
            }
        }
        monster.StartCoroutine(monster.AtkCoolTime());
    }
}
