using System.Collections;
using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Ghost : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveRange = 3f; // Range of random floating movement
        public float moveSpeed = 1f; // Speed at which the ghost moves

        [Header("Player References")]
        public Transform playerCharacterOne; // Reference to Player 1
        public Transform playerCharacterTwo; // Reference to Player 2

        [Header("Fireball Settings")]
        public float fireballImmunityDuration = 0.1f; // Temporary immunity duration after spawning

        private Vector3 startPosition; // Initial position of the ghost
        private GameState gameState; // Reference to the GameState script
        private Rigidbody2D rigidBody; // Rigidbody2D for movement
        private BoxCollider2D boxCollider; // BoxCollider2D for collisions
        private Vector3 currentTargetPosition; // Current target position for floating

        private bool isImmuneToFireball = false; // Whether the ghost is immune to fireball

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
        }

        void Start()
        {
            // Initialize variables
            startPosition = transform.position;
            gameState = FindObjectOfType<GameState>();

            // Configure Rigidbody and Collider
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            rigidBody.gravityScale = 0;
            boxCollider.isTrigger = false;

            // Start floating and temporary fireball immunity
            StartCoroutine(FloatRandomly());
            StartCoroutine(TemporaryImmunityToFireball());
        }

        void FixedUpdate()
        {
            // Move towards the current target position
            if (currentTargetPosition != Vector3.zero)
            {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, currentTargetPosition, moveSpeed * Time.fixedDeltaTime);
                rigidBody.MovePosition(newPosition);

                // Reset target if close enough
                if (Vector3.Distance(transform.position, currentTargetPosition) < 0.1f)
                {
                    currentTargetPosition = Vector3.zero;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerTwo"))
            {
                // Trigger game over if colliding with a player
                Debug.Log("[Ghost] Collided with Player. Triggering Game Over.");
                gameState?.TriggerGameOver();
            }

            if (collision.gameObject.CompareTag("Fireball") && !isImmuneToFireball)
            {
                // Destroy ghost if hit by a fireball and not immune
                DestroySelf();
            }
        }

        public void DestroySelf()
        {
            Debug.Log("[Ghost] Ghost destroyed.");
            Destroy(gameObject);
        }

        private IEnumerator FloatRandomly()
        {
            while (true)
            {
                // Generate a random target position within the movement range
                Vector3 randomOffset = new Vector3(
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange),
                    0f
                );

                currentTargetPosition = startPosition + randomOffset;

                // Wait before selecting a new target
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
}
