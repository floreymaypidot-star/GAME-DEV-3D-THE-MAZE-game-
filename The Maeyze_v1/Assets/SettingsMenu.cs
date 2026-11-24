using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject gameOverPanel;

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}