using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private static bool initialized = false; // Static variable to ensure it only runs once

    void Awake()
    {
        if (!initialized)
        {
            InitializePlayerPrefs(); // Call your initialization logic
            initialized = true; // Mark as initialized
            DontDestroyOnLoad(gameObject); // Keep this GameObject alive across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void InitializePlayerPrefs()
    {
        // Check if the key "unlockedCount" exists
        if (!PlayerPrefs.HasKey("unlockedCount"))
        {
            PlayerPrefs.SetInt("unlockedCount", 0);
            PlayerPrefs.Save(); // Save to persist the change
            Debug.Log("PlayerPrefs initialized: unlockedCount set to 0");
        }
        else
        {
            Debug.Log("PlayerPrefs already initialized: unlockedCount = " + PlayerPrefs.GetInt("unlockedCount"));
        }
    }
}
