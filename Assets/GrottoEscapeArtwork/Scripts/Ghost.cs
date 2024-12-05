using System.Collections;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float moveRange = 3f; // Range of movement in each direction
    public float moveSpeed = 1f; // Speed of floating
    public Transform PlayerCharacterOne; // Reference to the first player
    public Transform PlayerCharacterTwo; // Reference to the second player

    private Vector3 startPosition;
    private GameState gameState; // Reference to the GameState script

    private bool isImmuneToFireball = false; // Flag to prevent premature destruction by fireball
    public float fireballImmunityDuration = 0.1f; // Duration of immunity to fireball

    void Start()
    {
        startPosition = transform.position;
        gameState = FindObjectOfType<GameState>(); // Find the GameState in the scene
        StartCoroutine(FloatRandomly());

        // Start the temporary immunity to fireball
        StartCoroutine(TemporaryImmunityToFireball());
    }

    void Update()
    {
        // Check proximity to players
        if (PlayerCharacterOne != null && Vector3.Distance(transform.position, PlayerCharacterOne.position) < 0.5f)
        {
            TriggerGameOver();
        }

        if (PlayerCharacterTwo != null && Vector3.Distance(transform.position, PlayerCharacterTwo.position) < 0.5f)
        {
            TriggerGameOver();
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
            // Move ghost randomly within the specified range
            Vector3 randomOffset = new Vector3(
                Random.Range(-moveRange, moveRange),
                Random.Range(-moveRange, moveRange),
                0f
            );

            Vector3 targetPosition = startPosition + randomOffset;

            // Move towards the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(1f); // Pause before the next movement
        }
    }

    private IEnumerator TemporaryImmunityToFireball()
    {
        isImmuneToFireball = true; // Enable immunity
        Debug.Log("Ghost is immune to fireball for a short time.");
        yield return new WaitForSeconds(fireballImmunityDuration);
        isImmuneToFireball = false; // Disable immunity
        Debug.Log("Ghost is now vulnerable to fireball.");
    }
}
