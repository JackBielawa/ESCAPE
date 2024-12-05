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
    void Start()
    {



    }

    void Update()
    {
        //UpdateLockedLevels();
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
        if (unlockedCount >= 2 && level2Object != null)
        {
            level2Object.SetActive(true);
            Debug.Log("Level 2 unlocked");
        }
    }
    */

}
