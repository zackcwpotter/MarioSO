using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio; 
using UnityEngine.SceneManagement;

// CHANGED: Inherits from MonoBehaviour now, NOT Singleton<GameManager>
public class GameManager : MonoBehaviour, IPowerupApplicable
{
    // These three variables match your screenshot exactly
    public IntVariable gameScore;
    public UnityEvent gameStart;
    public UnityEvent updateScore;

    [Header("Audio Snapshots")]
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot sadGameOverSnapshot;

    [Tooltip("Tick in the first level only. Later levels keep the score from the previous level.")]
    public bool resetScoreOnStart = true;

    // Score when this level started; restarting the level goes back to it
    private int scoreAtLevelStart;

    void Start()
    {
        if (resetScoreOnStart) gameScore.Value = 0;
        scoreAtLevelStart = gameScore.Value;
        
        // Broadcasts to the OnGameStart event in the inspector
        gameStart.Invoke(); 
        Time.timeScale = 1.0f;
        AudioListener.pause = false;

        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(0f);
        }

        SceneManager.activeSceneChanged += SceneSetup;
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneSetup;
    }

    public void GameRestart()
    {
        gameScore.Value = scoreAtLevelStart;
        
        // Broadcasts to the OnUpdateScore event in the inspector
        updateScore.Invoke(); 
        Time.timeScale = 1.0f;
        AudioListener.pause = false;

        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(0f);
        }
    }

    public void IncreaseScore(int increment)
    {
        gameScore.ApplyChange(increment);
        gameScore.SaveHighScore();
        
        // Broadcasts to the OnUpdateScore event in the inspector
        updateScore.Invoke(); 
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;

        if (sadGameOverSnapshot != null)
        {
            sadGameOverSnapshot.TransitionTo(0f);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        AudioListener.pause = false;

        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(0f);
        }
    }

    public void ReturnToMenu()
    {
        gameScore.Value = 0;
        Time.timeScale = 1.0f;
        AudioListener.pause = false;

        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(0f);
        }

        SceneManager.LoadScene("Main Menu");
    }

    public void SceneSetup(Scene current, Scene next)
    {
        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(0f);
        }

        gameStart.Invoke();
        updateScore.Invoke();
    }

    public void RequestPowerupEffect(IPowerup i)
    {
        i.ApplyPowerup(this);
    }
}