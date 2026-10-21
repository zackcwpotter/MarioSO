using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour, IPowerupApplicable
{
    // Inspector Variables
    public GameConstants gameConstants;
    public Transform gameCamera;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioSource marioDeathAudio;

    // UnityEvents to hook up to ScriptableObject Game Events in the Inspector
    public UnityEvent onGameOver;
    public UnityEvent onPlayerDeath;

    private float speed;
    private float maxSpeed;
    private float upSpeed;
    private bool moving = false;
    int collisionLayerMask = (1 << 6) | (1 << 7) | (1 << 8);
    private Rigidbody2D marioBody;
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Vector3 startPosition;
    private bool jumpedState = false;
    
    [HideInInspector]
    public float deathImpulse;

    [System.NonSerialized]
    public bool alive = true;

    void Start()
    {
        speed = gameConstants.speed;
        maxSpeed = gameConstants.maxSpeed;
        upSpeed = gameConstants.upSpeed;
        deathImpulse = gameConstants.deathImpulse;

        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        startPosition = transform.position;

        marioAnimator.SetBool("onGround", onGroundState);
    }

    void Update()
    {
        if (alive)
            marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    void FlipMarioSprite(int value)
    {
        if (!alive || Time.timeScale == 0f) return;

        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f) marioAnimator.SetTrigger("onSkid");
        }
        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f) marioAnimator.SetTrigger("onSkid");
        }
    }

    void FixedUpdate()
    {
        if (alive && moving) Move(faceRightState == true ? 1 : -1);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if ((collisionLayerMask & (1 << col.gameObject.layer)) > 0)
        {
            onGroundState = true;
            marioAnimator.SetBool("onGround", true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // We can leave this empty or use it for other triggers, 
        // because the Goomba now handles detecting the hit and triggering DamageMario!
    }

    // Called by the On Damage Player event; Mario's state machine decides what the hit does
    public void DamageMario()
    {
        if (!alive) return;

        MarioStateController stateController = GetComponent<MarioStateController>();
        if (stateController != null)
            stateController.SetPowerup(PowerupType.Damage);
        else
            Die();
    }

    // Called by the DeadMario state (KillMario action)
    public void Die()
    {
        if (!alive) return;

        // Disable collider so he falls through the map
        GetComponent<Collider2D>().enabled = false;
        
        // Play death animation and sound
        marioAnimator.Play("mario-die");
        marioDeathAudio.Play();
        alive = false;

        // Trigger the UnityEvent to raise the Player Death ScriptableObject Event
        onPlayerDeath.Invoke();
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void GameOverScene()
    {
        // Trigger the UnityEvent to raise the Game Over ScriptableObject Event
        onGameOver.Invoke();
    }

    void PlayJumpSound()
    {
        if (marioBody.linearVelocity.y > 0.1f) marioAudio.PlayOneShot(marioAudio.clip);
    }

    public void Jump()
    {
        if (Time.timeScale == 0f) return;
        if (alive && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;
        }
    }

    void Move(int value)
    {
        Vector2 movement = new Vector2(value, 0);
        if (marioBody.linearVelocity.magnitude < maxSpeed) marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        if (value == 0) moving = false;
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    public void GameRestart()
    {
        GetComponent<Collider2D>().enabled = true;
        marioBody.transform.position = startPosition;
        marioBody.linearVelocity = Vector2.zero;
        faceRightState = true;
        marioSprite.flipX = false;
        marioAnimator.Play("mario-idle", 0, 0f);
        onGroundState = true;
        jumpedState = false;
        moving = false;
        marioAnimator.SetBool("onGround", true);
        alive = true;
        transform.localScale = Vector3.one;
    }

    // --- POWERUP INTERFACE IMPLEMENTATION ---

    public void RequestPowerupEffect(IPowerup i)
    {
        // Passes Mario (this) back to the powerup script so it can apply its effect
        i.ApplyPowerup(this);
    }
}
