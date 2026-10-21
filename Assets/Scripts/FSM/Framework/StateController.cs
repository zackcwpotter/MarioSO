using UnityEngine;

// Base class for every pluggable state machine.
// Subclasses override the virtual methods and call base where needed.
public abstract class StateController : MonoBehaviour
{
    public State startState;
    public State previousState;
    public State currentState;
    public State remainState;

    public bool transitionStateChanged = false;
    [HideInInspector] public float stateTimeElapsed;

    private bool isActive = true;

    // True while entering a state at level start or restart (not through a transition).
    // Actions can use this to skip sounds and effects.
    [HideInInspector] public bool isInitialSetup = false;

    // The state this level started in; restarting the level goes back to it
    protected State levelStartState;

    public virtual void Start()
    {
        currentState = InitialState();
        levelStartState = currentState;
        isInitialSetup = true;
        OnSetupState();
        isInitialSetup = false;
    }

    // Override to start in another state than startState (e.g. one carried over from the previous level)
    protected virtual State InitialState()
    {
        return startState;
    }

    // Runs the setup actions of the state we just entered
    public virtual void OnSetupState()
    {
        stateTimeElapsed = 0;
        if (currentState != null) currentState.DoSetupActions(this);
    }

    // Runs the exit actions of the state we are leaving
    public virtual void OnExitState()
    {
        if (currentState != null) currentState.DoExitActions(this);
    }

    public virtual void OnDrawGizmos()
    {
        if (currentState != null)
        {
            Gizmos.color = currentState.sceneGizmoColor;
            Gizmos.DrawWireSphere(transform.position, 0.6f);
        }
    }

    public void TransitionToState(State nextState)
    {
        if (nextState == null || nextState == remainState) return;

        OnExitState();
        previousState = currentState;
        currentState = nextState;
        transitionStateChanged = true;
        OnSetupState();
    }

    public bool CheckIfCountDownElapsed(float duration)
    {
        return stateTimeElapsed >= duration;
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }

    // Puts the state machine back into the state the level started in
    public virtual void GameRestart()
    {
        OnExitState();
        previousState = null;
        currentState = levelStartState != null ? levelStartState : startState;
        transitionStateChanged = false;
        isActive = true;
        isInitialSetup = true;
        OnSetupState();
        isInitialSetup = false;
    }

    public virtual void Update()
    {
        if (!isActive || currentState == null) return;

        stateTimeElapsed += Time.deltaTime;
        transitionStateChanged = false;
        currentState.UpdateState(this);
    }
}
