using UnityEngine;

// True when the controller has collected one of the listed powerups
[CreateAssetMenu(menuName = "PluggableSM/Decisions/Transform")]
public class TransformDecision : Decision
{
    public PowerupType[] powerupsCollected;

    public override bool Decide(StateController controller)
    {
        PowerupStateController c = controller as PowerupStateController;
        if (c == null || powerupsCollected == null) return false;

        foreach (PowerupType type in powerupsCollected)
        {
            if (c.currentPowerupType == type) return true;
        }
        return false;
    }
}
