using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 12f; // Speed for the fireball
    public float lifetime = 5f; // Time in seconds before the fireball is destroyed

    private Rigidbody2D rb;

    void Start()
    {
        // Destroy the fireball after its lifetime expires
        Destroy(gameObject, lifetime);

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Ensure the fireball moves horizontally
        if (rb != null)
        {
            rb.velocity = transform.right * speed; // Set the velocity of the Rigidbody2D
        }
    }

    private void FixedUpdate()
    {
        // Optional: If Rigidbody2D is not used, move the fireball manually
        if (rb == null)
        {
            transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle collision with enemies or environment
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
        {
            // Destroy the fireball upon collision
            Destroy(gameObject);
        }
    }
}
