using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int CurrentScore { get; private set; }
    public int TotalScore { get; private set; }
    
    // Advanced Scoring variables
    public int CurrentCombo { get; private set; }
    public int MaxCombo { get; private set; }
    public int HitsReceived { get; private set; }
    public float LevelStartTime { get; private set; }
    public int CurrentLevelRetries { get; private set; }
    private bool isLevelActive;
    private bool isPaused = false;

    [Header("Audio")]
    public AudioClip[] levelMusicTracks;
    public AudioClip pauseMusic;
    public AudioClip deathMusic;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnComboChanged;
    public event Action OnGameOver;
    public event Action<int, int, int> OnLevelCompleteSummary;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Change music when scene finishes loading
        if (AudioManager.Instance != null && levelMusicTracks != null && levelMusicTracks.Length > 0)
        {
            string sceneName = scene.name;
            int trackIndex = -1;
            
            if (sceneName.StartsWith("Level"))
            {
                if (int.TryParse(sceneName.Replace("Level", ""), out int levelNum))
                {
                    trackIndex = Mathf.Clamp(levelNum - 1, 0, levelMusicTracks.Length - 1);
                }
            }
            else if (sceneName == "SecretLevel")
            {
                trackIndex = Mathf.Clamp(5, 0, levelMusicTracks.Length - 1);
            }

            if (trackIndex != -1)
            {
                AudioManager.Instance.PlayMusic(levelMusicTracks[trackIndex]);
            }
            else if (sceneName == "MainMenu")
            {
                // Let MainMenuController handle it, or we could handle it here if we had the track
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartLevel(bool isRetry)
    {
        CurrentScore = 0;
        CurrentCombo = 1;
        MaxCombo = 1;
        HitsReceived = 0;
        LevelStartTime = Time.time;
        isLevelActive = true;

        if (!isRetry)
        {
            CurrentLevelRetries = 0;
        }

        isPaused = false;
        Time.timeScale = 1f;

        OnScoreChanged?.Invoke(CurrentScore);
        OnComboChanged?.Invoke(CurrentCombo);
    }

    private void Update()
    {
        if (!isLevelActive) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Time.timeScale = 0f;
            // The UIManager handles UI. Since UIManager might not be a singleton, 
            // we let the UIManager listen to an event, OR we just find it.
            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null) ui.TogglePauseMenu(true);

            if (AudioManager.Instance != null && pauseMusic != null) 
                AudioManager.Instance.PauseGameMusic(pauseMusic);
        }
        else
        {
            Time.timeScale = 1f;
            UIManager ui = FindObjectOfType<UIManager>();
            if (ui != null) ui.TogglePauseMenu(false);

            if (AudioManager.Instance != null) 
                AudioManager.Instance.ResumeGameMusic();
        }
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        TotalScore += amount;
        OnScoreChanged?.Invoke(CurrentScore);
    }

    public void AddComboHit()
    {
        if (!isLevelActive) return;
        
        // Add flat score per hit multiplied by combo
        AddScore(10 * CurrentCombo);
        
        CurrentCombo++;
        if (CurrentCombo > MaxCombo) MaxCombo = CurrentCombo;

        OnComboChanged?.Invoke(CurrentCombo);
    }

    public void RegisterPlayerHit()
    {
        if (!isLevelActive) return;

        HitsReceived++;
        CurrentCombo = 1; // Reset combo
        OnComboChanged?.Invoke(CurrentCombo);
    }

    public void TriggerGameOver()
    {
        if (!isLevelActive) return;
        isLevelActive = false;

        CurrentLevelRetries++;
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.RegisterDeath();
        }

        if (AudioManager.Instance != null && deathMusic != null)
        {
            AudioManager.Instance.PlayMusic(deathMusic);
        }

        OnGameOver?.Invoke();
    }

    public void TriggerLevelComplete(int levelIndex, int baseBossScore)
    {
        if (!isLevelActive) return;
        isLevelActive = false;
        
        // Calculate Time Bonus
        float timeTaken = Time.time - LevelStartTime;
        int timeBonus = Mathf.Max(0, 10000 - (int)(timeTaken * 50)); 
        
        // Calculate Damage Penalty
        int damagePenalty = HitsReceived * 2000;
        
        // Calculate Final Level Score
        int finalScore = baseBossScore + timeBonus - damagePenalty;
        finalScore = Mathf.Max(0, finalScore); 

        // Penalty for retries (Divide score by attempts)
        if (CurrentLevelRetries > 0)
        {
            finalScore = finalScore / (1 + CurrentLevelRetries);
        }

        AddScore(finalScore);

        OnLevelCompleteSummary?.Invoke(CurrentScore, MaxCombo, CurrentLevelRetries);
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CompleteLevel(levelIndex, CurrentScore);
        }
    }

    public void ResetCurrentScore(bool isRetry = false)
    {
        StartLevel(isRetry);
    }
}
