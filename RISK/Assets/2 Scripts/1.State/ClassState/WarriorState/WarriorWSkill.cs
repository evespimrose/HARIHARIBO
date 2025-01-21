using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorWSkill : BaseState<Player>
{
    private float skillDuration = 1.4f;
    private float skillTimer;
    public WarriorWSkill(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        player.Animator?.SetTrigger("WSkill");
    }
    public override void Update(Player player)
    {
        skillTimer -= Time.deltaTime;

        if (skillTimer <= 0)
        {
            Vector3 moveInput = player.GetMove();
            if (moveInput != Vector3.zero)
            {
                handler.ChangeState(typeof(WarriorMoveState));
            }
            else
            {
                handler.ChangeState(typeof(WarriorIdleState));
            }
        }
    }
    public override void Exit(Player player)
    {
        player.SetSkillInProgress(false);
    }

}
