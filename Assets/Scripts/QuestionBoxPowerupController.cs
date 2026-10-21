using UnityEngine;

public class QuestionBoxPowerupController : MonoBehaviour, IPowerupController
{
    // Matches the "Is Invisible" slot in your screenshot
    public bool isInvisible; 
    public Animator powerupAnimator;
    public BasePowerup powerup;
    public AudioClip coinSound;

    private AudioSource audioSource;
    private bool isUsed = false;
    private Vector3 originalLocalPos;
    private RigidbodyType2D originalBodyType = RigidbodyType2D.Kinematic;
    private Vector3 powerupLocalPos;
    private Vector3 animatorLocalPos;

    void Awake()
    {
        EnsureLocalReferences();
    }

    private void EnsureLocalReferences()
    {
        if (powerupAnimator == null || !powerupAnimator.gameObject.scene.IsValid())
        {
            if (transform.parent != null)
            {
                Animator[] anims = transform.parent.GetComponentsInChildren<Animator>(true);
                foreach (var a in anims)
                {
                    if (a.gameObject != this.gameObject) 
                    {
                        powerupAnimator = a;
                        break;
                    }
                }
            }
        }

        if (powerupAnimator != null)
        {
            BasePowerup animPowerup = powerupAnimator.GetComponent<BasePowerup>();
            if (animPowerup != null)
            {
                powerup = animPowerup;
            }
        }

        if (powerup == null || !powerup.gameObject.scene.IsValid())
        {
            if (transform.parent != null)
            {
                powerup = transform.parent.GetComponentInChildren<BasePowerup>(true);
            }
            else
            {
                powerup = GetComponentInChildren<BasePowerup>(true);
            }
        }

        if (powerup != null && transform.parent != null)
        {
            CoinPowerup[] allCoins = transform.parent.GetComponentsInChildren<CoinPowerup>(true);
            foreach (var c in allCoins)
            {
                if (c != powerup)
                {
                    c.enabled = false;
                }
            }
        }
    }

    void Start()
    {
        EnsureLocalReferences();
        audioSource = GetComponent<AudioSource>();

        originalLocalPos = transform.localPosition;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            originalBodyType = rb.bodyType;
        }

        if (powerup != null)
        {
            powerupLocalPos = powerup.transform.localPosition;
        }
        if (powerupAnimator != null)
        {
            animatorLocalPos = powerupAnimator.transform.localPosition;
            SpriteRenderer sr = powerupAnimator.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = false;
            }
        }
    }
    
    public void GameRestart()
    {
        StopAllCoroutines();
        isUsed = false;

        Animator boxAnim = GetComponent<Animator>();
        if (boxAnim != null)
        {
            boxAnim.Rebind();
            boxAnim.Update(0f);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = originalBodyType;
            rb.linearVelocity = Vector2.zero;
        }
        transform.localPosition = originalLocalPos;

        if (powerup != null)
        {
            powerup.gameObject.SetActive(true);
            powerup.transform.localPosition = powerupLocalPos;
            powerup.spawned = false;

            if (powerup is MagicMushroomPowerup mushroom)
            {
                mushroom.ResetPowerup(); 
                // NOTE: If you also renamed ResetPowerup to GameRestart in MagicMushroomPowerup, 
                // change the line above to: mushroom.GameRestart();
            }
            else if (powerup is CoinPowerup coin)
            {
                // CHANGED: This now correctly calls GameRestart() instead of ResetPowerup()
                coin.GameRestart();
            }
            else if (powerup is CollectiblePowerup collectible)
            {
                collectible.GameRestart();
            }
            else
            {
                Rigidbody2D pRb = powerup.GetComponent<Rigidbody2D>();
                if (pRb != null)
                {
                    pRb.bodyType = RigidbodyType2D.Kinematic;
                    pRb.linearVelocity = Vector2.zero;
                }
                Collider2D pCol = powerup.GetComponent<Collider2D>();
                if (pCol != null)
                {
                    pCol.enabled = false;
                }
            }
        }

        if (powerupAnimator != null)
        {
            powerupAnimator.gameObject.SetActive(true);
            powerupAnimator.transform.localPosition = animatorLocalPos;
            powerupAnimator.enabled = true; 
            powerupAnimator.Rebind();       
            powerupAnimator.Update(0f);

            Rigidbody2D aRb = powerupAnimator.GetComponent<Rigidbody2D>();
            if (aRb != null)
            {
                aRb.bodyType = RigidbodyType2D.Kinematic;
                aRb.linearVelocity = Vector2.zero;
            }
            Collider2D aCol = powerupAnimator.GetComponent<Collider2D>();
            if (aCol != null)
            {
                aCol.enabled = false;
            }
            SpriteRenderer aSr = powerupAnimator.GetComponent<SpriteRenderer>();
            if (aSr != null)
            {
                aSr.enabled = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isUsed && HitFromBelow(other))
        {
            isUsed = true;

            Animator boxAnim = GetComponent<Animator>();
            if (boxAnim != null)
            {
                boxAnim.SetTrigger("spawned");
            }

            if (powerupAnimator != null)
            {
                SpriteRenderer sr = powerupAnimator.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.enabled = true;
                powerupAnimator.SetTrigger("spawned");
            }

            if (powerup is MagicMushroomPowerup)
            {
                StartCoroutine(SpawnPowerupRoutine(0.75f));
            }
            else if (powerup != null)
            {
                powerup.SpawnPowerup();
            }

            AudioClip clipToPlay = coinSound != null ? coinSound : (audioSource != null ? audioSource.clip : null);
            if (clipToPlay != null && audioSource != null)
            {
                audioSource.PlayOneShot(clipToPlay);
            }
        }
    }

    private bool HitFromBelow(Collision2D other)
    {
        foreach (ContactPoint2D contact in other.contacts)
        {
            if (contact.normal.y > 0.5f)
                return true;
        }
        return false;
    }

    private System.Collections.IEnumerator SpawnPowerupRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (powerup != null && !powerup.hasSpawned)
        {
            powerup.SpawnPowerup();
        }
    }

    public void Disable()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
        transform.localPosition = originalLocalPos;
    }
}