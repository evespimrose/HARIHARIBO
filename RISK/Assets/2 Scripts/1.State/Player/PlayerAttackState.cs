//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerAttackState : BaseState<Player>
//{
//    private float attackDuration = 0.5f;
//    private float attackTimer;

//    public PlayerAttackState(StateHandler<Player> handler) : base(handler) { }

//    public override void Enter(Player player)
//    {
//        attackTimer = attackDuration;
//        player.Animator?.SetTrigger("Attack");
//        player.WeaponController?.Attack();
//    }

//    public override void Update(Player player)
//    {
//        attackTimer -= Time.deltaTime;

//        if (attackTimer <= 0)
//        {
//            // �̵� �Է��� ������ �̵� ���·�, ������ ��� ���·�
//            Vector3 moveInput = player.GetMove();
//            if (moveInput != Vector3.zero)
//            {
//                handler.ChangeState(typeof(PlayerMoveState));
//            }
//            else
//            {
//                handler.ChangeState(typeof(PlayerIdleState));
//            }
//        }
//    }
//}
