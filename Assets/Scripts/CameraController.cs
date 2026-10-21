using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;   // Mario's Transform
    public Transform endLimit; // Marks the right end of the level
    [Tooltip("Untick to keep the camera still (e.g. for a level that fits on one screen)")]
    public bool followPlayer = true;

    private float offset;            
    private float startX;            
    private float endX;              
    private float viewportHalfWidth; 
    
    // Added to remember the starting position for the restart event
    private Vector3 startPosition; 

    void Start()
    {
        // The tutorial keeps this standard way of finding the player
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        viewportHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        offset = transform.position.x - player.position.x;
        startX = transform.position.x;
        endX = endLimit.position.x - viewportHalfWidth;
        
        // Save the initial position when the game starts
        startPosition = transform.position; 
    }

    void Update()
    {
        if (!followPlayer) return;

        float desiredX = player.position.x + offset;
        float clampedX = Mathf.Clamp(desiredX, startX, endX);

        transform.position = new Vector3(
            clampedX,
            transform.position.y,
            transform.position.z
        );
    }

    // This is the public method the tutorial requires for the event listener
    public void GameRestart()
    {
        transform.position = startPosition;
    }
}