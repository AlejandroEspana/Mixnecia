using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 5)]
    public string text;
    public Sprite speakerPortrait;
    public bool isProtagonist;
}

[Serializable]
public class DialogueSequence
{
    public List<DialogueLine> lines;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public float typingSpeed = 0.05f;

    private Queue<DialogueLine> linesQueue;
    private Action onSequenceComplete;
    private bool isTyping;
    private string currentFullLine;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        linesQueue = new Queue<DialogueLine>();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        // Use Spacebar to advance dialogue
        if (dialoguePanel != null && dialoguePanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Finish typing instantly
                StopAllCoroutines();
                dialogueText.text = currentFullLine;
                isTyping = false;
            }
            else
            {
                DisplayNextLine();
            }
        }
    }

    public void PlaySequence(DialogueSequence sequence, Action onComplete = null)
    {
        if (sequence == null || sequence.lines == null || sequence.lines.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        Time.timeScale = 0f; // Freeze time during dialogue
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        this.onSequenceComplete = onComplete;
        
        linesQueue.Clear();
        foreach (DialogueLine line in sequence.lines)
        {
            linesQueue.Enqueue(line);
        }

        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (linesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = linesQueue.Dequeue();
        
        if (nameText != null) nameText.text = line.speakerName;
        
        Sprite portraitToUse = line.speakerPortrait;
        if (portraitToUse == null && PortraitManager.Instance != null)
        {
            portraitToUse = PortraitManager.Instance.GetPortrait(line.speakerName);
        }

        if (portraitImage != null && portraitToUse != null)
        {
            portraitImage.sprite = portraitToUse;
            portraitImage.enabled = true;
            
            // Flip portrait depending on who is talking
            if (line.isProtagonist)
                portraitImage.transform.localScale = new Vector3(1, 1, 1);
            else
                portraitImage.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (portraitImage != null)
        {
            portraitImage.enabled = false;
        }

        currentFullLine = line.text;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(line.text));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        if (dialogueText != null)
        {
            dialogueText.text = "";
            isTyping = true;
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSecondsRealtime(typingSpeed); // Realtime because timeScale is 0
            }
            isTyping = false;
        }
    }

    private void EndDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        Time.timeScale = 1f; // Unfreeze time
        onSequenceComplete?.Invoke();
    }
}
