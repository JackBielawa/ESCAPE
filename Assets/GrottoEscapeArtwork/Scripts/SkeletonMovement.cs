using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.Platformer
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class SkeletonBehavior : MonoBehaviour
    {
        private Animator animator;
        private Rigidbody2D rb;
        private BoxCollider2D boxCollider;
        public int speed = 5;

        [Header("Movement Settings")]
        public float moveSpeed = 2f;
        public LayerMask groundLayer;
        public float groundCheckDistance = 0.1f;

        private bool isMovingRight = true;
        private bool isDead = false;

        private Vector2 initialPosition; // To store the skeleton's starting position

        void Awake()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            initialPosition = transform.position; // Save the initial position
        }

        void Update()
        {
            // Prevent unintended position resets by locking the Y-axis
            if ((Vector2)transform.position != initialPosition)
            {
                LockPosition();
            }

            if (isDead)
            {
                animator.SetBool("Dead", true);
                rb.velocity = Vector2.zero;
                return;
            }
            else
            {
                animator.SetBool("Dead", false);
            }

            // Handle movement
            Move();

            // Animation updates
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }

        private void Move()
        {
            // Check for obstacles or edges to turn around
            bool shouldTurn = CheckForObstacleOrEdge();

            if (shouldTurn)
            {
                isMovingRight = !isMovingRight; // Flip direction
                Flip();
            }

            // Apply movement
            float moveDirection = isMovingRight ? 1f : -1f;
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        }

        private bool CheckForObstacleOrEdge()
        {
            // Check for ground in front of the skeleton
            Vector2 position = transform.position;
            Vector2 direction = isMovingRight ? Vector2.right : Vector2.left;

            // Raycast for obstacle detection
            RaycastHit2D obstacleHit = Physics2D.Raycast(position, direction, boxCollider.bounds.extents.x + 0.1f, groundLayer);

            // Raycast for edge detection
            Vector2 groundCheckPosition = position + (isMovingRight ? Vector2.right : Vector2.left) * boxCollider.bounds.extents.x;
            RaycastHit2D groundHit = Physics2D.Raycast(groundCheckPosition, Vector2.down, groundCheckDistance, groundLayer);

            return obstacleHit.collider != null || groundHit.collider == null;
        }

        private void Flip()
        {
            // Flip the skeleton's direction
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        private void LockPosition()
        {
            // Lock position to avoid teleportation
            Vector3 currentPosition = transform.position;
            transform.position = new Vector3(currentPosition.x, initialPosition.y, currentPosition.z);
        }

        public void Kill()
        {
            // Trigger death animation
            isDead = true;
            animator.SetTrigger("Death");
            Destroy(gameObject, .1f); // Optional: Destroy the skeleton after 1.5 seconds
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            // Example: Detect collision with a "Player" tag to trigger death
            if (collision.collider.CompareTag("Player"))
            {
                Kill();
            }
        }
    }
}
