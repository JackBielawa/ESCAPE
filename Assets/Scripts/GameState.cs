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
    public GameObject restartButton;
    private PlayerCharacterOne player1;
    private PlayerCharacterTwo player2;

    public int dragonCount;
    public int unlockedCount;

    private bool player1dead = false;
    private bool player2dead = false;
    private bool gameOver = false;
    private bool levelCompleteProcessed = false;
    private bool gameCompleteTriggered = false;

    public Image gameOverOverlay;
    public Image levelCompleteOverlay;
    public Image gameCompleteOverlay;
    public float fadeSpeed = 0.5f;
    private float maxAlpha = 120f / 255f;

    public TextMeshProUGUI gameOverTextTMP;
    public TextMeshProUGUI levelCompleteTextTMP;
    public TextMeshProUGUI gameCompleteTextTMP;
    private Color overlayColor;
    private Color wonOverlayColor;
    private Color textColor;

    public GameObject Bridge;

    private bool gameOverSequenceStarted = false;

    void Start()
    {
        player1 = GameObject.Find("Player1")?.GetComponent<PlayerCharacterOne>();
        player2 = GameObject.Find("Player2")?.GetComponent<PlayerCharacterTwo>();

        dragonCount = 0;
        gameOver = false;

        if (Bridge != null)
            Bridge.SetActive(false);

        // Initialize overlays to be invisible and inactive
        if (gameOverOverlay != null)
        {
            Color startColor = gameOverOverlay.color;
            startColor.a = 0f;
            gameOverOverlay.color = startColor;
            gameOverOverlay.gameObject.SetActive(false);
        }

        if (gameOverTextTMP != null)
        {
            Color textStartColor = gameOverTextTMP.color;
            textStartColor.a = 0f;
            gameOverTextTMP.color = textStartColor;
        }

        // If you want to use the "LevelMenu" tag to ensure the level menu exists:
        // (Optional) Check if there's a LevelMenu object in the scene
        GameObject levelMenuObject = GameObject.FindGameObjectWithTag("LevelMenu");
        if (levelMenuObject == null)
        {
            Debug.LogWarning("No object with 'LevelMenu' tag found in the scene.");
        }
    }

    void Update()
    {
        if (player1 != null && player1.is_dead)
            player1dead = true;
        if (player2 != null && player2.is_dead)
            player2dead = true;

        // If either player is dead -> game over
        if ((player1dead || player2dead) && !gameOver && !levelCompleteProcessed && !gameCompleteTriggered)
        {
            gameOver = true;
        }

        if (gameOver && !gameOverSequenceStarted)
        {
            gameOverSequenceStarted = true;
            StartCoroutine(GameOverWaitAndLoadScene());
        }

        // If all dragons collected and not processed yet -> level complete
        if (dragonCount >= 2 && !levelCompleteProcessed && !gameOver)
        {
            Debug.Log("Dragon Count detected 2. Level complete. Calling UpdateLockedLevels...");
            levelCompleteProcessed = true;
            UpdateLockedLevels();
        }
    }

    private IEnumerator GameOverWaitAndLoadScene()
    {
        // Add null checks before using gameOverOverlay and gameOverTextTMP
        while ((gameOverOverlay != null && gameOverOverlay.color.a < maxAlpha) ||
               (gameOverTextTMP != null && gameOverTextTMP.color.a < 1.0f))
        {
            DisplayGameOverScreen();
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);
        // Load scene index 2 for level menu
        SceneManager.LoadScene(2);
    }

    public void TriggerGameOver()
    {
        if (!levelCompleteProcessed && !gameCompleteTriggered)
        {
            gameOver = true;

            if (!gameOverSequenceStarted)
            {
                gameOverSequenceStarted = true;
                StartCoroutine(GameOverWaitAndLoadScene());
            }
        }
    }

    private void DisplayGameOverScreen()
    {
        if (gameOverOverlay != null)
        {
            if (!gameOverOverlay.gameObject.activeSelf)
                gameOverOverlay.gameObject.SetActive(true);

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

    private void DisplayLevelCompleteScreen()
    {
        // Null checks for levelCompleteOverlay and levelCompleteTextTMP
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

    public void HandleLavaCollision(GameObject player)
    {
        if (player.CompareTag("Player") || player.CompareTag("PlayerTwo") || player.CompareTag("PlayerTag"))
        {
            if (player.CompareTag("Player") || player.CompareTag("PlayerTag"))
                player1dead = true;
            else if (player.CompareTag("PlayerTwo"))
                player2dead = true;

            gameOver = true;
            if (!gameOverSequenceStarted)
            {
                gameOverSequenceStarted = true;
                StartCoroutine(GameOverWaitAndLoadScene());
            }
        }
    }

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

    public void BackToLevelMenu()
    {
        StartCoroutine(WaitAndLoadScene());
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator WaitAndLoadScene()
    {
        // Add null checks for levelCompleteOverlay and levelCompleteTextTMP
        while ((levelCompleteOverlay != null && levelCompleteOverlay.color.a < maxAlpha) ||
               (levelCompleteTextTMP != null && levelCompleteTextTMP.color.a < 1.0f))
        {
            DisplayLevelCompleteScreen();
            yield return null;
        }

        Debug.Log("Level complete screen fully displayed. Transitioning to level menu...");
        yield return new WaitForSeconds(2.0f);

        // Load scene 2 for level menu
        SceneManager.LoadScene(2);
    }

    private IEnumerator GameCompleteBackToMenu()
    {
        while ((gameCompleteOverlay != null && gameCompleteOverlay.color.a < maxAlpha) ||
               (gameCompleteTextTMP != null && gameCompleteTextTMP.color.a < 1.0f))
        {
            DisplayGameCompleteScreen();
            yield return null;
        }

        Debug.Log("Game complete screen fully displayed. Transitioning to main menu...");
        yield return new WaitForSeconds(2.0f);
        BackToMainMenu();
    }

    public void UpdateLockedLevels()
    {
        unlockedCount = PlayerPrefs.GetInt("unlockedCount", 0);
        unlockedCount++;
        Debug.Log("Updated unlockedCount to: " + unlockedCount);

        PlayerPrefs.SetInt("unlockedCount", unlockedCount);
        PlayerPrefs.Save();

        if (unlockedCount == 3)
        {
            Debug.Log("Game complete detected. Starting GameCompleteBackToMenu...");
            StartCoroutine(GameCompleteBackToMenu());
        }
        else
        {
            DisplayLevelCompleteScreen();
            BackToLevelMenu();
        }
    }

    public void RestartButton()
    {
        SceneManager.LoadScene(2);
    }
}
