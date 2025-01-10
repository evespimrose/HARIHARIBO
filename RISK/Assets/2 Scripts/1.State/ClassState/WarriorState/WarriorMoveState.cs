using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorMoveState : BaseState<Player>
{
    public WarriorMoveState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        player.Animator?.SetBool("IsMoving", true);
    }

    public override void Update(Player player)
    {
        Vector3 moveInput = player.GetMove();

        if (moveInput == Vector3.zero)
        {
            handler.ChangeState(typeof(WarriorIdleState));
            return;
        }

        player.Move(moveInput);

        //// �̵� �� ����
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    handler.ChangeState(typeof(PlayerAttackState));
        //}
    }

    public override void Exit(Player player)
    {
        player.Animator?.SetBool("IsMoving", false);
    }
}