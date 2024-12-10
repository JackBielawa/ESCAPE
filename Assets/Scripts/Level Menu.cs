using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public GameObject level2Object;
    public GameObject level3Object;
    public GameObject level4Object;
    public GameObject level5Object;

    public GameObject level2Lock;
    public GameObject level3Lock;
    public GameObject level4Lock;
    public GameObject level5Lock;

    private int unlockedCount;
    void Start()
    {
        // Retrieve unlockedCount from PlayerPrefs, with a default value of 0
        unlockedCount = PlayerPrefs.GetInt("unlockedCount", 0);
        Debug.Log("unlockedCount retrieved: " + unlockedCount);

        level4Object.SetActive(false);

        // Unlock levels based on unlockedCount
        if (unlockedCount >= 1)
        {
            if (level2Object != null)
            {
                level2Object.SetActive(true);
                level2Lock.SetActive(false);
            }
            else
            {
                Debug.LogError("level2Object is not assigned in the Inspector!");
            }
        }
        if (unlockedCount >= 2)
        {
            if (level3Object != null)
            {
                level3Object.SetActive(true);
                level3Lock.SetActive(false);
            }
            else
            {
                Debug.LogError("level3Object is not assigned in the Inspector!");
            }
        }
        if (unlockedCount >= 3)
        {
            if (level4Object != null)
            {
                level4Object.SetActive(true);
                level4Lock.SetActive(false);
            }
            else
            {
                Debug.LogError("level4Object is not assigned in the Inspector!");
            }
        }
    }

    void Update()
    {

    }

    public void MainMenuButton ()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayLevel1Button ()
    {
        SceneManager.LoadScene(3);
    }

    public void PlayLevel2Button ()
    {
        SceneManager.LoadScene(4);
    }

    public void PlayLevel3Button ()
    {
        SceneManager.LoadScene(5);
    }

    public void ContinueButton ()
    {
        SceneManager.LoadScene(2);
    }

    public void NextContinueButton ()
    {
        SceneManager.LoadScene(6);
    }

    public void LoreRestartButton ()
    {
        SceneManager.LoadScene(0);
    }


}
