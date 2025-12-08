using UnityEngine;

// manages transitions between finite state instances
public class FSM : MonoBehaviour
{
    private FSMState currentState;
    private FSMState previousState;

    // returns the state that is currently active
    public FSMState CurrentState => currentState;
    // returns the state that was active before the current one
    public FSMState PreviousState => previousState;

    // switches to a new state and runs exit and enter hooks
    public void ChangeState(FSMState newState)
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

    // ticks the active state every frame
    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    // returns to the previously active state if possible
    public void RevertToPreviousState()
    {
        if (previousState != null)
        {
            ChangeState(previousState);
        }
    }
}

