using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f; // Set the speed of the fireball

    private Rigidbody2D rb;

    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle collision with enemies or environment
        // You can add tags or layers to differentiate targets

        // Destroy the fireball upon collision
        Destroy(gameObject);
    }
}
