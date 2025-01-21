using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerAttackState : BaseState<Player>
{

    private float[] attackDurations = new float[] { 1.1f, 1.3f, 1.4f, 2f };
    private float attackTimer;
    private static int comboCount = 0;  // 콤보 카운트
    private bool isNextAttackReady = false;

    public DestroyerAttackState(StateHandler<Player> handler) : base(handler) { }

    public override void Enter(Player player)
    {
        if (!player.photonView.IsMine) return;

        // 콤보 카운트 증가
        comboCount++;
        if (comboCount > 4) comboCount = 1;

        // 공격 시작
        attackTimer = attackDurations[comboCount - 1];
        player.Animator?.SetTrigger($"Attack{comboCount}");
        Debug.Log($"Attack {comboCount} Started");

        isNextAttackReady = false;

        //player.photonView?.RPC("SyncAttackState", RpcTarget.Others, player, inputCount);


    }

    public override void Update(Player player)
    {
        if (!player.photonView.IsMine) return;

        attackTimer -= Time.deltaTime;

        // 공격 키 입력 체크
        if (Input.GetKeyDown(KeyCode.A))
        {
            isNextAttackReady = true;
        }

        // 현재 공격 종료 체크
        if (attackTimer <= 0)
        {
            if (isNextAttackReady && comboCount < 4)
            {
                // 다음 콤보로
                handler.ChangeState(typeof(DestroyerAttackState));
            }
            else
            {
                // 콤보 종료
                comboCount = 0;
                Vector3 moveInput = player.GetMove();
                handler.ChangeState(moveInput != Vector3.zero ?
                    typeof(DestroyerMoveState) : typeof(DestroyerIdleState));
            }
        }
    }
    public override void Exit(Player player)
    {
        if (!isNextAttackReady)
        {
            comboCount = 0;
        }
    }

    //[PunRPC]
    //public void SyncAttackState(Player player, int attackIndex)
    //{
    //    player.Animator?.SetTrigger($"Attack{attackIndex}");
    //}
}



