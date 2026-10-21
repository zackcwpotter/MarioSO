using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class MarioStateController : PowerupStateController
{
    [Header("Carry state between levels")]
    [Tooltip("Remembers Mario's state so the next level can start in it")]
    public StateVariable savedState;
    [Tooltip("Tick in the first level only. Later levels start in the saved state from the previous level.")]
    public bool resetSavedStateOnStart = true;

    [HideInInspector] public PlayerController player;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public BuffStateController buffController;

    // Tint set by the current Mario state; the starman buff restores this when it ends
    [HideInInspector] public Color baseTint = Color.white;

    // Size set by the current Mario state. Applied in LateUpdate because the idle animation also animates the scale.
    [HideInInspector] public Vector3 marioScale = Vector3.one;

    private bool flickering = false;

    // Mario can't be hurt while flickering after a hit, or while the starman buff is active
    public bool IsInvincible
    {
        get { return flickering || (buffController != null && buffController.IsInvincible); }
    }

    void Awake()
    {
        player = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        buffController = GetComponent<BuffStateController>();
    }

    protected override State InitialState()
    {
        if (savedState != null && !resetSavedStateOnStart && savedState.Value != null)
            return savedState.Value;
        return startState;
    }

    public override void OnSetupState()
    {
        base.OnSetupState();
        if (savedState != null) savedState.Value = currentState;
    }

    public override void Update()
    {
        base.Update();

        if (Keyboard.current != null && Keyboard.current.zKey.wasPressedThisFrame)
            Fire();
    }

    void LateUpdate()
    {
        transform.localScale = marioScale;
    }

    public override void SetPowerup(PowerupType type)
    {
        if (type == PowerupType.Damage && IsInvincible) return;
        base.SetPowerup(type);
    }

    // Triggers the attack event actions of the current state (only FireMario has one)
    public void Fire()
    {
        if (Time.timeScale == 0f || !player.alive || currentState == null) return;
        currentState.DoEventTriggeredActions(this, ActionType.Attack);
    }

    public override void GameRestart()
    {
        StopAllCoroutines();
        flickering = false;
        spriteRenderer.enabled = true;
        base.GameRestart();
    }

    public void SetRendererToFlicker()
    {
        StartCoroutine(BlinkSpriteRenderer(currentState));
    }

    // Blinks Mario until we leave the state that started the blinking
    private IEnumerator BlinkSpriteRenderer(State blinkingState)
    {
        flickering = true;
        while (currentState == blinkingState)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.075f);
        }
        spriteRenderer.enabled = true;
        flickering = false;
    }
}
