using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorESkill : BaseState<Player>
{
    private float skillDuration = 2f;
    private float skillTimer;
    private float moveSpeed = 4f;
    private float maxDistance = 1.5f;
    private float movedDistance = 0f;
    private bool effectPlayed = false;
    public WarriorESkill(StateHandler<Player> handler) : base(handler) { }
    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        movedDistance = 0f;
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
                effectHandler.PlayEffect(1); // E ?ㅽ궗 ?댄럺??
            }
            effectPlayed = true;
        }

        if (movedDistance < maxDistance)
        {
            float movement = moveSpeed * Time.deltaTime;
            player.transform.position += player.transform.forward * movement;
            movedDistance += movement;
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

    [PunRPC]
    public void SyncAttackState(Player player)
    {
        player.Animator?.SetTrigger($"ESkill");
    }
}
