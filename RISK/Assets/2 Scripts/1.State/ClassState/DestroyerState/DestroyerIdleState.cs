using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerIdleState : BaseState<Player>
{
    public DestroyerIdleState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        player.Animator?.SetTrigger("Idle");
    }

    public override void Update(Player player)
    {
        Vector3 moveInput = player.GetMove();

        if (moveInput != Vector3.zero)
        {
            handler.ChangeState(typeof(DestroyerMoveState));
            return;
        }


        if (Input.GetKeyDown(KeyCode.A))
        {
            handler.ChangeState(typeof(DestroyerAttackState));
        }
    }
}
