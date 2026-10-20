using UnityEngine;
using UnityEngine.Events;

public class HomeButtonController : MonoBehaviour, IInteractiveButton
{
    public UnityEvent returnToMenu;

    public void ButtonClick()
    {
        Debug.Log("Onclick home button");
        returnToMenu.Invoke();
    }
}
