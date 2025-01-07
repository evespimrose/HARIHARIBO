using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<T> : IState<T> where T : BaseCharacter
{
    public virtual void Enter(T entity) { }
    public virtual void Update(T entity) { }
    public virtual void Exit(T entity) { }
}
