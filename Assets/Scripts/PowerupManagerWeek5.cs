using UnityEngine;
using UnityEngine.Events;

public class PowerupManagerWeek5 : MonoBehaviour
{
    [Header("Powerup Event Channels")]
    public UnityEvent<IPowerup> powerupAffectsManager;
    public UnityEvent<IPowerup> powerupAffectsPlayer;

    /// <summary>
    /// Receives a collected powerup and routes it to either the manager or player event channel.
    /// </summary>
    public void FilterAndCastPowerup(IPowerup i)
    {
        if (i == null) return;

        // Coins and extra life mushrooms affect manager-level states (score, lives)
        if (i.powerupType == PowerupType.Coin || i.powerupType == PowerupType.OneUpMushroom)
        {
            powerupAffectsManager.Invoke(i);
        }
        else // Magic Mushroom, StarMan, etc. affect Mario/Player directly
        {
            powerupAffectsPlayer.Invoke(i);
        }
    }
}

