using UnityEngine;

// Changes how Mario looks (animator, size, colour) when entering a state, and plays a sound
[CreateAssetMenu(menuName = "PluggableSM/Actions/SetupAnimator")]
public class SetupAnimator : Action
{
    [Tooltip("Leave empty to keep the current Animator Controller")]
    public RuntimeAnimatorController animatorController;
    public Vector3 scale = Vector3.one;
    public Color tint = Color.white;
    [Tooltip("Optional sound played when entering the state")]
    public AudioClip enterSound;

    public override void Act(StateController controller)
    {
        MarioStateController m = controller as MarioStateController;
        if (m == null) return;

        if (animatorController != null && m.player.marioAnimator != null)
            m.player.marioAnimator.runtimeAnimatorController = animatorController;

        m.marioScale = scale;
        m.transform.localScale = scale;
        m.baseTint = tint;
        if (m.buffController == null || !m.buffController.IsInvincible)
            m.spriteRenderer.color = tint;

        if (enterSound != null && m.player.marioAudio != null && !controller.isInitialSetup)
            m.player.marioAudio.PlayOneShot(enterSound);
    }
}
