using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/ExitStarman")]
public class ExitStarmanAction : Action
{
    public override void Act(StateController controller)
    {
        BuffStateController b = controller as BuffStateController;
        if (b != null) b.ExitStarman();
    }
}
