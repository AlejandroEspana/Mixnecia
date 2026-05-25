using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class CreditsManager : MonoBehaviour
{
    public static CreditsManager Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject creditsPanel;
    public TextMeshProUGUI creditsText;
    public float scrollSpeed = 50f;

    [Header("Credits Content")]
    [TextArea(10, 20)]
    public string creditsString = "MIXEL - A Visual Novel\n\nGame Design & Story\nAlejandro\n\nArt & Sprites\nAlejandro\n\nProgramming\nAlejandro\n\nThanks for playing!";

    private RectTransform textRectTransform;
    private bool isScrolling = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            // Make sure the object this script is attached to survives
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
            // Also protect the Canvas that holds the UI, otherwise it gets destroyed!
            if (creditsPanel != null)
            {
                Canvas parentCanvas = creditsPanel.GetComponentInParent<Canvas>();
                if (parentCanvas != null)
                {
                    parentCanvas.transform.SetParent(null);
                    DontDestroyOnLoad(parentCanvas.gameObject);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (creditsText != null) textRectTransform = creditsText.GetComponent<RectTransform>();
    }

    public void StartCredits(Action onCreditsFinished = null)
    {
        Debug.Log("CreditsManager: StartCredits called.");
        if (creditsPanel != null) creditsPanel.SetActive(true);
        if (creditsText != null && textRectTransform != null)
        {
            Debug.Log("CreditsManager: UI elements are valid. Scrolling started.");
            creditsText.text = creditsString;
            textRectTransform.anchoredPosition = new Vector2(textRectTransform.anchoredPosition.x, -Screen.height);
            isScrolling = true;
            StartCoroutine(ScrollCredits(onCreditsFinished));
        }
        else
        {
            // If UI is not assigned, just invoke callback to prevent softlock
            Debug.LogWarning("CreditsManager: Missing creditsText or textRectTransform! Skipping credits.");
            onCreditsFinished?.Invoke();
        }
    }

    private IEnumerator ScrollCredits(Action onCreditsFinished)
    {
        while (isScrolling && textRectTransform.anchoredPosition.y < textRectTransform.rect.height + Screen.height)
        {
            textRectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.unscaledDeltaTime;
            
            // Allow skip
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            {
                break;
            }
            yield return null;
        }

        isScrolling = false;
        if (creditsPanel != null) creditsPanel.SetActive(false);
        onCreditsFinished?.Invoke();
    }
}
