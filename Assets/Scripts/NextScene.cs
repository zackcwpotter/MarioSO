using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public string nextSceneName;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
        {
            Debug.Log("Change scene to: " + nextSceneName);
            SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        }
    }
}