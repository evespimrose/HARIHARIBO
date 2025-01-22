using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorIdleState : BaseState<Player>
{
    public WarriorIdleState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        player.Animator?.SetTrigger("Idle");
    }

    public override void Update(Player player)
    {
        Vector3 moveInput = player.GetMove();

        if (moveInput != Vector3.zero)
        {
            handler.ChangeState(typeof(WarriorMoveState));
            return;
        }
        var dungeonUI = GameObject.FindObjectOfType<DungeonUIController>();

        if (Input.GetKeyDown(KeyCode.A))
        {
            handler.ChangeState(typeof(WarriorAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            handler.ChangeState(typeof(WarriorWSkill));
            dungeonUI?.StartPCCooldown(0);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            handler.ChangeState(typeof(WarriorESkill));
            dungeonUI?.StartPCCooldown(1);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            handler.ChangeState(typeof(WarriorRSkill));
            dungeonUI?.StartPCCooldown(2);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            handler.ChangeState(typeof(WarriorTSkill));
            dungeonUI?.StartPCCooldown(3);
        }
    }
}
