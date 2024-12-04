using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using IndieMarc.Platformer;

public class GameState : MonoBehaviour
{

    private PlayerCharacterOne player1;
    private PlayerCharacterTwo player2;

    private bool player1dead = false;
    private bool player2dead = false;

    private bool gameOver = false;

    public Image gameOverOverlay;
    public float fadeSpeed = 0.5f;

    private Color overlayColor;
    private float maxAlpha = 120f / 255f;

    // Start is called before the first frame update
    void Start()
    {
        if (gameOverOverlay != null)
            overlayColor = gameOverOverlay.color;
            
    }

    // Update is called once per frame
    void Update()
    {
        player1 = GameObject.Find("Player1Object").GetComponent<PlayerCharacterOne>();
        player2 = GameObject.Find("Player2Object").GetComponent<PlayerCharacterTwo>();

        if (player1 != null)
            Debug.Log("Player 1 is_dead: " + player1.is_dead);
            if(player1.is_dead == true)
                player1dead = true;

        if (player2 != null)
            Debug.Log("Player 2 is_dead: " + player2.is_dead);
            if(player2.is_dead == true)
                player2dead = true;

        if(player1dead == true || player2dead == true)
        {
            gameOver = true;
        }

        if (gameOver)
        {
            if (gameOverOverlay != null)
            {
                overlayColor.a = Mathf.Clamp(overlayColor.a + fadeSpeed * Time.deltaTime, 0, maxAlpha);
                gameOverOverlay.color = overlayColor;
            }
        }

    }
}

