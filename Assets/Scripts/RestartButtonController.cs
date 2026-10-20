using UnityEngine;
using UnityEngine.Events; // Add this line to use UnityEvents

public class RestartButtonController : MonoBehaviour, IInteractiveButton
{
    // Create a generic UnityEvent that will show up in the Inspector
    public UnityEvent gameRestart; 

    public void ButtonClick()
    {
        Debug.Log("Onclick restart button");
        
        // Remove GameManager.instance.GameRestart();
        // Invoke the UnityEvent instead
        gameRestart.Invoke(); 
    }
}