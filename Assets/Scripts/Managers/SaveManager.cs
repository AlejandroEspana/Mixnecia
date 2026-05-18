using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    
    public GameSaveData CurrentSaveData { get; private set; }
    public int CurrentSlotIndex { get; private set; } = -1;

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

    private string GetSaveFilePath(int slotIndex)
    {
        return Application.persistentDataPath + $"/bullethell_save_slot{slotIndex}.dat";
    }

    public void SelectSlot(int slotIndex)
    {
        CurrentSlotIndex = slotIndex;
        GameSaveData data = LoadGameData(slotIndex);
        
        if (data == null)
        {
            CurrentSaveData = new GameSaveData();
            SaveGame();
        }
        else
        {
            CurrentSaveData = data;
        }
    }

    public void SaveGame()
    {
        if (CurrentSlotIndex == -1) return;
        if (CurrentSaveData == null) CurrentSaveData = new GameSaveData();

        string saveFilePath = GetSaveFilePath(CurrentSlotIndex);

        using (FileStream stream = new FileStream(saveFilePath, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(CurrentSaveData.completedLevels.Length);
                for (int i = 0; i < CurrentSaveData.completedLevels.Length; i++)
                {
                    writer.Write(CurrentSaveData.completedLevels[i]);
                    writer.Write(CurrentSaveData.levelScores[i]);
                }
                writer.Write(CurrentSaveData.isSecretLevelUnlocked);
                writer.Write(CurrentSaveData.totalDeaths);
            }
        }
    }

    public GameSaveData LoadGameData(int slotIndex)
    {
        string saveFilePath = GetSaveFilePath(slotIndex);
        if (File.Exists(saveFilePath))
        {
            try
            {
                using (FileStream stream = new FileStream(saveFilePath, FileMode.Open))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        GameSaveData data = new GameSaveData();
                        int numLevels = reader.ReadInt32();
                        
                        if (numLevels == data.completedLevels.Length)
                        {
                            for (int i = 0; i < numLevels; i++)
                            {
                                data.completedLevels[i] = reader.ReadBoolean();
                                data.levelScores[i] = reader.ReadInt32();
                            }
                            data.isSecretLevelUnlocked = reader.ReadBoolean();
                            data.totalDeaths = reader.ReadInt32();
                            return data;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading save file: " + e.Message);
            }
        }
        return null; // Return null if file doesn't exist or is corrupted
    }

    public bool IsSlotEmpty(int slotIndex)
    {
        return !File.Exists(GetSaveFilePath(slotIndex));
    }

    public void RegisterDeath()
    {
        if (CurrentSaveData != null)
        {
            CurrentSaveData.totalDeaths++;
            SaveGame(); // Save immediately to prevent cheating
        }
    }

    public void DeleteSlot(int slotIndex)
    {
        string path = GetSaveFilePath(slotIndex);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted save slot {slotIndex}");
        }

        if (CurrentSlotIndex == slotIndex)
        {
            CurrentSlotIndex = -1;
            CurrentSaveData = null;
        }
    }

    public int GetNextLevelIndex()
    {
        if (CurrentSaveData == null) return 1;

        // Find the first uncompleted level (0 to 4 is Level 1 to 5)
        for (int i = 0; i < CurrentSaveData.completedLevels.Length; i++)
        {
            if (!CurrentSaveData.completedLevels[i])
            {
                return i + 1; // +1 because Level 1 is index 0
            }
        }

        // If all completed, return 5 (last level) or maybe 6 if secret is unlocked
        if (CurrentSaveData.isSecretLevelUnlocked) return 6; // Secret Level
        return 5;
    }

    public void CompleteLevel(int levelIndex, int score)
    {
        if (CurrentSaveData != null && levelIndex >= 0 && levelIndex < CurrentSaveData.completedLevels.Length)
        {
            CurrentSaveData.completedLevels[levelIndex] = true;
            if (score > CurrentSaveData.levelScores[levelIndex])
            {
                CurrentSaveData.levelScores[levelIndex] = score;
            }

            CheckSecretLevelUnlock();
            SaveGame();
        }
    }

    private void CheckSecretLevelUnlock()
    {
        bool allLevelsCompleted = true;
        foreach (bool completed in CurrentSaveData.completedLevels)
        {
            if (!completed) allLevelsCompleted = false;
        }

        // Unlock only if all 5 levels are beaten with ZERO total deaths
        if (allLevelsCompleted && CurrentSaveData.totalDeaths == 0)
        {
            CurrentSaveData.isSecretLevelUnlocked = true;
        }
    }
}
