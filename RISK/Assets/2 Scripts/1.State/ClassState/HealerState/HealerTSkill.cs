using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerTSkill : BaseState<Player>
{
    private float skillDuration = 2.6f;
    private float skillTimer;
    public HealerTSkill(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        player.Animator?.SetTrigger("TSkill");
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
