using UnityEngine;

// defines a base state that works with the fsm component
public abstract class FSMState
{
    protected GameObject gameObject;
    protected Transform transform;
    protected FSM fsm;

    // stores references needed by derived states
    public FSMState(GameObject gameObject, FSM fsm)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
        this.fsm = fsm;
    }

    // runs when the state becomes active
    public virtual void Enter() { }
    // runs each frame while the state stays active
    public virtual void Update() { }
    // runs before the state hands control to another
    public virtual void Exit() { }
}

