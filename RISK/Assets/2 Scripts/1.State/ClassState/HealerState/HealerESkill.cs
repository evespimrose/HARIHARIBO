using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerESkill : BaseState<Player>
{
    private float skillDuration = 2.3f;
    private float skillTimer;
    public HealerESkill(StateHandler<Player> handler) : base(handler) { }

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

    [PunRPC]
    public void SyncAttackState(Player player)
    {
        player.Animator?.SetTrigger($"ESkill");
    }
}
