using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    private int cookieCount;

    public int unlockedCount;
    public GameObject level2Object;
    public GameObject level3Object;
    public GameObject level4Object;
    public GameObject level5Object;
    
    void Start()
    {

        unlockedCount = 1;
        /*
        if (PlayerPrefs.HasKey("CookieCount"))
        {
            cookieCount = PlayerPrefs.GetInt("CookieCount");
            Debug.Log("Cookie count retrieved: " + cookieCount);
        }
        else
        {
            Debug.LogError("Cookie count not found in PlayerPrefs.");
        }
        Debug.Log("Cookie count is " + cookieCount);
        */
        if(cookieCount == 1)
        {
            if (level2Object != null) // Check if the reference is assigned
            {
                level2Object.SetActive(true); // Activate the GameObject
                Debug.Log("Player's cookie count: " + cookieCount);
            }
            else
            {
                Debug.LogError("level2Object is not assigned in the Inspector!");
            }

        } else if(cookieCount == 2)
        {
            if (level3Object != null) // Check if the reference is assigned
            {
                level3Object.SetActive(true); // Activate the GameObject
                Debug.Log("Player's cookie count: " + cookieCount);
            }
            else
            {
                Debug.LogError("level3Object is not assigned in the Inspector!");
            }
        }

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

/*

    public void UpdateLockedLevels ()
    {
        if(cookieCount == 1)
        {
            GameObject imageGameObject = GameObject.Find("Level 2");
            imageGameObject.SetActive(true);
            Debug.Log("Player's cookie count: " + cookieCount);

        }
    }
*/
}
