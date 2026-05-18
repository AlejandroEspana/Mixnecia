using System;

[Serializable]
public class GameSaveData
{
    public bool[] completedLevels;
    public int[] levelScores;
    public bool isSecretLevelUnlocked;
    public int totalDeaths;

    public GameSaveData()
    {
        completedLevels = new bool[5];
        levelScores = new int[5];
        isSecretLevelUnlocked = false;
        totalDeaths = 0;
    }
}
