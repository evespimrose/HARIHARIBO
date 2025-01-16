using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerESkill : BaseState<Player>
{
    private float skillDuration = 2.2f;
    private float skillTimer;
    private bool effectPlayed = false;
    public DestroyerESkill(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        effectPlayed = false;
        player.Animator?.SetTrigger("ESkill");
    }
    public override void Update(Player player)
    {
        skillTimer -= Time.deltaTime;
        if (!effectPlayed && skillTimer <= skillDuration * 0.5f)
        {
            var effectHandler = player.GetComponent<AnimationEventEffects>();
            if (effectHandler != null)
            {
                effectHandler.PlayEffect(1); // E 스킬 이펙트
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

    [PunRPC]
    public void SyncAttackState(Player player)
    {
        player.Animator?.SetTrigger($"ESkill");
    }
}
