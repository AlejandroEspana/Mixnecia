using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

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

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        if (ObjectPooler.Instance != null) ObjectPooler.Instance.DeactivateAllObjects();
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadLevel(int levelIndex)
    {
        if (ObjectPooler.Instance != null) ObjectPooler.Instance.DeactivateAllObjects();
        SceneManager.LoadScene("Level" + levelIndex);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetCurrentScore(false);
        }
    }

    public void RetryCurrentLevel()
    {
        if (ObjectPooler.Instance != null) ObjectPooler.Instance.DeactivateAllObjects();
        int index = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(index);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetCurrentScore(true);
        }
    }

    public void LoadNextLevel()
    {
        if (ObjectPooler.Instance != null) ObjectPooler.Instance.DeactivateAllObjects();
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        string activeSceneName = SceneManager.GetActiveScene().name;

        // If we just finished Level 5 (Build Index 6 or name "Level5"), check for Secret Level
        if (currentIndex == 6 || activeSceneName == "Level5")
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData != null && SaveManager.Instance.CurrentSaveData.isSecretLevelUnlocked)
            {
                Debug.Log("SceneController: Secret level unlocked. Loading Level 6.");
                SceneManager.LoadScene("Level6"); // Load by name for index-independence
                if (GameManager.Instance != null) GameManager.Instance.ResetCurrentScore(false);
            }
            else
            {
                Debug.Log("SceneController: Secret level NOT unlocked. Attempting to show credits.");
                if (CreditsManager.Instance != null)
                {
                    Debug.Log("SceneController: CreditsManager found. Starting credits.");
                    CreditsManager.Instance.StartCredits(() => { LoadMainMenu(); });
                }
                else
                {
                    Debug.LogWarning("SceneController: CreditsManager is NULL! Skipping directly to Main Menu.");
                    LoadMainMenu();
                }
            }
        }
        // If we just finished Secret Level (Build Index 7 or name "Level6"), roll credits and return to menu
        else if (currentIndex == 7 || activeSceneName == "Level6")
        {
            Debug.Log("SceneController: Finished Secret Level. Attempting to show credits.");
            if (CreditsManager.Instance != null)
            {
                Debug.Log("SceneController: CreditsManager found. Starting credits.");
                CreditsManager.Instance.StartCredits(() => { LoadMainMenu(); });
            }
            else
            {
                Debug.LogWarning("SceneController: CreditsManager is NULL! Skipping directly to Main Menu.");
                LoadMainMenu();
            }
        }
        // General progression for Level 1 to 4
        else if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
            if (GameManager.Instance != null) GameManager.Instance.ResetCurrentScore(false);
        }
        else
        {
            Debug.Log("SceneController: Finished last normal level. Attempting to show credits.");
            if (CreditsManager.Instance != null)
            {
                Debug.Log("SceneController: CreditsManager found. Starting credits.");
                CreditsManager.Instance.StartCredits(() => { LoadMainMenu(); });
            }
            else
            {
                Debug.LogWarning("SceneController: CreditsManager is NULL! Skipping directly to Main Menu.");
                LoadMainMenu();
            }
        }
    }

    public void LoadSecretLevel()
    {
        if (ObjectPooler.Instance != null) ObjectPooler.Instance.DeactivateAllObjects();
        SceneManager.LoadScene("Level6");
    }
}
