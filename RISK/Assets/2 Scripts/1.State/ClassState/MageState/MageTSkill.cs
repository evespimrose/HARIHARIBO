using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageTSkill : BaseState<Player>
{
    private float skillDuration = 4.5f;
    private float skillTimer;
    public MageTSkill(StateHandler<Player> handler) : base(handler) { }

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
