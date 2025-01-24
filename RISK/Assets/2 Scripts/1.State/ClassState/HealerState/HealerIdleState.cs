using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerIdleState : BaseState<Player>
{
    public HealerIdleState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        player.Animator?.SetTrigger("Idle");
    }

    public override void Update(Player player)
    {
        Vector3 moveInput = player.GetMove();

        if (moveInput != Vector3.zero)
        {
            handler.ChangeState(typeof(HealerMoveState));
            return;
        }
        var dungeonUI = GameObject.FindObjectOfType<DungeonUIController>();
        if (dungeonUI == null) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            handler.ChangeState(typeof(HealerAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W) && !dungeonUI.IsSkillInCooldown(0))
        {
            handler.ChangeState(typeof(HealerWSkill));
            dungeonUI.StartPCCooldown(0);
        }
        else if (Input.GetKeyDown(KeyCode.E) && !dungeonUI.IsSkillInCooldown(1))
        {
            handler.ChangeState(typeof(HealerESkill));
            dungeonUI.StartPCCooldown(1);
        }
        else if (Input.GetKeyDown(KeyCode.R) && !dungeonUI.IsSkillInCooldown(2))
        {
            handler.ChangeState(typeof(HealerRSkill));
            dungeonUI.StartPCCooldown(2);
        }
        else if (Input.GetKeyDown(KeyCode.T) && !dungeonUI.IsSkillInCooldown(3))
        {
            handler.ChangeState(typeof(HealerTSkill));
            dungeonUI.StartPCCooldown(3);
        }
        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.I))
        {
            UnitManager.Instance.DoomToMonsters();
        }
    }


}
