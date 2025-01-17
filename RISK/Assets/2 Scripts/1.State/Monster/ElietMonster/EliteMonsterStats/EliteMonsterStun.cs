using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterStun : BaseState<EliteMonster>
{
    public EliteMonsterStun(StateHandler<EliteMonster> handler) : base(handler) { }

    public float stunTime = 5f;

    public override void Enter(EliteMonster monster)
    {
        Debug.Log("스턴진입");
        monster.animator.SetBool("Stun", true);
        monster.StartCoroutine(StartStun(monster));
    }

    public override void Update(EliteMonster monster)
    {
        if (!monster.isStun && !monster.isStunAction)
        {
            monster.eMHandler.ChangeState(typeof(EliteMonsterIdle));
        }
    }

    public override void Exit(EliteMonster monster)
    {
        monster.isStunAction = false;
        monster.isStun = false;
        Debug.Log("스턴종료");
        monster.animator.SetBool("Stun", false);
    }

    private IEnumerator StartStun(EliteMonster monster)
    {
        yield return new WaitForSeconds(stunTime);
        monster.isStun = false;
        monster.isHitAction = false;
    }
}
