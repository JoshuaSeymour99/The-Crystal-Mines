using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class StateManager<Estate> : MonoBehaviour where Estate : Enum
{
    protected Dictionary<Estate, BaseState<Estate>> States = new Dictionary<Estate, BaseState<Estate>>();

    protected BaseState<Estate> CurrentState;
    protected bool IsTransitioningState = false;

    protected Animator myAnimator; 
    protected Rigidbody2D rb;



    void Start()
    {
        CurrentState.EnterState();
    }

    void Update()
    {
        Estate nextStateKey = CurrentState.GetNextState();
        
        if(!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        } else if (!IsTransitioningState) 
        {
            TransitionToState(nextStateKey);
        }


    }

    public void TransitionToState(Estate stateKey)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }

    void OnTriggerEnter(Collider other){}

    void OnTriggerStay(Collider other){}

    void OnTriggerExit(Collider other){}




}