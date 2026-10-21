using UnityEngine;

// Makes Mario blink (and ignore damage) for as long as he stays in this state
[CreateAssetMenu(menuName = "PluggableSM/Actions/SetupInvincibility")]
public class InvincibleAction : Action
{
    public AudioClip invincibilityStart;

    public override void Act(StateController controller)
    {
        MarioStateController m = controller as MarioStateController;
        if (m == null) return;

        if (invincibilityStart != null && m.player.marioAudio != null && !controller.isInitialSetup)
            m.player.marioAudio.PlayOneShot(invincibilityStart);
        m.SetRendererToFlicker();
    }
}
