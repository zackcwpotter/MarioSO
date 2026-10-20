using UnityEngine;

public class MushroomCollisionRelay : MonoBehaviour
{
    private MagicMushroomPowerup parentPowerup;

    void Awake()
    {
        FindParentPowerup();
    }

    void OnEnable()
    {
        FindParentPowerup();
    }

    private void FindParentPowerup()
    {
        if (parentPowerup == null)
            parentPowerup = GetComponentInParent<MagicMushroomPowerup>();
        if (parentPowerup == null)
            parentPowerup = GetComponent<MagicMushroomPowerup>();
        if (parentPowerup == null)
            parentPowerup = FindFirstObjectByType<MagicMushroomPowerup>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        FindParentPowerup();
        if (parentPowerup != null)
        {
            parentPowerup.OnCustomCollisionEnter2D(col);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        FindParentPowerup();
        if (parentPowerup != null)
        {
            parentPowerup.OnCustomCollisionEnter2D(col);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        FindParentPowerup();
        if (parentPowerup != null)
        {
            parentPowerup.HandleTrigger(other);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        FindParentPowerup();
        if (parentPowerup != null)
        {
            parentPowerup.HandleTrigger(other);
        }
    }

    // Also forwards the Animation Event if triggered on child Animator
    public void TriggerSpawn()
    {
        FindParentPowerup();
        if (parentPowerup != null)
        {
            parentPowerup.TriggerSpawn();
        }
    }
}

