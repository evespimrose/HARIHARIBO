using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageWSkill : BaseState<Player>
{
    private float skillDuration = 1.8f;
    private float skillTimer;
    public MageWSkill(StateHandler<Player> handler) : base(handler) { }

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
                handler.ChangeState(typeof(MageMoveState));
            }
            else
            {
                handler.ChangeState(typeof(MageIdleState));
            }
        }
    }
    public override void Exit(Player player)
    {
        player.SetSkillInProgress(false);
    }
}
