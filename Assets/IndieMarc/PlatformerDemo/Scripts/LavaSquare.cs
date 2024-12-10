using UnityEngine;

public class LavaSquare : MonoBehaviour
{
    private GameState gameState;

    void Start()
    {
        // Find the GameState script in the scene
        gameState = FindObjectOfType<GameState>();

        // Log warning if GameState isn't found
        if (gameState == null)
            Debug.LogError("GameState not found in scene!");

        // Log warning if tag is missing
        if (!CompareTag("LavaSquare"))
            Debug.LogWarning("LavaSquare object missing 'LavaSquare' tag!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameState != null)
        {
            gameState.HandleLavaCollision(other.gameObject);
        }
        else
        {
            Debug.LogError("GameState not found - cannot handle lava collision!");
        }
    }
}