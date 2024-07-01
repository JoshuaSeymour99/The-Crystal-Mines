using UnityEngine;
using System;

//Abstract class to make the blueprint for all the states. 
// the estate is an enum which will help avoid miss typing a value
public abstract class BaseState<Estate> where Estate : Enum
{
    public BaseState(Estate key)
    {
        StateKey = key;
    }


    public Estate StateKey { get; private set; }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract Estate GetNextState();
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerStay(Collider other);
    public abstract void OnTriggerExit(Collider other);
}

