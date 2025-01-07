using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState<T> : BaseState<T> where T : BaseCharacter
{
    protected Vector3 moveDirection;

    public override void Enter(T entity)
    {
        entity.animator?.SetBool("IsMoving", true);
    }
    public override void Update(T entity)
    {       
        if (moveDirection.magnitude > 0)
        {          
            Vector3 movement = moveDirection * entity.MoveSpeed * Time.deltaTime;
            entity.rb.MovePosition(entity.transform.position + movement);

            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            entity.transform.rotation = Quaternion.RotateTowards(
                entity.transform.rotation,
                toRotation,
                720f * Time.deltaTime  
            );
        }
    }
    public override void Exit(T entity)
    {
        entity.animator?.SetBool("IsMoving", false);
    }
}
