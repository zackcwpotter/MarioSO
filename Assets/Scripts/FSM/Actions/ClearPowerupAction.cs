using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/ClearPowerup")]
public class ClearPowerupAction : Action
{
    public override void Act(StateController controller)
    {
        PowerupStateController c = controller as PowerupStateController;
        if (c != null) c.currentPowerupType = PowerupType.Default;
    }
}
