using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESkillState : BaseState<Player>
{
    private float skillDuration = 2f;
    private float skillTimer;
    private float moveSpeed = 1f;
    private float maxDistance = 1f;
    private float movedDistance = 0f;
    public ESkillState(StateHandler<Player> handler) : base(handler) { }
    public override void Enter(Player player)
    {
        skillTimer = skillDuration;
        movedDistance = 0f;
        player.Animator?.SetTrigger("ESkill");
    }
    public override void Update(Player player)
    {
        skillTimer -= Time.deltaTime;

        if (movedDistance < maxDistance)
        {
            float movement = moveSpeed * Time.deltaTime;
            player.transform.position += player.transform.forward * movement;
            movedDistance += movement;
        }


        if (skillTimer <= 0)
        {
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
        player.SetSkillInProgress(false);
    }

}
