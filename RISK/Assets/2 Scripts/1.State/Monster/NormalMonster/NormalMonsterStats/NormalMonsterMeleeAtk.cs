using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalMonsterMeleeAtk : BaseState<NormalMonster>
{
    public NormalMonsterMeleeAtk(StateHandler<NormalMonster> handler) : base(handler) { }


    public float atkDuration = 5f;
    public float atkDelay = 0.4f;
    private float curTime = 0;
    private bool isAtk = false;
    public override void Enter(NormalMonster entity)
    {
        Debug.Log("MeleeAtk진입");
        entity.animator.SetTrigger("Atk");
        curTime = 0;
        isAtk = false;
    }

    public override void Update(NormalMonster entity)
    {
        if (atkDuration - curTime < 0.1f)
        {
            //공격종료
            entity.nMHandler.ChangeState(typeof(NormalMonsterIdle));
        }
        if (atkDelay - curTime < 0.1f && isAtk == false)
        {
            Atk(entity);
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(NormalMonster entity)
    {
    }

    private void Atk(NormalMonster entity)
    {
        Debug.Log("MeleeAtk공격");
        entity.isAtk = true;
        isAtk = true;
        Collider[] cols = Physics.OverlapSphere(entity.transform.position, entity.atkRange);
        foreach (Collider col in cols)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponent<ITakedamage>().Takedamage(entity.atkDamage);
            }
        }
        entity.StartCoroutine(entity.AtkCoolTime());
    }
}
