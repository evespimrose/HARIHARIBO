using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttackState : BaseState<Player>
{
    private float attackDuration = 1f;
    private float attackTimer;
    private float comboWindow = 0.5f;
    private float comboTimer;
    private float lastKeyPressTime;
    private int inputCount = 0;

    public PlayerAttackState(StateHandler<Player> handler) : base(handler) { }

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

        player.photonView.RPC("SyncAttackState", RpcTarget.Others, player.photonView.ViewID);
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
                handler.ChangeState(typeof(PlayerMoveState));
            }
            else
            {
                handler.ChangeState(typeof(PlayerIdleState));
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

    [PunRPC]
    public void SyncAttackState(int playerId)
    {
        Player player = PhotonView.Find(playerId).GetComponent<Player>();
        int attackIndex = Mathf.Clamp(inputCount, 1, 3);
        player.Animator?.SetTrigger($"Attack{attackIndex}");
    }
}
