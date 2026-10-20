using UnityEngine;
using UnityEngine.Events;

public class CoinPowerup : BasePowerup
{
    private Vector3 startPosition;

    // Broadcast that this powerup was collected, passing itself as IPowerup
    public UnityEvent<IPowerup> powerupCollected;

    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.Coin;
        startPosition = transform.localPosition;
    }

    public override void SpawnPowerup()
    {
        if (spawned) return;
        spawned = true;
        
        // CHANGED: Broadcast that this powerup was collected, passing itself
        powerupCollected.Invoke(this);
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        if (i is GameManager gm)
        {
            gm.IncreaseScore(1);
        }
    }

    // 1. Deactivate instead of deleting
    public override void DestroyPowerup()
    {
        gameObject.SetActive(false);
    }

    // 2. Called on restart
    public void GameRestart()
    {
        spawned = false;
        transform.localPosition = startPosition;
        gameObject.SetActive(true);
        Animator anim = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.Play("idle");
        }
    }
}