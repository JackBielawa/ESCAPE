using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 12f; // Speed of the fireball
    public float lifetime = 5f; // Time before the fireball is destroyed

    private Rigidbody2D rb;

    void Start()
    {
        // Destroy the fireball after its lifetime
        Destroy(gameObject, lifetime);

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Set the fireball's velocity
        if (rb != null)
        {
            rb.velocity = transform.right * speed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Ghost ghost = other.GetComponent<Ghost>(); // Check if the collided object is a Ghost
        if (ghost != null)
        {
            ghost.DestroySelf(); // Destroy the ghost
            Destroy(gameObject); // Destroy the fireball
            Debug.Log("[Fireball] Fireball destroyed the ghost.");
        }
    }
}
