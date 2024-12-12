using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Preserve this object across scenes
        }
        else
        {
            Destroy(gameObject); // Avoid duplicates if another AudioManager exists
        }
    }
}
