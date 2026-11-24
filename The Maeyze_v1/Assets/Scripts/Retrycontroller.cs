using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Retrycontroller : MonoBehaviour
{
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Easy Level");
        Time.timeScale = 1f;
    }
}
