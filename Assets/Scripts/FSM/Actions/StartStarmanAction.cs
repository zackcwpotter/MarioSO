using UnityEngine;

[CreateAssetMenu(menuName = "PluggableSM/Actions/StartStarman")]
public class StartStarmanAction : Action
{
    public AudioClip starmanMusic;

    public override void Act(StateController controller)
    {
        BuffStateController b = controller as BuffStateController;
        if (b != null) b.StartStarman(starmanMusic);
    }
}
