using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour , ITakedamage
{
    [SerializeField] protected float maxHp;
    [SerializeField] public float curHp;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float atkDamage;
    [SerializeField] protected float atkSpeed;

    protected IState<BaseCharacter> currentState;
    public float MaxHp => maxHp;
    public float MoveSpeed => moveSpeed;
    public float AtkDamage => atkDamage;
    public float AtkSpeed => atkSpeed;
    public Animator animator { get; private set; }
    public Rigidbody rb { get; private set; }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        InitializeState();
    }

    protected abstract void InitializeState();

    protected virtual void Update()
    {
        currentState?.Update(this);
    }

    public virtual void ChangeState<TState>(TState newState) where TState : IState<BaseCharacter>
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public virtual void Takedamage(float damage)
    {
        curHp -= damage;
        if (curHp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
