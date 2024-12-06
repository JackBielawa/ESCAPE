using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Ghost : MonoBehaviour
{
    public float moveRange = 3f; // Range of movement in each direction
    public float moveSpeed = 1f; // Speed of floating
    public Transform PlayerCharacterOne; // Reference to the first player
    public Transform PlayerCharacterTwo; // Reference to the second player

    private Vector3 startPosition;
    private GameState gameState; // Reference to the GameState script
    private Rigidbody2D rigid;
    private BoxCollider2D boxCollider;

    private bool isImmuneToFireball = false; // Flag to prevent premature destruction by fireball
    public float fireballImmunityDuration = 0.1f; // Duration of immunity to fireball
    private Vector3 currentTargetPosition; // Current target position for floating

    void Start()
    {
        startPosition = transform.position;
        gameState = FindObjectOfType<GameState>(); // Find the GameState in the scene
        rigid = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Configure Rigidbody2D for interactions
        rigid.bodyType = RigidbodyType2D.Kinematic; // Kinematic avoids physical reactions
        rigid.gravityScale = 0; // Disable gravity

        // Ensure the ghost has a collider
        boxCollider.isTrigger = false;

        // Start floating and immunity logic
        StartCoroutine(FloatRandomly());
        StartCoroutine(TemporaryImmunityToFireball());
    }

    private void FixedUpdate()
    {
        // Move the ghost towards the current target position
        if (currentTargetPosition != Vector3.zero)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, currentTargetPosition, moveSpeed * Time.fixedDeltaTime);
            rigid.MovePosition(newPosition);

            // If the ghost reaches the target position, reset it for the next move
            if (Vector3.Distance(transform.position, currentTargetPosition) < 0.1f)
            {
                currentTargetPosition = Vector3.zero;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore Rigidbody-to-Rigidbody interactions and keep moving
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("[Ghost] Collided with PlayerCharacterOne. Triggering Game Over.");
            TriggerGameOver();
        }
        else if (collision.gameObject.CompareTag("PlayerTwo"))
        {
            Debug.Log("[Ghost] Collided with PlayerCharacterTwo. Triggering Game Over.");
            TriggerGameOver();
        }

        // Interaction with fireballs
        if (collision.gameObject.CompareTag("Fireball") && !isImmuneToFireball)
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
        Debug.Log("[Ghost] Ghost destroyed.");
    }

    private void TriggerGameOver()
    {
        gameState?.TriggerGameOver(); // Notify GameState of game over
    }

    private IEnumerator FloatRandomly()
    {
        while (true)
        {
            // Select a random position within the specified range
            Vector3 randomOffset = new Vector3(
                Random.Range(-moveRange, moveRange),
                Random.Range(-moveRange, moveRange),
                0f
            );

            // Set the target position for the ghost to move toward
            currentTargetPosition = startPosition + randomOffset;

            // Wait until the target is reached or a short pause
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator TemporaryImmunityToFireball()
    {
        isImmuneToFireball = true; // Enable immunity
        Debug.Log("[Ghost] Immune to fireball for a short time.");
        yield return new WaitForSeconds(fireballImmunityDuration);
        isImmuneToFireball = false; // Disable immunity
        Debug.Log("[Ghost] Vulnerable to fireball.");
    }
}
