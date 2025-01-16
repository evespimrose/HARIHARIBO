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


        if (Input.GetKeyDown(KeyCode.A))
        {
            handler.ChangeState(typeof(HealerAttackState));
        }

        else if (Input.GetKeyDown(KeyCode.W))
        {
            handler.ChangeState(typeof(HealerWSkill));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            handler.ChangeState(typeof(HealerESkill));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            handler.ChangeState(typeof(HealerRSkill));
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            handler.ChangeState(typeof(HealerTSkill));
        }
    }


}
