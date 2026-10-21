using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // ADDED: Required for UnityEvents

[RequireComponent(typeof(AudioSource))]
public class EnemyController : MonoBehaviour
{
    public GameConstants gameConstants;

    // ADDED: UnityEvents to match the screenshot exactly
    public UnityEvent damagePlayer;
    public UnityEvent<int> increaseScore;

    [HideInInspector] public Vector3 startPosition;
    [HideInInspector] public AudioClip stompSound;
    [HideInInspector] public Sprite stompedSprite;

    private float originalX;
    private float maxOffset;
    private float enemyPatroltime;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

    private bool stomped = false;

    void Start()
    {
        maxOffset = gameConstants.goombaMaxOffset;
        enemyPatroltime = gameConstants.goombaPatrolTime;
        
        enemyBody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalSprite = spriteRenderer.sprite;

        startPosition = transform.localPosition;
        originalX = transform.position.x;

        ComputeVelocity();
    }

    void ComputeVelocity()
    {
        velocity = new Vector2(moveRight * maxOffset / enemyPatroltime, 0);
    }

    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }

    void FixedUpdate()
    {
        if (stomped) return;

        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            Movegoomba();
        }
        else
        {
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || stomped)
            return;

        // Starman: touching the enemy from any side defeats it
        BuffStateController buff = other.GetComponent<BuffStateController>();
        if (buff != null && buff.IsInvincible)
        {
            Defeat();
            return;
        }

        Rigidbody2D marioBody = other.GetComponent<Rigidbody2D>();

        bool marioIsAbove = other.transform.position.y > transform.position.y + 0.3f;
        bool marioIsFalling = marioBody != null && marioBody.linearVelocity.y < 0;

        if (marioIsAbove && marioIsFalling)
        {
            Debug.Log("GOOMBA STOMPED!");
            Stomp();

            // CHANGED: Invoke the UnityEvent for score instead of the old delegate
            increaseScore.Invoke(1); 
        }
        else
        {
            // ADDED: If Mario hits from the side, trigger the damage event
            damagePlayer.Invoke();
        }
    }

    // Used when the enemy is killed by something other than a stomp (e.g. a fireball)
    public void Defeat()
    {
        if (stomped) return;
        Stomp();
        increaseScore.Invoke(1);
    }

    private void Stomp()
    {
        if (stomped) return;

        stomped = true;
        velocity = Vector2.zero;

        PlayStompSound();
        spriteRenderer.sprite = stompedSprite;

        StartCoroutine(HideAfterStomp());
    }

    private IEnumerator HideAfterStomp()
    {
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    public void PlayStompSound()
    {
        if (stompSound != null)
        {
            audioSource.PlayOneShot(stompSound);
        }
    }

    public void GameRestart()
    {
        StopAllCoroutines();
        stomped = false;

        spriteRenderer.enabled = true;
        spriteRenderer.sprite = originalSprite;
        GetComponent<Collider2D>().enabled = true;

        transform.localPosition = startPosition;
        originalX = transform.position.x;

        moveRight = -1;
        ComputeVelocity();
    }
}