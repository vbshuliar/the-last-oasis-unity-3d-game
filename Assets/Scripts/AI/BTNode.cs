using UnityEngine;

// indicates the outcome of a behaviour tree node evaluation
public enum BTNodeStatus
{
    Success,
    Failure,
    Running
}

// base type for nodes that behaviour trees can execute
public abstract class BTNode
{
    protected string name;
    protected BTNodeStatus status;

    // captures the node name and sets default status
    public BTNode(string name)
    {
        this.name = name;
        status = BTNodeStatus.Failure;
    }

    // derived nodes implement their own logic here
    public abstract BTNodeStatus Evaluate();
    // resets the node state to a default failure
    public virtual void Reset() { status = BTNodeStatus.Failure; }
}

