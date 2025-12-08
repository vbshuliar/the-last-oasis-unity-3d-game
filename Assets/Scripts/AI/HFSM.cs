using UnityEngine;

// coordinates hierarchical states with parent child relationships
public class HFSM : MonoBehaviour
{
    private HFSMState currentState;
    private HFSMState previousState;

    // exposes the currently active hierarchical state
    public HFSMState CurrentState => currentState;
    // tracks what state was active previously
    public HFSMState PreviousState => previousState;

    // activates a new state and handles exit enter calls
    public void ChangeState(HFSMState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
            previousState = currentState;
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.Enter();
        }
    }

    // steps the current state each frame
    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    // swaps back to the previously active state
    public void RevertToPreviousState()
    {
        if (previousState != null)
        {
            ChangeState(previousState);
        }
    }
}

