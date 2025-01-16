using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerAttackState : BaseState<Player>
{

    private float[] attackDurations = new float[] { 1.1f, 1.3f, 1.4f, 2f };  // 예시 시간
    private float attackTimer;
    private float comboWindow = 1.2f;
    private float lastKeyPressTime;
    private int inputCount = 0;
    private bool canReceiveInput = true;
    private bool hasNextInput = false;  // 다음 입력 저장
    private float minAnimationPlayTime = 0.7f;  // 최소 애니메이션 재생 시간


    public DestroyerAttackState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        if (player?.Animator == null) return;

        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }

        inputCount = Mathf.Min(inputCount + 1, 4);

        // 현재 콤보 단계에 맞는 지속시간 설정
        attackTimer = attackDurations[inputCount - 1];
        hasNextInput = false;

        Debug.Log($"Attack {inputCount} Duration: {attackTimer}");
        player.Animator?.SetTrigger($"Attack{inputCount}");

        player.photonView?.RPC("SyncAttackState", RpcTarget.Others, player, inputCount);

        lastKeyPressTime = Time.time;
        canReceiveInput = true;
    }

    public override void Update(Player player)
    {
        if (player?.Animator == null) return;

        attackTimer -= Time.deltaTime;

        float currentAttackDuration = attackDurations[inputCount - 1];
        float normalizedTime = 1 - (attackTimer / currentAttackDuration);

        if (canReceiveInput && normalizedTime >= minAnimationPlayTime)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                lastKeyPressTime = Time.time;
                if (inputCount < 4)
                {
                    hasNextInput = true;  // 다음 입력 저장
                }
            }
        }

        // 현재 애니메이션이 충분히 재생된 후에만 다음 콤보로 전환
        if (hasNextInput && normalizedTime >= 0.7f)
        {
            handler.ChangeState(typeof(DestroyerAttackState));
            return;
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
        hasNextInput = false;

        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }
    }

    [PunRPC]
    public void SyncAttackState(Player player, int attackIndex)
    {
        player.Animator?.SetTrigger($"Attack{attackIndex}");
    }
}



