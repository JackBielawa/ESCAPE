using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

using IndieMarc.Platformer;

public class GameState : MonoBehaviour
{

    private PlayerCharacterOne player1;
    private PlayerCharacterTwo player2;

    public int dragonCount;

    private bool player1dead = false;
    private bool player2dead = false;

    private bool gameOver = false;

    public Image gameOverOverlay;
    public Image levelCompleteOverlay;
    public float fadeSpeed = 0.5f;

    private Color overlayColor;
    private float maxAlpha = 120f / 255f;
    public TextMeshProUGUI gameOverTextTMP;
    public TextMeshProUGUI levelCompleteTextTMP;
    private Color textColor;
    //private LevelMenu levelMenu;

    public GameObject Bridge;

    public int unlockedCount;

    // Start is called before the first frame update
    void Start()
    {
        player1 = GameObject.Find("Player1")?.GetComponent<PlayerCharacterOne>();
        player2 = GameObject.Find("Player2")?.GetComponent<PlayerCharacterTwo>();
        //levelMenu = FindObjectOfType<LevelMenu>();

        dragonCount = 0;
        unlockedCount = 1;
        gameOver = false;

        Bridge.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

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
                

            // Fade in the overlay
            if (gameOverOverlay != null)
            {
                overlayColor = gameOverOverlay.color;
                overlayColor.a = Mathf.Clamp(overlayColor.a + fadeSpeed * Time.deltaTime, 0, maxAlpha);
                gameOverOverlay.color = overlayColor;
            }

            // Fade in the Game Over text
            if (gameOverTextTMP != null)
            {
                textColor = gameOverTextTMP.color;
                //textColor.a = 0;
                textColor.a = Mathf.Clamp(textColor.a + 2 * fadeSpeed * Time.deltaTime, 0, 1); // Full visibility is alpha = 1
                gameOverTextTMP.color = textColor;
            }

            BackToLevelMenu();

            Debug.Log($"Game Over Text Alpha: {textColor.a}");
        }

        if(dragonCount == 2)
        {
            // Fade in the overlay
            if (levelCompleteOverlay != null)
            {
                overlayColor = levelCompleteOverlay.color;
                overlayColor.a = Mathf.Clamp(overlayColor.a + fadeSpeed * Time.deltaTime, 0, maxAlpha);
                levelCompleteOverlay.color = overlayColor;
            }

            // Fade in the Game Over text
            if (levelCompleteTextTMP != null)
            {
                textColor = levelCompleteTextTMP.color;
                //textColor.a = 0;
                textColor.a = Mathf.Clamp(textColor.a + 2 * fadeSpeed * Time.deltaTime, 0, 1); // Full visibility is alpha = 1
                levelCompleteTextTMP.color = textColor;
            }
            unlockedCount++;
            Debug.Log($"Unlocked levels: {unlockedCount}");
            BackToLevelMenu();
            Debug.Log("Back to level menu");
        }



    }

    public void BackToLevelMenu()
    {
        StartCoroutine(WaitAndLoadScene());
    }

    private IEnumerator WaitAndLoadScene()
    {
        Debug.Log($"Waiting for 4 seconds before loading the scene...");
        yield return new WaitForSeconds(4.0f); 
        Debug.Log("Loading scene...");
        SceneManager.LoadScene(1); 
    }

}

