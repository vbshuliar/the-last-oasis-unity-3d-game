using UnityEngine;

public enum BTNodeStatus
{
    Success,
    Failure,
    Running
}

public abstract class BTNode
{
    protected string name;
    protected BTNodeStatus status;

    public BTNode(string name)
    {
        this.name = name;
        status = BTNodeStatus.Failure;
    }

    public abstract BTNodeStatus Evaluate();
    public virtual void Reset() { status = BTNodeStatus.Failure; }
}

