using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorTSkill : BaseState<Player>
{
    private float skillDuration = 3.4f;
    private float skillTimer;
    private bool effectPlayed = false;
    public WarriorTSkill(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        effectPlayed = false;
        player.Animator?.SetTrigger("TSkill");
    }
    public override void Update(Player player)
    {
        skillTimer -= Time.deltaTime;

        if (!effectPlayed && skillTimer <= skillDuration * 0.5f)
        {
            var effectHandler = player.GetComponent<AnimationEventEffects>();
            if (effectHandler != null)
            {
                effectHandler.PlayEffect(3); // T 스킬 이펙트
            }
            effectPlayed = true;
        }

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
