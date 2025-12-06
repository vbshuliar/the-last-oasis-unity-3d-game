using UnityEngine;

public class FSM : MonoBehaviour
{
    private FSMState currentState;
    private FSMState previousState;

    public FSMState CurrentState => currentState;
    public FSMState PreviousState => previousState;

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

    void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public void RevertToPreviousState()
    {
        if (previousState != null)
        {
            ChangeState(previousState);
        }
    }
}

