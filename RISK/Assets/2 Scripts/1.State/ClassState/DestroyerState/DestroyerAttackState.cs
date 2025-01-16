using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerAttackState : BaseState<Player>
{

    private float[] attackDurations = new float[] { 1.1f, 1.3f, 1.4f, 2f };  // 예시 시간
    private float attackTimer;
    private float comboWindow = 0.8f;
    private float lastKeyPressTime;
    private static int inputCount = 0;
    private bool canReceiveInput = true;

    public DestroyerAttackState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {

        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }

        inputCount = Mathf.Min(inputCount + 1, 4);

        // 현재 콤보 단계에 맞는 지속시간 설정
        attackTimer = attackDurations[inputCount - 1];

        lastKeyPressTime = Time.time;
        canReceiveInput = true;

        Debug.Log($"Attack {inputCount} Duration: {attackTimer}");
        player.Animator?.SetTrigger($"Attack{inputCount}");
    }

    public override void Update(Player player)
    {
        attackTimer -= Time.deltaTime;

        float currentAttackDuration = attackDurations[inputCount - 1];
        if (canReceiveInput && attackTimer <= currentAttackDuration * 0.7f)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (Time.time - lastKeyPressTime <= comboWindow && inputCount < 4)
                {
                    handler.ChangeState(typeof(DestroyerAttackState));
                    return;
                }
            }
        }

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
        if (player.Animator != null)
        {
            for (int i = 1; i <= 4; i++)
            {
                player.Animator.ResetTrigger($"Attack{i}");
            }
        }

        canReceiveInput = false;

        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }
    }
}



