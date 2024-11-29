using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieMarc.Platformer;


public class PowerUpOne : MonoBehaviour
{
    // Duration of the power-up effect (optional)
    public float duration = 10f;

    // When the player collides with the power-up
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacterOne player = collision.GetComponent<PlayerCharacterOne>();
        if (player != null)
        {
            // Grant the ability to shoot to the player
            player.EnableShooting();

            // Optionally, you can destroy the power-up object
            Destroy(gameObject);

            // If you want the power-up to have a duration, you can start a coroutine
            // StartCoroutine(TemporaryPowerUp(player));
        }
    }

    // Optional: If you want the power-up to be temporary
    private IEnumerator TemporaryPowerUp(PlayerCharacterOne player)
    {
        player.EnableShooting();
        Destroy(gameObject); // Remove the power-up from the scene

        yield return new WaitForSeconds(duration);

        player.DisableShooting();
    }
}
