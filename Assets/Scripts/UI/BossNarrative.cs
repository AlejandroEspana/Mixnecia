using UnityEngine;

public class BossNarrative : MonoBehaviour
{
    [Header("Narrative Configuration")]
    public LevelID levelID;
    public bool loadFromDatabase = true;

    [Header("Manual Narrative Sequences")]
    public DialogueSequence introDialogue;
    public DialogueSequence outroDialogue;
    public DialogueSequence loreSequence;

    private void Awake()
    {
        if (loadFromDatabase)
        {
            introDialogue = DialogueDatabase.GetIntro(levelID);
            outroDialogue = DialogueDatabase.GetOutro(levelID);
            // loreSequence puede cargarse de forma similar si se añade al DialogueDatabase
        }
    }

    public DialogueSequence GetOutroDialogue()
    {
        if (loadFromDatabase)
        {
            outroDialogue = DialogueDatabase.GetOutro(levelID);
        }
        return outroDialogue;
    }

    private void Start()
    {
        // Register this narrative to the GameManager so UIManager can access the Lore sequence later
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentBossNarrative = this;
        }

        // Play Intro immediately when boss spawns
        if (DialogueManager.Instance != null && introDialogue != null && introDialogue.lines != null && introDialogue.lines.Count > 0)
        {
            DialogueManager.Instance.PlaySequence(introDialogue, null);
        }
    }
}
