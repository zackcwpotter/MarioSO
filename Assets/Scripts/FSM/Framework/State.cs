using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/State")]
public class State : ScriptableObject
{
    public Action[] setupActions;
    public Action[] actions;
    public EventAction[] eventTriggeredActions;
    public Action[] exitActions;
    public Transition[] transitions;
    public Color sceneGizmoColor = Color.grey;

    public void UpdateState(StateController controller)
    {
        DoActions(controller);
        CheckTransitions(controller);
    }

    public void DoSetupActions(StateController controller)
    {
        RunActions(setupActions, controller);
    }

    public void DoActions(StateController controller)
    {
        RunActions(actions, controller);
    }

    public void DoExitActions(StateController controller)
    {
        RunActions(exitActions, controller);
    }

    public void DoEventTriggeredActions(StateController controller, ActionType type)
    {
        if (eventTriggeredActions == null) return;
        foreach (EventAction eventAction in eventTriggeredActions)
        {
            if (eventAction.type == type && eventAction.action != null)
                eventAction.action.Act(controller);
        }
    }

    private void RunActions(Action[] list, StateController controller)
    {
        if (list == null) return;
        foreach (Action action in list)
        {
            if (action != null) action.Act(controller);
        }
    }

    // Checks transitions in order and stops at the first one that changes state
    private void CheckTransitions(StateController controller)
    {
        if (transitions == null) return;
        foreach (Transition transition in transitions)
        {
            if (transition.decision == null) continue;

            bool decisionSucceeded = transition.decision.Decide(controller);
            controller.TransitionToState(decisionSucceeded ? transition.trueState : transition.falseState);

            if (controller.transitionStateChanged) break;
        }
    }
}
