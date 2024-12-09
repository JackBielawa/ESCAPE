using System.Collections;
using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Bat : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveRange = 3f;
        public float moveSpeed = 1f;

        [Header("Shooting Settings")]
        public GameObject fireballPrefab;
        public float fireballSpeed = 8f;  // Increased speed
        public float initialShootDelay = 0.1f;  // Very small initial delay
        public float shootInterval = 0.5f;  // Decreased interval
        public float shootRange = 20f;  // Increased range

        private Vector3 startPosition;
        private GameState gameState;
        private Rigidbody2D rigidBody;
        private BoxCollider2D boxCollider;
        private Vector3 currentTargetPosition;

        void Start()
        {
            startPosition = transform.position;
            gameState = FindObjectOfType<GameState>();

            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            rigidBody.gravityScale = 0;

            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.isTrigger = false;

            StartCoroutine(FloatRandomly());
            StartCoroutine(ShootFireballs());
        }

        void FixedUpdate()
        {
            // Move towards current target position
            if (currentTargetPosition != Vector3.zero)
            {
                Vector3 newPosition = Vector3.MoveTowards(transform.position, currentTargetPosition, moveSpeed * Time.fixedDeltaTime);
                rigidBody.MovePosition(newPosition);

                if (Vector3.Distance(transform.position, currentTargetPosition) < 0.1f)
                {
                    currentTargetPosition = Vector3.zero;
                }
            }
        }

        private void ShootFireball()
        {
            GameObject player1 = GameObject.FindWithTag("Player");
            GameObject player2 = GameObject.FindWithTag("PlayerTwo");

            if (fireballPrefab != null)
            {
                // Shoot at Player One if exists
                if (player1 != null)
                {
                    LaunchFireballAtTarget(player1.transform.position);
                }

                // Shoot at Player Two if exists
                if (player2 != null)
                {
                    LaunchFireballAtTarget(player2.transform.position);
                }
            }
        }

        private void LaunchFireballAtTarget(Vector3 targetPosition)
        {
            Vector2 direction = (targetPosition - transform.position).normalized;
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

            Rigidbody2D fireballRb = fireball.GetComponent<Rigidbody2D>();
            if (fireballRb)
            {
                fireballRb.velocity = direction * fireballSpeed;
            }
            fireball.tag = "Fireball";
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerTwo"))
            {
                gameState?.TriggerGameOver();
            }

            if (collision.gameObject.CompareTag("Fireball"))
            {
                Destroy(gameObject);
            }
        }

        private IEnumerator FloatRandomly()
        {
            while (true)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-moveRange, moveRange),
                    Random.Range(-moveRange, moveRange),
                    0f
                );

                currentTargetPosition = startPosition + randomOffset;
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator ShootFireballs()
        {
            // Initial delay before starting to shoot
            yield return new WaitForSeconds(initialShootDelay);

            while (true)
            {
                ShootFireball();
                yield return new WaitForSeconds(shootInterval);
            }
        }
    }
}