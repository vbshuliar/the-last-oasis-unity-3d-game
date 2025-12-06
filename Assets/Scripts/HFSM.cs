using UnityEngine;

public class HFSM : MonoBehaviour
{
    private HFSMState currentState;
    private HFSMState previousState;

    public HFSMState CurrentState => currentState;
    public HFSMState PreviousState => previousState;

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

