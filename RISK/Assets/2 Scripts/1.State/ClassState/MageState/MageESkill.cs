using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageESkill : BaseState<Player>
{
    private float skillDuration = 1f;
    private float skillTimer;
    public MageESkill(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        player.Animator?.SetTrigger("ESkill");
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

    [PunRPC]
    public void SyncAttackState(Player player)
    {
        player.Animator?.SetTrigger($"ESkill");
    }
}
