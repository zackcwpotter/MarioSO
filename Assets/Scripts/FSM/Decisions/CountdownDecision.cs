using UnityEngine;

// True once the controller has been in the current state for buffDuration seconds
[CreateAssetMenu(menuName = "PluggableSM/Decisions/Countdown")]
public class CountdownDecision : Decision
{
    public float buffDuration = 2f;

    public override bool Decide(StateController controller)
    {
        return controller.CheckIfCountDownElapsed(buffDuration);
    }
}
