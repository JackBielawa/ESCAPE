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

    public int unlockedCount;

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

    public GameObject Bridge;

    private bool levelCompleteProcessed = false; // New flag to track level completion

    void Start()
    {
        player1 = GameObject.Find("Player1")?.GetComponent<PlayerCharacterOne>();
        player2 = GameObject.Find("Player2")?.GetComponent<PlayerCharacterTwo>();

        dragonCount = 0;
        gameOver = false;

        Bridge.SetActive(false);
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
        }

        if (dragonCount == 2)
        {

            DisplayLevelCompleteScreen();

            BackToLevelMenu();
        }
        if (dragonCount == 2 && !levelCompleteProcessed)
        {
            levelCompleteProcessed = true; // Set flag to prevent reprocessing
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
    }

    public void BackToLevelMenu()
    {
        StartCoroutine(WaitAndLoadScene());
    }

    private IEnumerator WaitAndLoadScene()
    {
        yield return new WaitForSeconds(4.0f);
        SceneManager.LoadScene(1);
    }

    public void UpdateLockedLevels()
    {

        unlockedCount++;
        Debug.Log("Updated unlockedCount to: " + unlockedCount);
        PlayerPrefs.SetInt("unlockedCount", unlockedCount);
        PlayerPrefs.Save();
            
    }
}
