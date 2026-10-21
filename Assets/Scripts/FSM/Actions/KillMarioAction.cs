using UnityEngine;

// Plays Mario's death animation and sound, which ends in game over
[CreateAssetMenu(menuName = "PluggableSM/Actions/KillMario")]
public class KillMarioAction : Action
{
    public override void Act(StateController controller)
    {
        MarioStateController m = controller as MarioStateController;
        if (m != null) m.player.Die();
    }
}
