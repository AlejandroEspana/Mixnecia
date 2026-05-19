using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public GameObject pausePanel;
    public GameObject restScreenPanel;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public Slider playerHealthSlider;
    public Slider bossHealthSlider;
    public Slider precisionStaminaSlider;

    [Header("Level Complete Summary Elements")]
    public TextMeshProUGUI summaryScoreText;
    public TextMeshProUGUI summaryComboText;
    public TextMeshProUGUI summaryDeathsText;

    private PlayerHealth playerHealth;
    private BossHealth bossHealth;
    private PlayerController playerController;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnComboChanged += UpdateCombo;
            GameManager.Instance.OnGameOver += ShowGameOver;
            GameManager.Instance.OnLevelCompleteSummary += ShowLevelCompleteSummary;
            UpdateScore(GameManager.Instance.CurrentScore);
            UpdateCombo(GameManager.Instance.CurrentCombo);
        }

        // Setup Player Health
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdatePlayerHealth;
            if (playerHealthSlider != null)
            {
                playerHealthSlider.maxValue = playerHealth.maxHealth;
                playerHealthSlider.value = playerHealth.currentHealth;
            }
        }

        // Setup Boss Health
        bossHealth = FindObjectOfType<BossHealth>();
        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged += UpdateBossHealth;
            if (bossHealthSlider != null)
            {
                bossHealthSlider.maxValue = bossHealth.maxHealth;
                bossHealthSlider.value = bossHealth.currentHealth;
            }
        }

        // Setup Player Controller (Stamina)
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.OnStaminaChanged += UpdateStaminaUI;
            if (precisionStaminaSlider != null)
            {
                precisionStaminaSlider.maxValue = playerController.maxStamina;
                precisionStaminaSlider.value = playerController.currentStamina;
                precisionStaminaSlider.gameObject.SetActive(playerController.HasPrecisionUnlocked);
            }
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (restScreenPanel != null) restScreenPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnComboChanged -= UpdateCombo;
            GameManager.Instance.OnGameOver -= ShowGameOver;
            GameManager.Instance.OnLevelCompleteSummary -= ShowLevelCompleteSummary;
        }

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdatePlayerHealth;
        }

        if (bossHealth != null)
        {
            bossHealth.OnHealthChanged -= UpdateBossHealth;
        }

        if (playerController != null)
        {
            playerController.OnStaminaChanged -= UpdateStaminaUI;
        }
    }

    private void UpdateScore(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore.ToString("D6");
        }
    }

    private void UpdateCombo(int newCombo)
    {
        if (comboText != null)
        {
            comboText.text = "Combo: x" + newCombo;
        }
    }

    private void UpdatePlayerHealth(int currentHealth)
    {
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = currentHealth;
        }
    }

    private void UpdateBossHealth(int currentHealth, int maxHealth)
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = currentHealth;
        }
    }

    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        if (precisionStaminaSlider != null)
        {
            precisionStaminaSlider.maxValue = maxStamina;
            precisionStaminaSlider.value = currentStamina;
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    private void ShowLevelCompleteSummary(int finalScore, int maxCombo, int retries)
    {
        if (levelCompletePanel != null) levelCompletePanel.SetActive(true);
        if (summaryScoreText != null) summaryScoreText.text = "Final Score: " + finalScore;
        if (summaryComboText != null) summaryComboText.text = "Max Combo: x" + maxCombo;
        if (summaryDeathsText != null) summaryDeathsText.text = "Attempts: " + (retries + 1);
    }

    // Button Events
    public void RetryLevel()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.RetryCurrentLevel();
        }
    }

    public void NextLevel()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentBossNarrative != null)
        {
            DialogueSequence lore = GameManager.Instance.CurrentBossNarrative.loreSequence;
            if (lore != null && lore.lines != null && lore.lines.Count > 0)
            {
                if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
                
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.PlaySequence(lore, () => {
                        ShowRestScreen();
                    });
                    return;
                }
            }
        }

        ShowRestScreen();
    }

    private void ShowRestScreen()
    {
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        
        if (restScreenPanel != null)
        {
            restScreenPanel.SetActive(true);
        }
        else
        {
            if (SceneController.Instance != null) SceneController.Instance.LoadNextLevel();
        }
    }

    public void ContinueFromRestScreen()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadNextLevel();
        }
    }

    public void ReturnToMenu()
    {
        if (GameManager.Instance != null && Time.timeScale == 0f)
        {
            GameManager.Instance.TogglePause(); // Resume timescale before changing scene
        }

        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadMainMenu();
        }
    }

    public void TogglePauseMenu(bool isPaused)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
    }

    public void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
    }
}
