using System.Collections.Generic;
using UnityEngine;

public class HFSMState
{
    protected GameObject gameObject;
    protected Transform transform;
    protected HFSM fsm;
    protected HFSMState parentState;
    protected List<HFSMState> subStates = new List<HFSMState>();
    protected HFSMState currentSubState;

    public HFSMState(GameObject gameObject, HFSM fsm, HFSMState parentState = null)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
        this.fsm = fsm;
        this.parentState = parentState;
    }

    public virtual void Enter() 
    {
        // Enter first sub-state if available
        if (subStates.Count > 0 && currentSubState == null)
        {
            currentSubState = subStates[0];
            currentSubState.Enter();
        }
    }

    public virtual void Update() 
    {
        // Update current sub-state
        if (currentSubState != null)
        {
            currentSubState.Update();
        }
    }

    public virtual void Exit() 
    {
        // Exit current sub-state
        if (currentSubState != null)
        {
            currentSubState.Exit();
            currentSubState = null;
        }
    }

    public void AddSubState(HFSMState subState)
    {
        subStates.Add(subState);
    }

    public void ChangeSubState(HFSMState newSubState)
    {
        if (currentSubState != null)
        {
            currentSubState.Exit();
        }

        currentSubState = newSubState;

        if (currentSubState != null)
        {
            currentSubState.Enter();
        }
    }

    public HFSMState GetCurrentSubState()
    {
        return currentSubState;
    }

    public HFSMState GetSubState(int index)
    {
        if (index >= 0 && index < subStates.Count)
        {
            return subStates[index];
        }
        return null;
    }

    public int GetSubStateCount()
    {
        return subStates.Count;
    }
}

