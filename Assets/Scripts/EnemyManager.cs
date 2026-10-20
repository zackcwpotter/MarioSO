using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Required for UnityEvents

public class EnemyManager : MonoBehaviour
{
    // ADDED: Replaces the GameManager reference for increasing the score
    public UnityEvent<int> increaseScore;

    void Start()
    {
        // Subscribe to each Goomba's stomp event
        foreach (Transform child in transform)
        {
            EnemyController enemy = child.GetComponent<EnemyController>();

            if (enemy != null)
            {
                // You can safely uncomment this whenever you are ready
                // enemy.OnGoombaStomp += HandleGoombaStomp;
            }
        }
    }

    private void HandleGoombaStomp()
    {
        Debug.Log("EnemyManager received stomp event");

        // CHANGED: Broadcasts the score increase to the Event System
        increaseScore.Invoke(1);
    }

    public void GameRestart()
    {
        foreach (Transform child in transform)
        {
            EnemyController enemy = child.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.GameRestart();
            }
        }
    }
    
    // DELETED: OnDestroy() is no longer needed since we aren't listening to the old GameManager
}