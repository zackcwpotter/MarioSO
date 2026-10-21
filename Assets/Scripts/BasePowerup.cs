using UnityEngine;

public abstract class BasePowerup : MonoBehaviour, IPowerup
{
    public PowerupType type;
    public bool spawned = false;
    protected bool consumed = false;
    protected bool goRight = true;
    protected Rigidbody2D rigidBody;

    // base methods
    protected virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // interface methods
    // 1. concrete methods
    public PowerupType powerupType
    {
        get
        {
            return type;
        }
    }

    public bool hasSpawned
    {
        get
        {
            return spawned;
        }
    }

    public virtual void DestroyPowerup()
    {
        Destroy(this.gameObject);
    }

    // Lets Mario's state machines (Mario FSM and buff FSM) react to this powerup
    protected void SendToStateControllers(MonoBehaviour target)
    {
        if (target == null) return;
        foreach (PowerupStateController controller in target.GetComponents<PowerupStateController>())
        {
            controller.SetPowerup(powerupType);
        }
    }

    // 2. abstract methods, must be implemented by derived classes
    public abstract void SpawnPowerup();
    public abstract void ApplyPowerup(MonoBehaviour i);
}