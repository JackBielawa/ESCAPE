using System.Collections;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float moveRange = 3f; // Range of movement in each direction
    public float moveSpeed = 1f; // Speed of floating
    public Transform PlayerCharacterOne; // Reference to the first player
    public Transform PlayerCharacterTwo; // Reference to the second player
    public GameObject fireball; // Reference to the fireball prefab or object

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Save the initial position
        StartCoroutine(FloatRandomly());
    }

    void Update()
    {
        // Check for collision with PlayerCharacterOne
        if (PlayerCharacterOne != null && Vector3.Distance(transform.position, PlayerCharacterOne.position) < 0.5f)
        {
            GameOver();
        }

        // Check for collision with PlayerCharacterTwo
        if (PlayerCharacterTwo != null && Vector3.Distance(transform.position, PlayerCharacterTwo.position) < 0.5f)
        {
            GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the ghost collides with the fireball
        if (collision.gameObject.CompareTag("Fire"))
        {
            Destroy(gameObject); // Destroy the ghost
        }
    }

    private void GameOver()
    {
        // Implement game-over logic (e.g., reload scene or stop the game)
        Debug.Log("Game Over! Ghost made contact with a player.");
        Time.timeScale = 0; // Stop the game
    }

    private IEnumerator FloatRandomly()
    {
        while (true)
        {
            // Generate a random target position within the move range
            Vector3 randomOffset = new Vector3(
                Random.Range(-moveRange, moveRange),
                Random.Range(-moveRange, moveRange),
                0f
            );

            Vector3 targetPosition = startPosition + randomOffset;

            // Move the ghost toward the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Wait for a short time before moving again
            yield return new WaitForSeconds(1f);
        }
    }
}
