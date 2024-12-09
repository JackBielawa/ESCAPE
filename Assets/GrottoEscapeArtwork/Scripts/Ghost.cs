using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Ghost : MonoBehaviour
{
    public float moveRange = 3f;         // Range of random floating movement
    public float moveSpeed = 1f;         // Speed at which the ghost moves
    public Transform PlayerCharacterOne; // Reference to Player 1 (optional if needed)
    public Transform PlayerCharacterTwo; // Reference to Player 2 (optional if needed)

    private Vector3 startPosition;
    private GameState gameState;
    private Rigidbody2D rigid;
    private BoxCollider2D boxCollider;

    private bool isImmuneToFireball = false; // Temporary immunity after spawning
    public float fireballImmunityDuration = 0.1f;
    private Vector3 currentTargetPosition;

    void Start()
    {
        startPosition = transform.position;
        gameState = FindObjectOfType<GameState>();
        rigid = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Rigidbody and Collider settings
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.gravityScale = 0;
        boxCollider.isTrigger = false;

        // Start movement and immunity coroutines
        StartCoroutine(FloatRandomly());
        StartCoroutine(TemporaryImmunityToFireball());
    }

    void FixedUpdate()
    {
        // Move the ghost towards the current target position
        if (currentTargetPosition != Vector3.zero)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, currentTargetPosition, moveSpeed * Time.fixedDeltaTime);
            rigid.MovePosition(newPosition);

            // If close to target, reset
            if (Vector3.Distance(transform.position, currentTargetPosition) < 0.1f)
            {
                currentTargetPosition = Vector3.zero;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If it hits a player, trigger game over
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerTwo"))
        {
            Debug.Log("[Ghost] Collided with Player. Triggering Game Over.");
            gameState?.TriggerGameOver();
        }

        // If hit by a fireball and not immune, destroy the ghost
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

    private IEnumerator FloatRandomly()
    {
        while (true)
        {
            // Pick a random position within the defined range
            Vector3 randomOffset = new Vector3(
                Random.Range(-moveRange, moveRange),
                Random.Range(-moveRange, moveRange),
                0f
            );

            currentTargetPosition = startPosition + randomOffset;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator TemporaryImmunityToFireball()
    {
        isImmuneToFireball = true;
        Debug.Log("[Ghost] Immune to fireball for a short time.");
        yield return new WaitForSeconds(fireballImmunityDuration);
        isImmuneToFireball = false;
        Debug.Log("[Ghost] Vulnerable to fireball.");
    }
}
