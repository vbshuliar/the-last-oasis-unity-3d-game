using UnityEngine;

public abstract class FSMState
{
    protected GameObject gameObject;
    protected Transform transform;
    protected FSM fsm;

    public FSMState(GameObject gameObject, FSM fsm)
    {
        this.gameObject = gameObject;
        this.transform = gameObject.transform;
        this.fsm = fsm;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}

