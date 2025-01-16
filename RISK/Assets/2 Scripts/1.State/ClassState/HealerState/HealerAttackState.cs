using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerAttackState : BaseState<Player>
{
    private float attackDuration = 1.2f;
    private float attackTimer;
    private float comboWindow = 0.5f;
    private float comboTimer;
    private float lastKeyPressTime;
    private int inputCount = 0;
    public HealerAttackState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        attackTimer = attackDuration;


        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }

        inputCount++;
        lastKeyPressTime = Time.time;

        int attackIndex = Mathf.Clamp(inputCount, 1, 3);
        player.Animator?.SetTrigger($"Attack{attackIndex}");
    }

    public override void Update(Player player)
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            if (Time.time - lastKeyPressTime > comboWindow)
            {
                inputCount = 0;
            }

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
        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }

    }

}
