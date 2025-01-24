using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerIdleState : BaseState<Player>
{
    public DestroyerIdleState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        player.Animator?.SetTrigger("Idle");
    }

    public override void Update(Player player)
    {
        Vector3 moveInput = player.GetMove();

        if (moveInput != Vector3.zero)
        {
            handler.ChangeState(typeof(DestroyerMoveState));
            return;
        }

        var dungeonUI = GameObject.FindObjectOfType<DungeonUIController>();
        if (dungeonUI == null) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            handler.ChangeState(typeof(DestroyerAttackState));
        }
        else if (Input.GetKeyDown(KeyCode.W) && !dungeonUI.IsSkillInCooldown(0))
        {
            Debug.Log("GetKeyDown : W");
            handler.ChangeState(typeof(DestroyerWSkill));
            dungeonUI.StartPCCooldown(0);
        }
        else if (Input.GetKeyDown(KeyCode.E) && !dungeonUI.IsSkillInCooldown(1))
        {
            Debug.Log("GetKeyDown : E");
            handler.ChangeState(typeof(DestroyerESkill));
            dungeonUI.StartPCCooldown(1);
        }
        else if (Input.GetKeyDown(KeyCode.R) && !dungeonUI.IsSkillInCooldown(2))
        {
            Debug.Log("GetKeyDown : R");
            handler.ChangeState(typeof(DestroyerRSkill));
            dungeonUI.StartPCCooldown(2);
        }
        else if (Input.GetKeyDown(KeyCode.T) && !dungeonUI.IsSkillInCooldown(3))
        {
            Debug.Log("GetKeyDown : T");
            handler.ChangeState(typeof(DestroyerTSkill));
            dungeonUI.StartPCCooldown(3);
        }
        if (PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.I))
        {
            UnitManager.Instance.DoomToMonsters();
        }
    }

}
