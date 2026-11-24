using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject gameOverPanel;

    // Your player script can read this: SettingsPanel.Sensitivity
    public static float Sensitivity => PlayerPrefs.GetFloat("Sensitivity", 1f);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ApplySavedVolume()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1f);
    }

    private void Awake()
    {
        volumeSlider.onValueChanged.AddListener(v =>
        {
            AudioListener.volume = v;
            PlayerPrefs.SetFloat("Volume", v);
        });

        sensitivitySlider.onValueChanged.AddListener(v =>
            PlayerPrefs.SetFloat("Sensitivity", v));

        fullscreenToggle.onValueChanged.AddListener(on =>
        {
            Screen.fullScreen = on;
            PlayerPrefs.SetInt("Fullscreen", on ? 1 : 0);
        });

        backButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        volumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Volume", 1f));
        sensitivitySlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("Sensitivity", 1f));
        fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }
}