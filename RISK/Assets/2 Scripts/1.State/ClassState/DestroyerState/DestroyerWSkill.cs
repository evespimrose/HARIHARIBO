using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerWSkill : BaseState<Player>
{
    private float skillDuration = 1.2f;
    private float skillTimer;
    private bool effectPlayed = false;
    public DestroyerWSkill(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        effectPlayed = false;
        player.Animator?.SetTrigger("WSkill");
    }
    public override void Update(Player player)
    {
        skillTimer -= Time.deltaTime;

        if (!effectPlayed && skillTimer <= skillDuration * 0.5f)
        {
            var effectHandler = player.GetComponent<AnimationEventEffects>();
            if (effectHandler != null)
            {
                effectHandler.PlayEffect(0); // W Ω∫≈≥ ¿Ã∆Â∆Æ
            }
            effectPlayed = true;
        }

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
