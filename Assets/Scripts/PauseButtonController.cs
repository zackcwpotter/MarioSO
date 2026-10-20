using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events; // ADDED: Required for UnityEvents

public class PauseButtonController : MonoBehaviour, IInteractiveButton
{
    private bool isPaused = false;
    private Image image;

    // These public variables now perfectly match your screenshot
    public Sprite pauseIcon;
    public Sprite playIcon;
    public GameObject pausePanel; 
    public Button restartButton; 

    // ADDED: UnityEvents to broadcast pause state to the rest of the game
    public UnityEvent gamePaused;
    public UnityEvent gameResumed;

    void Start()
    {
        image = GetComponent<Image>();
        AudioListener.pause = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);
            
        // DELETED: GameManager event subscriptions
    }

    // This must be public so you can hook it up to the OnGameRestart and OnPlayerDeath GameEvents
    public void ResetPause()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        image.sprite = pauseIcon;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void ButtonClick()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0.0f : 1.0f;
        AudioListener.pause = isPaused;

        if (isPaused)
        {
            image.sprite = playIcon;
            if (pausePanel != null) pausePanel.SetActive(true);
            
            // Broadcast that the game is paused
            gamePaused.Invoke();
        }
        else
        {
            image.sprite = pauseIcon;
            if (pausePanel != null) pausePanel.SetActive(false);
            
            // Broadcast that the game has resumed
            gameResumed.Invoke();
        }
    }

    public void OnPlayerDeath()
    {
        if (isPaused)
            ResetPause();
        gameObject.SetActive(false);
    }

    // DELETED: OnDestroy() method (no longer needed since GameManager is gone)
}