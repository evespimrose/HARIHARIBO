using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class MageAttackState : BaseState<Player>
{
    private float[] attackDurations = new float[] { 1f, 0.7f, 0.9f };
    private float attackTimer;
    private float comboWindow = 0.8f;
    private float lastKeyPressTime;
    private static int inputCount = 0;
    private bool canReceiveInput = true;

    public MageAttackState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }

        inputCount = Mathf.Min(inputCount + 1, 3);

        attackTimer = attackDurations[inputCount - 1];

        lastKeyPressTime = Time.time;
        canReceiveInput = true;

        Debug.Log($"Attack {inputCount} Duration: {attackTimer}");
        player.Animator?.SetTrigger($"Attack{inputCount}");
        //player.photonView.RPC("SyncAttackState", RpcTarget.Others, player, inputCount);
    }

    public override void Update(Player player)
    {
        attackTimer -= Time.deltaTime;

        float currentAttackDuration = attackDurations[inputCount - 1];
        if (canReceiveInput && attackTimer <= currentAttackDuration * 0.7f)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (Time.time - lastKeyPressTime <= comboWindow && inputCount < 3)
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
        if (player.Animator != null)
        {
            for (int i = 1; i <= 3; i++)
            {
                player.Animator.ResetTrigger($"Attack{i}");
            }
        }

        if (Time.time - lastKeyPressTime > comboWindow)
        {
            inputCount = 0;
        }

    }

    //[PunRPC]
    //public void SyncAttackState(Player player, int attackIndex)
    //{
    //    player.Animator?.SetTrigger($"Attack{attackIndex}");
    //}
}
