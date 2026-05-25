using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterPortrait
{
    public string characterName;
    public Sprite defaultPortrait;
}

public class PortraitManager : MonoBehaviour
{
    public static PortraitManager Instance { get; private set; }

    [Header("Global Character Portraits")]
    public List<CharacterPortrait> characterPortraits = new List<CharacterPortrait>();

    private Dictionary<string, Sprite> portraitDict;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDictionary()
    {
        portraitDict = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        foreach (var cp in characterPortraits)
        {
            if (!string.IsNullOrEmpty(cp.characterName) && cp.defaultPortrait != null)
            {
                portraitDict[cp.characterName] = cp.defaultPortrait;
            }
        }
    }

    public Sprite GetPortrait(string characterName)
    {
        if (string.IsNullOrEmpty(characterName)) return null;

        if (portraitDict.TryGetValue(characterName, out Sprite sprite))
        {
            return sprite;
        }

        return null;
    }
}
