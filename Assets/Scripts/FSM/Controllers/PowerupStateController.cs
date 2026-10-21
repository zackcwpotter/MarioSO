using UnityEngine;

// A state machine that reacts to collected powerups (used by both Mario's FSM and the buff FSM)
public abstract class PowerupStateController : StateController
{
    public PowerupType currentPowerupType = PowerupType.Default;

    public virtual void SetPowerup(PowerupType type)
    {
        currentPowerupType = type;
    }

    public override void GameRestart()
    {
        currentPowerupType = PowerupType.Default;
        base.GameRestart();
    }
}
