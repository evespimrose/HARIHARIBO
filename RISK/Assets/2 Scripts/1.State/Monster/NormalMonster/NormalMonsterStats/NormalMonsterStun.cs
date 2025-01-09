using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NormalMonster;

public class NormalMonsterStun : BaseState<NormalMonster>
{
    public NormalMonsterStun(StateHandler<NormalMonster> handler) : base(handler) { }

    public float stunTime = 5f;
    private float curTime = 0;

    public override void Enter(NormalMonster monster)
    {
        Debug.Log("��������");
        monster.isStunAction = true;
        //monster.animator.SetBool("Stun", true);
        curTime = 0;
    }

    public override void Update(NormalMonster monster)
    {
        if (monster.isAirborne == true)
        {
            Debug.Log("���ϵ��߿� ������ͼ� ����ĵ��");
            monster.nMHandler.ChangeState(typeof(NormalMonsterAirborne));
        }
        if (curTime >= stunTime - 0.1f)
        {
            monster.nMHandler.ChangeState(typeof(NormalMonsterIdle));
        }
        curTime += Time.deltaTime;
    }

    public override void Exit(NormalMonster monster)
    {
        monster.isStunAction = false;
        monster.isStun = false;
        Debug.Log("��������");
        //monster.animator.SetBool("Stun", false);
    }
}
