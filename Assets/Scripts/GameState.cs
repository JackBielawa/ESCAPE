using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace IndieMarc.Platformer
{
    public class GameState : MonoBehaviour
    {
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

        public GameObject Bridge;

        private Color overlayColor;
        private Color textColor;
        private Color wonOverlayColor;

        void Start()
        {
            unlockedCount = PlayerPrefs.GetInt("unlockedCount", 0);

            player1 = GameObject.FindWithTag("Player")?.GetComponent<PlayerCharacterOne>();
            player2 = GameObject.FindWithTag("PlayerTwo")?.GetComponent<PlayerCharacterTwo>();

        }

        void Update()
        {
            if (player1 != null && player1.is_dead)
                player1dead = true;
            if (player2 != null && player2.is_dead)
                player2dead = true;

            if ((player1dead || player2dead) && !gameOver && !levelCompleteProcessed && !gameCompleteTriggered)
            {
                gameOver = true;
                StartCoroutine(GameOverSequence());
            }

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

        public void UpdateLockedLevels()
        {
            // Update unlock count (increment by 1 each time a level is completed)
            unlockedCount = PlayerPrefs.GetInt("unlockedCount", 0);
            unlockedCount++;
            PlayerPrefs.SetInt("unlockedCount", unlockedCount);
            PlayerPrefs.Save();

            if (unlockedCount == 4)
            {
                // All levels unlocked - game complete
                gameCompleteTriggered = true;
                StartCoroutine(GameCompleteSequence());
            }
            else
            {
                StartCoroutine(LevelCompleteSequence());
            }
        }

        private IEnumerator GameOverSequence()
        {
            // Fade in the game over screen until fully visible
            while ((gameOverOverlay != null && gameOverOverlay.color.a < maxAlpha)
                   || (gameOverTextTMP != null && gameOverTextTMP.color.a < 1.0f))
            {
                DisplayGameOverScreen();
                yield return null;
            }

            // Wait before returning to level selection
            yield return new WaitForSeconds(2.0f);
            BackToLevelMenu();
        }

        private IEnumerator LevelCompleteSequence()
        {
            while ((levelCompleteOverlay != null && levelCompleteOverlay.color.a < maxAlpha)
                   || (levelCompleteTextTMP != null && levelCompleteTextTMP.color.a < 1.0f))
            {
                DisplayLevelCompleteScreen();
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);
            BackToLevelMenu();
        }

        private IEnumerator GameCompleteSequence()
        {
            while ((gameCompleteOverlay != null && gameCompleteOverlay.color.a < maxAlpha)
                   || (gameCompleteTextTMP != null && gameCompleteTextTMP.color.a < 1.0f))
            {
                DisplayGameCompleteScreen();
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);
            BackToMainMenu();
        }

        public void BackToLevelMenu()
        {
            // Load the level selection scene, which should read unlockedCount from PlayerPrefs
            SceneManager.LoadScene(1);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
