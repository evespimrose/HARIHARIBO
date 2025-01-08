using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : BaseState<Player>
{
    public PlayerIdleState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        player.Animator?.SetTrigger("Idle");
    }

    public override void Update(Player player)
    {
        Vector3 moveInput = player.GetMove();

        // �̵� �Է��� ������ �̵� ���·� ��ȯ
        if (moveInput != Vector3.zero)
        {
            handler.ChangeState(typeof(PlayerMoveState));
            return;
        }

        // ���� �Է� Ȯ��
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    handler.ChangeState(typeof(PlayerAttackState));
        //}
    }
}
