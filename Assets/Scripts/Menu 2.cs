using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Called when the Play button is clicked
    public void OnPlayButton()
    {
        Debug.Log("Play button clicked. Loading the next scene...");
        SceneManager.LoadScene(1); // Loads Level Menu
    }

    // Called when the Quit button is clicked
    public void OnQuitButton()
    {
        Debug.Log("Quit button clicked. Exiting the application...");
        Application.Quit(); // Exits the game when built
    }
}
