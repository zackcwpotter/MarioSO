using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    private AudioSource music;

    void Start()
    {
        music = GetComponent<AudioSource>();
    }

    // Play the background music from the beginning on restart
    public void RestartMusic()
    {
        music.Stop();
        music.Play();
    }
}