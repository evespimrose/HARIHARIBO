using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerRSkill : BaseState<Player>
{
    private float skillDuration = 1.2f;
    private float skillTimer;
    private bool effectPlayed = false;
    public HealerRSkill(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        effectPlayed = false;
        player.Animator?.SetTrigger("RSkill");
    }

    public override void Update(Player player)
    {
        skillTimer -= Time.deltaTime;

        if (!effectPlayed && skillTimer <= skillDuration * 0.5f)
        {
            var effectHandler = player.GetComponent<AnimationEventEffects>();
            if (effectHandler != null)
            {
                effectHandler.PlayEffect(2); // R Ω∫≈≥ ¿Ã∆Â∆Æ
            }
            effectPlayed = true;
        }

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
