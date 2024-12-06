using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private static bool initialized = false; // Static variable to ensure it only runs once

    void Awake()
    {
        if (!PlayerPrefs.HasKey("unlockedCount"))
        {
            PlayerPrefs.SetInt("unlockedCount", 0);
            PlayerPrefs.Save();
            Debug.Log("Initialized unlockedCount to 0");
        }
    }

}
