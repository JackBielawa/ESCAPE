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

    public Image gameOverOverlay;
    public Image levelCompleteOverlay;
    public Image gameCompleteOverlay;
    public float fadeSpeed = 0.5f;

    private Color overlayColor;
    private Color wonOverlayColor;
    private float maxAlpha = 120f / 255f;
    public TextMeshProUGUI gameOverTextTMP;
    public TextMeshProUGUI levelCompleteTextTMP;
    public TextMeshProUGUI gameCompleteTextTMP;
    private Color textColor;

    public GameObject Bridge;

    private bool levelCompleteProcessed = false; // New flag to track level completion

    void Start()
    {
        player1 = GameObject.Find("Player1")?.GetComponent<PlayerCharacterOne>();
        player2 = GameObject.Find("Player2")?.GetComponent<PlayerCharacterTwo>();

        dragonCount = 0;
        gameOver = false;

        Bridge.SetActive(false);

        Debug.Log($"Dragon count set to " + dragonCount);

    }

    void Update()
    {
        if (player1 != null && player1.is_dead)
            player1dead = true;

        if (player2 != null && player2.is_dead)
            player2dead = true;

        if (player1dead || player2dead)
        {
            gameOver = true;
        }

        if (gameOver)
        {
            DisplayGameOverScreen();
            BackToLevelMenu();
            //dragonCount = 0;
        }


        if (dragonCount >= 2 && !levelCompleteProcessed)
        {

            Debug.Log("Dragon Count detected 2. Level complete. Calling UpdateLockedLevels...");
            levelCompleteProcessed = true;
            UpdateLockedLevels();
        }

    }

    public void TriggerGameOver()
    {
        gameOver = true;
    }

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

        //dragonCount = 0;
    }

    private void DisplayGameCompleteScreen()
    {
        if (gameCompleteOverlay != null)
        {
            wonOverlayColor = gameCompleteOverlay.color;
            wonOverlayColor.a = Mathf.Clamp(wonOverlayColor.a + fadeSpeed * Time.deltaTime, 0, maxAlpha);
            gameCompleteOverlay.color = wonOverlayColor;
            Debug.Log($"GameComplete Overlay Alpha: {gameCompleteOverlay.color.a}");
        }

        if (gameCompleteTextTMP != null)
        {
            textColor = gameCompleteTextTMP.color;
            textColor.a = Mathf.Clamp(textColor.a + 2 * fadeSpeed * Time.deltaTime, 0, 1);
            gameCompleteTextTMP.color = textColor;
            Debug.Log($"GameComplete Text Alpha: {gameCompleteTextTMP.color.a}");
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
        // Continuously call the fade-in method until fully visible
        while (levelCompleteOverlay.color.a < maxAlpha || levelCompleteTextTMP.color.a < 1.0f)
        {
            DisplayLevelCompleteScreen(); // Ensure fade continues
            yield return null; // Wait for the next frame
        }

        Debug.Log("Level complete screen fully displayed. Transitioning to level menu...");
        yield return new WaitForSeconds(2.0f); // Optional extra delay for display
        SceneManager.LoadScene(2);
    }


    private IEnumerator GameCompleteBackToMenu()
    {
        Debug.Log("GameCompleteBackToMenu coroutine started.");
        // Continuously call the fade-in method until fully visible

        Debug.Log($"Initial GameComplete Overlay Alpha: {gameCompleteOverlay.color.a}");
        Debug.Log($"Initial GameComplete Text Alpha: {gameCompleteTextTMP.color.a}");

        Debug.Log("Hi");

        while (gameCompleteOverlay.color.a < maxAlpha || gameCompleteTextTMP.color.a < 1.0f)
        {
            Debug.Log("Displaying Game Complete Screen...");
            DisplayGameCompleteScreen(); // Ensure fade continues
            yield return null; // Wait for the next frame
        }

        Debug.Log("Game complete screen fully displayed. Transitioning to main menu...");
        yield return new WaitForSeconds(2.0f); // Optional extra delay for display
        BackToMainMenu();
    }

 
    public void UpdateLockedLevels()
    {
        // Retrieve the current unlockedCount from PlayerPrefs
        unlockedCount = PlayerPrefs.GetInt("unlockedCount", 0);

        // Increment the unlockedCount
        unlockedCount++;
        Debug.Log("Updated unlockedCount to: " + unlockedCount);

        // Save the updated unlockedCount back to PlayerPrefs
        PlayerPrefs.SetInt("unlockedCount", unlockedCount);
        PlayerPrefs.Save(); // Ensure the changes are saved immediately

        if (unlockedCount == 4)
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
