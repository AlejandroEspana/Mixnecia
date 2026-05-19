using UnityEngine;

public class BossNarrative : MonoBehaviour
{
    [Header("Narrative Sequences")]
    public DialogueSequence introDialogue;
    public DialogueSequence outroDialogue;
    public DialogueSequence loreSequence;

    private void Start()
    {
        // Register this narrative to the GameManager so UIManager can access the Lore sequence later
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentBossNarrative = this;
        }

        // Play Intro immediately when boss spawns
        if (DialogueManager.Instance != null && introDialogue != null && introDialogue.lines.Count > 0)
        {
            DialogueManager.Instance.PlaySequence(introDialogue, null);
        }
    }
}
