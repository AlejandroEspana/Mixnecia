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

        // If we just finished Level 5 (Build Index 6), check for Secret Level
        if (currentIndex == 6)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSaveData.isSecretLevelUnlocked)
            {
                SceneManager.LoadScene(7); // Secret Level
                if (GameManager.Instance != null) GameManager.Instance.ResetCurrentScore(false);
            }
            else
            {
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
            LoadMainMenu();
        }
    }

    public void LoadSecretLevel()
    {
        if (ObjectPooler.Instance != null) ObjectPooler.Instance.DeactivateAllObjects();
        SceneManager.LoadScene("SecretLevel");
    }
}
