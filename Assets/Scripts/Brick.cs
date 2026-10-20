using System.Collections;
using UnityEngine;
using UnityEngine.Events; // ADDED: Required for UnityEvents

[RequireComponent(typeof(AudioSource))]
public class Brick : MonoBehaviour
{
    public bool hasCoin = false;
    public GameObject coin; 
    public AudioClip coinSound; 

    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.15f;

    // ADDED: Replaces the old GameManager score increase
    public UnityEvent<int> increaseScore;

    private Vector3 startPosition;
    private bool isUsed = false;
    private bool isBouncing = false;
    private AudioSource audioSource; 

    void Start()
    {
        startPosition = transform.localPosition;
        audioSource = GetComponent<AudioSource>(); 
        
        if (coin != null) 
        {
            coin.SetActive(false);
        }

        // DELETED: GameManager.instance.gameRestart.AddListener(ResetBrick);
        // We will now use a SimpleGameEventListener in the Inspector instead!
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Make sure Mario is hitting it from below
            if (contact.normal.y > 0.5f && !isUsed && !isBouncing)
            {
                isUsed = true;
                StartCoroutine(Bounce());
                
                if (hasCoin) 
                {
                    SpawnCoin();
                }
                
                break;
            }
        }
    }

    private IEnumerator Bounce()
    {
        isBouncing = true;
        Vector3 topPosition = startPosition + Vector3.up * bounceHeight;

        float time = 0f;
        while (time < bounceDuration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(startPosition, topPosition, time / bounceDuration);
            yield return null;
        }

        time = 0f;
        while (time < bounceDuration)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(topPosition, startPosition, time / bounceDuration);
            yield return null;
        }

        transform.localPosition = startPosition;
        isBouncing = false;
    }

    private void SpawnCoin()
    {
        if (coinSound != null)
        {
            audioSource.PlayOneShot(coinSound);
        }

        // ADDED: Broadcast the score increase to the Event System!
        increaseScore.Invoke(1);

        if (coin != null)
        {
            coin.SetActive(true);
            StartCoroutine(MoveCoin(coin));
        }
    }

    private IEnumerator MoveCoin(GameObject coinObj)
    {
        Vector3 startCoinPos = coinObj.transform.position;
        Vector3 topPosition = startCoinPos + Vector3.up * 1.5f;

        float duration = 0.3f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            coinObj.transform.position = Vector3.Lerp(startCoinPos, topPosition, time / duration);
            yield return null;
        }

        time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            coinObj.transform.position = Vector3.Lerp(topPosition, startCoinPos, time / duration);
            yield return null;
        }

        coinObj.SetActive(false);
    }

    // Must be public so the SimpleGameEventListener can trigger it
    public void ResetBrick()
    {
        isUsed = false;
        isBouncing = false;
        transform.localPosition = startPosition;
        
        if (coin != null) 
        {
            coin.SetActive(false);
        }
    }
}