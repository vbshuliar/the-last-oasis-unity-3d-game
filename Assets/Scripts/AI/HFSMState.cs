using System.Collections.Generic;
using UnityEngine;

// represents a hierarchical state that can own substates
public class HFSMState
{
    protected GameObject gameObject;
    protected Transform transform;
    protected HFSM fsm;
    protected HFSMState parentState;
    protected List<HFSMState> subStates = new List<HFSMState>();
    protected HFSMState currentSubState;

    // stores references and optional parent pointers
    public HFSMState(GameObject gameObject, HFSM fsm, HFSMState parentState = null)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
        this.fsm = fsm;
        this.parentState = parentState;
    }

    // activates this state and ensures first substate runs
    public virtual void Enter()
    {
        if (subStates.Count > 0 && currentSubState == null)
        {
            currentSubState = subStates[0];
            currentSubState.Enter();
        }
    }

    // updates the active substate if one exists
    public virtual void Update()
    {
        if (currentSubState != null)
        {
            currentSubState.Update();
        }
    }

    // exits the current substate before leaving
    public virtual void Exit()
    {
        if (currentSubState != null)
        {
            currentSubState.Exit();
            currentSubState = null;
        }
    }

    // registers a child state for this state
    public void AddSubState(HFSMState subState)
    {
        subStates.Add(subState);
    }

    // hands control to another substate
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

    // returns reference to current substate
    public HFSMState GetCurrentSubState()
    {
        return currentSubState;
    }

    // retrieves a substate by index if available
    public HFSMState GetSubState(int index)
    {
        if (index >= 0 && index < subStates.Count)
        {
            return subStates[index];
        }
        return null;
    }

    // reports how many substates belong to this state
    public int GetSubStateCount()
    {
        return subStates.Count;
    }
}

