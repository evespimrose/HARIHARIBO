using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerAttackState : BaseState<Player>
{

    private float attackDuration = 2.3f;
    private float attackTimer;
    private float comboWindow = 0.5f;
    private float lastKeyPressTime;
    private static int inputCount = 0;

    public DestroyerAttackState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {

        attackTimer = attackDuration;

        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }

        inputCount++;
        lastKeyPressTime = Time.time;

        int attackIndex = Mathf.Clamp(inputCount, 1, 4);  // Destroyer는 4단 콤보
        Debug.Log($"Attack {attackIndex}");
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
        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }
    }
}



