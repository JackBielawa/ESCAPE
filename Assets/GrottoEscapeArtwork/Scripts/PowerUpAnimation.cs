using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpAnimation : MonoBehaviour
{
    public float bobSpeed = 2f; // Speed of the bobbing motion
    public float bobHeight = 0.5f; // Height of the bobbing motion

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial position of the power-up
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new Y position using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        // Update the position of the power-up
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
