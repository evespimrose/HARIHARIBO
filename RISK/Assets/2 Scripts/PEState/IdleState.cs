using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState <T> : BaseState<T> where T : BaseCharacter
{
    public override void Enter(T entity)
    {
        entity.animator?.SetBool("IsMoving", false);
    }
    public override void Update(T entity)
    {
        
    }
}
