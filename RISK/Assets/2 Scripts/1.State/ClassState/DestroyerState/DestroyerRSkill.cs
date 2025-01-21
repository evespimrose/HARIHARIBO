using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerRSkill : BaseState<Player>
{
    private float skillDuration = 2.8f;
    private float skillTimer;
    public DestroyerRSkill(StateHandler<Player> handler) : base(handler) { }

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
                handler.ChangeState(typeof(DestroyerMoveState));
            }
            else
            {
                handler.ChangeState(typeof(DestroyerIdleState));
            }
        }
    }
    public override void Exit(Player player)
    {
        player.SetSkillInProgress(false);
    }
}
