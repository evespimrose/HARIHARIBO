using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState<T> where T : BaseCharacter
{
    void Enter(T entity);
    void Update(T entity);
    void Exit(T entity);
}
