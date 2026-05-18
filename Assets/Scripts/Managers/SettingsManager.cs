using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private const string VOL_KEY = "MasterVolume";
    private const string FULLSCREEN_KEY = "IsFullscreen";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadSettings()
    {
        // Load Volume (default 1.0)
        float volume = PlayerPrefs.GetFloat(VOL_KEY, 1f);
        AudioListener.volume = volume;

        // Load Fullscreen (default true)
        bool isFullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat(VOL_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
