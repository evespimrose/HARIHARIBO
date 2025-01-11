using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerMoveState : BaseState<Player>
{
    public DestroyerMoveState(StateHandler<Player> handler) : base(handler) { }
    
    public override void Enter(Player player)
    {
        player.Animator?.SetBool("IsMoving", true);
    }

    public override void Update(Player player)
    {
        Vector3 moveInput = player.GetMove();

        if (moveInput == Vector3.zero)
        {
            handler.ChangeState(typeof(DestroyerIdleState));
            return;
        }

        player.Move(moveInput);

        //// 이동 중 공격
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    handler.ChangeState(typeof(DestroyerAttackState));
        //}
    }

    public override void Exit(Player player)
    {
        player.Animator?.SetBool("IsMoving", false);
    }
}
