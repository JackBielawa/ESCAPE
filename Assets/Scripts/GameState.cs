using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

using IndieMarc.Platformer;

public class GameState : MonoBehaviour
{
    // Player references
    private PlayerCharacterOne player1;
    private PlayerCharacterTwo player2;

    // Track collected dragons and unlocked levels
    public int dragonCount;
    public int unlockedCount;

    // State tracking
    private bool player1dead = false;
    private bool player2dead = false;
    private bool gameOver = false;
    private bool levelCompleteProcessed = false;
    private bool gameCompleteTriggered = false;

    // UI elements and fade parameters
    public Image gameOverOverlay;
    public Image levelCompleteOverlay;
    public Image gameCompleteOverlay;
    public float fadeSpeed = 0.5f;
    private float maxAlpha = 120f / 255f;

    // TMP Text references
    public TextMeshProUGUI gameOverTextTMP;
    public TextMeshProUGUI levelCompleteTextTMP;
    public TextMeshProUGUI gameCompleteTextTMP;

    // Bridge and other elements
    public GameObject Bridge;

    // Internal color trackers
    private Color overlayColor;
    private Color textColor;
    private Color wonOverlayColor;

    void Start()
    {
        player1 = GameObject.Find("Player1")?.GetComponent<PlayerCharacterOne>();
        player2 = GameObject.Find("Player2")?.GetComponent<PlayerCharacterTwo>();

        dragonCount = 0;
        gameOver = false;
        levelCompleteProcessed = false;
        gameCompleteTriggered = false;

        Bridge.SetActive(false);
    }

    void Update()
    {
        // Check if any player is dead
        if (player1 != null && player1.is_dead)
            player1dead = true;
        if (player2 != null && player2.is_dead)
            player2dead = true;

        // If either player is dead -> game over
        if ((player1dead || player2dead) && !gameOver && !levelCompleteProcessed && !gameCompleteTriggered)
        {
            gameOver = true;
            StartCoroutine(GameOverSequence());
        }

        // If all dragons collected and not processed yet -> level complete
        if (dragonCount == 2 && !levelCompleteProcessed && !gameOver && !gameCompleteTriggered)
        {
            levelCompleteProcessed = true;
            UpdateLockedLevels();
        }
    }

    // Public method to trigger Game Over
    public void TriggerGameOver()
    {
        if (!levelCompleteProcessed && !gameCompleteTriggered)
        {
            gameOver = true;
            StartCoroutine(GameOverSequence());
        }
    }

    // Display and fade-in the Game Over screen
    private void DisplayGameOverScreen()
    {
        if (gameOverOverlay != null)
        {
            overlayColor = gameOverOverlay.color;
            overlayColor.a = Mathf.Clamp(overlayColor.a + fadeSpeed * Time.deltaTime, 0, maxAlpha);
            gameOverOverlay.color = overlayColor;
        }

        if (gameOverTextTMP != null)
        {
            textColor = gameOverTextTMP.color;
            textColor.a = Mathf.Clamp(textColor.a + 2 * fadeSpeed * Time.deltaTime, 0, 1);
            gameOverTextTMP.color = textColor;
        }
    }

    // Display and fade-in the Level Complete screen
    private void DisplayLevelCompleteScreen()
    {
        if (levelCompleteOverlay != null)
        {
            overlayColor = levelCompleteOverlay.color;
            overlayColor.a = Mathf.Clamp(overlayColor.a + fadeSpeed * Time.deltaTime, 0, maxAlpha);
            levelCompleteOverlay.color = overlayColor;
        }

        if (levelCompleteTextTMP != null)
        {
            textColor = levelCompleteTextTMP.color;
            textColor.a = Mathf.Clamp(textColor.a + 2 * fadeSpeed * Time.deltaTime, 0, 1);
            levelCompleteTextTMP.color = textColor;
        }
    }

    // Display and fade-in the Game Complete screen
    private void DisplayGameCompleteScreen()
    {
        if (gameCompleteOverlay != null)
        {
            wonOverlayColor = gameCompleteOverlay.color;
            wonOverlayColor.a = Mathf.Clamp(wonOverlayColor.a + fadeSpeed * Time.deltaTime, 0, maxAlpha);
            gameCompleteOverlay.color = wonOverlayColor;
        }

        if (gameCompleteTextTMP != null)
        {
            textColor = gameCompleteTextTMP.color;
            textColor.a = Mathf.Clamp(textColor.a + 2 * fadeSpeed * Time.deltaTime, 0, 1);
            gameCompleteTextTMP.color = textColor;
        }
    }

    // Called when all dragons are collected
    public void UpdateLockedLevels()
    {
        // Update unlock count
        unlockedCount = PlayerPrefs.GetInt("unlockedCount", 0);
        unlockedCount++;
        PlayerPrefs.SetInt("unlockedCount", unlockedCount);
        PlayerPrefs.Save();

        // If all levels are unlocked -> game complete
        if (unlockedCount == 4)
        {
            gameCompleteTriggered = true;
            StartCoroutine(GameCompleteSequence());
        }
        else
        {
            // Show level complete sequence and then back to level menu
            StartCoroutine(LevelCompleteSequence());
        }
    }

    // Coroutine for Game Over scenario
    private IEnumerator GameOverSequence()
    {
        // Fade in the game over screen until fully visible
        while ((gameOverOverlay != null && gameOverOverlay.color.a < maxAlpha)
               || (gameOverTextTMP != null && gameOverTextTMP.color.a < 1.0f))
        {
            DisplayGameOverScreen();
            yield return null;
        }

        // Wait a bit before going back to the level selection scene
        yield return new WaitForSeconds(2.0f);
        BackToLevelMenu();
    }

    // Coroutine for Level Complete scenario
    private IEnumerator LevelCompleteSequence()
    {
        // Fade in the level complete screen until fully visible
        while ((levelCompleteOverlay != null && levelCompleteOverlay.color.a < maxAlpha)
               || (levelCompleteTextTMP != null && levelCompleteTextTMP.color.a < 1.0f))
        {
            DisplayLevelCompleteScreen();
            yield return null;
        }

        // Wait a bit before going back to level select
        yield return new WaitForSeconds(2.0f);
        BackToLevelMenu();
    }

    // Coroutine for Game Complete scenario
    private IEnumerator GameCompleteSequence()
    {
        while ((gameCompleteOverlay != null && gameCompleteOverlay.color.a < maxAlpha)
               || (gameCompleteTextTMP != null && gameCompleteTextTMP.color.a < 1.0f))
        {
            DisplayGameCompleteScreen();
            yield return null;
        }

        // Wait a bit and then back to main menu
        yield return new WaitForSeconds(2.0f);
        BackToMainMenu();
    }

    // Methods to load scenes
    public void BackToLevelMenu()
    {
        SceneManager.LoadScene(1); // Assuming Scene 1 is the level selection scene
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0); // Assuming Scene 0 is the main menu
    }
}
