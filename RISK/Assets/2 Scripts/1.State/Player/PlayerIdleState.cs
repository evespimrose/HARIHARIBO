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

        // 이동 입력이 있으면 이동 상태로 전환
        if (moveInput != Vector3.zero)
        {
            handler.ChangeState(typeof(PlayerMoveState));
            return;
        }

        // 공격 입력 확인
        //if (Input.GetButtonDown("Fire1"))
        //{
        //    handler.ChangeState(typeof(PlayerAttackState));
        //}
    }
}
