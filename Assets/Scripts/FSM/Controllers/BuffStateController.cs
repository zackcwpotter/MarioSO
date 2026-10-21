using System.Collections;
using UnityEngine;

// Second, independent state machine on Mario that only handles buffs (e.g. Starman)
public class BuffStateController : PowerupStateController
{
    [Tooltip("Separate AudioSource on Mario used for buff music")]
    public AudioSource buffAudio;

    [HideInInspector] public MarioStateController marioController;

    public bool IsInvincible { get; private set; }

    void Awake()
    {
        marioController = GetComponent<MarioStateController>();
    }

    public void StartStarman(AudioClip music)
    {
        IsInvincible = true;
        if (buffAudio != null && music != null)
        {
            buffAudio.clip = music;
            buffAudio.loop = true;
            buffAudio.Play();
        }
        StopAllCoroutines();
        StartCoroutine(RainbowSprite());
    }

    public void ExitStarman()
    {
        IsInvincible = false;
        StopAllCoroutines();
        if (buffAudio != null) buffAudio.Stop();
        if (marioController != null)
            marioController.spriteRenderer.color = marioController.baseTint;
    }

    private IEnumerator RainbowSprite()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float hue = 0f;
        while (true)
        {
            hue = (hue + Time.deltaTime * 4f) % 1f;
            sprite.color = Color.HSVToRGB(hue, 0.6f, 1f);
            yield return null;
        }
    }
}
