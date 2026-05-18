using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject saveSlotsPanel;
    public GameObject configPanel;
    public GameObject deleteConfirmPanel;
    public GameObject levelSelectPanel;

    [Header("Settings UI")]
    public UnityEngine.UI.Slider volumeSlider;
    public UnityEngine.UI.Toggle fullscreenToggle;

    [Header("Audio")]
    public AudioClip menuMusic;

    [Header("Slot UI Updates")]
    public TextMeshProUGUI[] slotStatusTexts; // Array of 3 TMP elements for the 3 slots

    [Header("Level Select UI")]
    public UnityEngine.UI.Button[] levelButtons; // Array of 6 buttons (0 = Level 1... 5 = Level 6)

    private int slotToDelete = -1;

    private void Start()
    {
        ShowMainMenu();

        if (AudioManager.Instance != null && menuMusic != null)
        {
            AudioManager.Instance.PlayMusic(menuMusic);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = PlayerPrefs.GetInt("IsFullscreen", 1) == 1;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    // Called by the Play button
    public void OnPlayClicked()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (saveSlotsPanel != null) saveSlotsPanel.SetActive(true);
        if (configPanel != null) configPanel.SetActive(false);
        UpdateSlotTexts();
    }

    // Called by the Config button
    public void OnConfigClicked()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (saveSlotsPanel != null) saveSlotsPanel.SetActive(false);
        if (configPanel != null) configPanel.SetActive(true);
    }

    // Called by the Quit button
    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Application Quit"); // For editor testing
    }

    // Called by back buttons to return to the root main menu
    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (saveSlotsPanel != null) saveSlotsPanel.SetActive(false);
        if (configPanel != null) configPanel.SetActive(false);
        if (deleteConfirmPanel != null) deleteConfirmPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    // --- Configuration Logic ---
    public void SetVolume(float val)
    {
        if (SettingsManager.Instance != null) SettingsManager.Instance.SetVolume(val);
    }

    public void SetFullscreen(bool val)
    {
        if (SettingsManager.Instance != null) SettingsManager.Instance.SetFullscreen(val);
    }

    // --- Deletion Logic ---
    public void PromptDeleteSlot(int slotIndex)
    {
        if (SaveManager.Instance == null || SaveManager.Instance.IsSlotEmpty(slotIndex)) return;
        
        slotToDelete = slotIndex;
        if (deleteConfirmPanel != null) deleteConfirmPanel.SetActive(true);
    }

    public void ConfirmDeleteSlot()
    {
        if (slotToDelete != -1 && SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSlot(slotToDelete);
            UpdateSlotTexts();
        }
        CancelDeleteSlot();
    }

    public void CancelDeleteSlot()
    {
        slotToDelete = -1;
        if (deleteConfirmPanel != null) deleteConfirmPanel.SetActive(false);
    }

    // Called by the 3 Slot buttons (pass 0, 1, or 2 from the button's OnClick event)
    public void OnSlotClicked(int slotIndex)
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager not found in scene!");
            return;
        }

        SaveManager.Instance.SelectSlot(slotIndex);
        ShowLevelSelector();
    }

    public void ShowLevelSelector()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (saveSlotsPanel != null) saveSlotsPanel.SetActive(false);
        if (configPanel != null) configPanel.SetActive(false);
        if (deleteConfirmPanel != null) deleteConfirmPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);

        if (SaveManager.Instance == null || SaveManager.Instance.CurrentSaveData == null) return;
        
        GameSaveData data = SaveManager.Instance.CurrentSaveData;

        // Level 1 is always unlocked
        if (levelButtons.Length > 0 && levelButtons[0] != null)
        {
            levelButtons[0].interactable = true;
            levelButtons[0].gameObject.SetActive(true);
        }

        // Levels 2 to 5 unlock if the previous level is completed
        for (int i = 1; i < 5; i++)
        {
            if (i < levelButtons.Length && levelButtons[i] != null)
            {
                levelButtons[i].gameObject.SetActive(true);
                levelButtons[i].interactable = data.completedLevels[i - 1]; // Can play if previous is done
            }
        }

        // Level 6 (Secret Level)
        if (levelButtons.Length > 5 && levelButtons[5] != null)
        {
            if (data.isSecretLevelUnlocked)
            {
                levelButtons[5].gameObject.SetActive(true);
                levelButtons[5].interactable = true;
            }
            else
            {
                levelButtons[5].gameObject.SetActive(false); // Hide the button completely
            }
        }
    }

    public void BackToSaveSlots()
    {
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (saveSlotsPanel != null) saveSlotsPanel.SetActive(true);
        UpdateSlotTexts();
    }

    public void LoadLevelFromSelector(int levelIndex)
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadLevel(levelIndex);
        }
    }

    private void UpdateSlotTexts()
    {
        if (SaveManager.Instance == null || slotStatusTexts == null) return;

        for (int i = 0; i < slotStatusTexts.Length; i++)
        {
            if (i >= 3) break; // Hardcoded to 3 slots based on requirements

            if (SaveManager.Instance.IsSlotEmpty(i))
            {
                slotStatusTexts[i].text = "Slot " + (i + 1) + " - Empty";
            }
            else
            {
                // Load data temporarily to show progress
                GameSaveData data = SaveManager.Instance.LoadGameData(i);
                if (data != null)
                {
                    int totalScore = 0;
                    foreach (int s in data.levelScores) totalScore += s;
                    slotStatusTexts[i].text = "Slot " + (i + 1) + " - Score: " + totalScore;
                }
                else
                {
                    slotStatusTexts[i].text = "Slot " + (i + 1) + " - Corrupt";
                }
            }
        }
    }
}
