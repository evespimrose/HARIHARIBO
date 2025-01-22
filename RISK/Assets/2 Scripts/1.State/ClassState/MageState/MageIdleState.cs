using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageIdleState : BaseState<Player>
{
    public MageIdleState(StateHandler<Player> handler) : base(handler) { }

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
            handler.ChangeState(typeof(MageMoveState));
            return;
        }
        var dungeonUI = GameObject.FindObjectOfType<DungeonUIController>();

        if (Input.GetKeyDown(KeyCode.A))
        {
            handler.ChangeState(typeof(MageAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            handler.ChangeState(typeof(MageWSkill));
            dungeonUI?.StartPCCooldown(0);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            handler.ChangeState(typeof(MageESkill));
            dungeonUI?.StartPCCooldown(1);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            handler.ChangeState(typeof(MageRSkill));
            dungeonUI?.StartPCCooldown(2);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            handler.ChangeState(typeof(MageTSkill));
            dungeonUI?.StartPCCooldown(3);
        }
    }
}
