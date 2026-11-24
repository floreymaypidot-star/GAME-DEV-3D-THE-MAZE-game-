using UnityEngine;
using UnityEngine.SceneManagement;

public class QuittoMenu : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "StartScreen";

    public void GoToMainMenu()
    {
        // Reset time in case the game was paused
        Time.timeScale = 1f;

        // Unlock the cursor (important for a first-person game)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuSceneName);
    }
}