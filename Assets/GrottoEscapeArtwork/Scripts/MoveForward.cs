using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardX : MonoBehaviour
{
    public float speed;
    public float lifetime = 5f; // Time in seconds before the fireball is destroyed

    void Start()
    {
        // Destroy the fireball after `lifetime` seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the fireball horizontally along the X-axis
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
