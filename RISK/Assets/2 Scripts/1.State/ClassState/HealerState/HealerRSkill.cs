using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerRSkill : BaseState<Player>
{
    private float skillDuration = 2.1f;
    private float skillTimer;
    public HealerRSkill(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        player.Animator?.SetTrigger("RSkill");
    }

    public override void Update(Player player)
    {
        skillTimer -= Time.deltaTime;

        if (skillTimer <= 0)
        {
            Vector3 moveInput = player.GetMove();
            if (moveInput != Vector3.zero)
            {
                handler.ChangeState(typeof(HealerMoveState));
            }
            else
            {
                handler.ChangeState(typeof(HealerIdleState));
            }
        }
    }
    public override void Exit(Player player)
    {
        player.SetSkillInProgress(false);
    }
}
