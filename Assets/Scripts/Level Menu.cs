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
        



        if (PlayerPrefs.HasKey("unlockedCount"))
        {
            unlockedCount = PlayerPrefs.GetInt("unlockedCount");
            Debug.Log("unlockedCount retrieved: " + unlockedCount);
        }
        else
        {
            Debug.LogError("unlockedCount not found in PlayerPrefs.");
        }
        Debug.Log("unlockedCount is " + unlockedCount);

        if(unlockedCount == 1)
        {
            if (level2Object != null) // Check if the reference is assigned
            {
                level2Object.SetActive(true);
                level2Lock.SetActive(false);
                Debug.Log("unlockedCount: " + unlockedCount);
            }
            else
            {
                Debug.LogError("level2Object is not assigned in the Inspector!");
            }

        } else if(unlockedCount == 2)
        {
            if (level3Object != null) // Check if the reference is assigned
            {
                level3Object.SetActive(true); // Activate the GameObject
                level3Lock.SetActive(false);
                Debug.Log("unlockedCount: " + unlockedCount);
            }
            else
            {
                Debug.LogError("level3Object is not assigned in the Inspector!");
            }
        } else if(unlockedCount == 3)
        {
            if (level4Object != null) // Check if the reference is assigned
            {
                level4Object.SetActive(true); // Activate the GameObject
                level4Lock.SetActive(false);
                Debug.Log("unlockedCount: " + unlockedCount);
            }
            else
            {
                Debug.LogError("level4Object is not assigned in the Inspector!");
            }
        } else if(unlockedCount == 4)
        {
            if (level5Object != null) // Check if the reference is assigned
            {
                level5Object.SetActive(true); // Activate the GameObject
                level5Lock.SetActive(false);
                Debug.Log("unlockedCount: " + unlockedCount);
            }
            else
            {
                Debug.LogError("level5Object is not assigned in the Inspector!");
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
        SceneManager.LoadScene(2);
    }

    public void PlayLevel2Button ()
    {
        SceneManager.LoadScene(3);
    }

    public void PlayLevel3Button ()
    {
        SceneManager.LoadScene(4);
    }

    public void PlayLevel4Button ()
    {
        SceneManager.LoadScene(5);
    }

    public void PlayLevel5Button ()
    {
        SceneManager.LoadScene(6);
    }


}
