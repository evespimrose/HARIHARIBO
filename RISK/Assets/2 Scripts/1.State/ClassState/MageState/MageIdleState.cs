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
        if (dungeonUI == null) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            handler.ChangeState(typeof(MageAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W) && !dungeonUI.IsSkillInCooldown(0))
        {
            handler.ChangeState(typeof(MageWSkill));
            dungeonUI.StartPCCooldown(0);
        }
        else if (Input.GetKeyDown(KeyCode.E) && !dungeonUI.IsSkillInCooldown(1))
        {
            handler.ChangeState(typeof(MageESkill));
            dungeonUI.StartPCCooldown(1);
        }
        else if (Input.GetKeyDown(KeyCode.R) && !dungeonUI.IsSkillInCooldown(2))
        {
            handler.ChangeState(typeof(MageRSkill));
            dungeonUI.StartPCCooldown(2);
        }
        else if (Input.GetKeyDown(KeyCode.T) && !dungeonUI.IsSkillInCooldown(3))
        {
            handler.ChangeState(typeof(MageTSkill));
            dungeonUI.StartPCCooldown(3);
        }
    }
}
