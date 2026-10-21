using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// A powerup Mario collects by touching it (e.g. Fire Flower or Starman).
// Needs a Collider2D set to "Is Trigger".
// With hiddenUntilSpawned it stays hidden until a question box calls SpawnPowerup, then rises out of the box.
public class CollectiblePowerup : BasePowerup
{
    public UnityEvent<IPowerup> powerupCollected;
    public AudioClip pickupSound;

    [Header("Question box")]
    public bool hiddenUntilSpawned = false;
    public float riseHeight = 1f;
    public float riseDuration = 0.5f;

    private Vector3 startLocalPosition;

    protected override void Start()
    {
        base.Start();
        startLocalPosition = transform.localPosition;
        ResetToStart();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed || !spawned) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null || !player.alive) return;

        consumed = true;
        if (pickupSound != null && player.marioAudio != null)
            player.marioAudio.PlayOneShot(pickupSound);

        powerupCollected.Invoke(this);
        ApplyPowerup(player);
        DestroyPowerup();
    }

    // Called by the question box when Mario hits it
    public override void SpawnPowerup()
    {
        if (spawned) return;
        spawned = true;
        StartCoroutine(RiseFromBox());
    }

    private IEnumerator RiseFromBox()
    {
        SetSpriteVisible(true);
        SetColliderEnabled(false);

        Vector3 from = startLocalPosition;
        Vector3 to = startLocalPosition + Vector3.up * riseHeight;
        float time = 0f;
        while (time < riseDuration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(from, to, time / riseDuration);
            yield return null;
        }
        transform.localPosition = to;

        SetColliderEnabled(true);
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        SendToStateControllers(i);
    }

    // Hide instead of destroying so it can come back on restart.
    // The object stays active so a restart listener on it keeps working.
    public override void DestroyPowerup()
    {
        SetSpriteVisible(false);
        SetColliderEnabled(false);
    }

    public void GameRestart()
    {
        StopAllCoroutines();
        ResetToStart();
    }

    private void ResetToStart()
    {
        consumed = false;
        transform.localPosition = startLocalPosition;

        bool visible = !hiddenUntilSpawned;
        spawned = visible;
        SetSpriteVisible(visible);
        SetColliderEnabled(visible);
    }

    private void SetSpriteVisible(bool visible)
    {
        foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>(true))
            sprite.enabled = visible;
    }

    private void SetColliderEnabled(bool enabled)
    {
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>(true))
            col.enabled = enabled;
    }
}
