using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler<T> where T : class
{
    private T owner;
    private IState<T> currentState;
    private IState<T> previousState;
    private IState<T> globalState;
    private Dictionary<Type, IState<T>> states;

    public StateHandler(T owner)
    {
        this.owner = owner;
        states = new Dictionary<Type, IState<T>>();
    }

    public void RegisterState(IState<T> state)
    {
        var stateType = state.GetType();
        if (!states.ContainsKey(stateType))
        {
            states[stateType] = state;
        }
    }

    public void ChangeState(Type stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            Debug.LogError($"?곹깭 {stateType.Name}???깅줉?섏? ?딆븯?듬땲??");
            return;
        }

        previousState = currentState;
        currentState?.Exit(owner);
        currentState = states[stateType];
        currentState.Enter(owner);

        if (owner is Player player)
        {
            player.photonView.RPC("SyncStateChange", RpcTarget.Others, stateType.Name);
        }
    }

    public void RevertToPreviousState()
    {
        if (previousState != null)
        {
            ChangeState(previousState.GetType());
        }
    }

    public bool IsInState<TState>() where TState : IState<T>
    {
        return currentState?.GetType() == typeof(TState);
    }

    public void SetGlobalState(IState<T> state)
    {
        globalState = state;
    }

    public void Update()
    {
        globalState?.Update(owner);
        currentState?.Update(owner);
    }
}
