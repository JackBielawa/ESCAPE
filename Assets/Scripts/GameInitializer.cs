using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
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