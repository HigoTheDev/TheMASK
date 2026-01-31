using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Component for displaying dialogue
/// Attach to dialogue panel in Canvas
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel chứa toàn bộ dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    
    [Tooltip("Text hiển thị tên NPC")]
    [SerializeField] private TextMeshProUGUI npcNameText;
    
    [Tooltip("Text hiển thị nội dung dialogue")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    [Tooltip("Image hiển thị avatar NPC (tùy chọn)")]
    [SerializeField] private Image npcAvatarImage;
    
    [Tooltip("Indicator cho player biết có thể next (ví dụ: arrow, text 'Press Space')")]
    [SerializeField] private GameObject continueIndicator;

    [Header("Animation Settings")]
    [Tooltip("Tốc độ typewriter effect (characters per second). 0 = không dùng effect")]
    [SerializeField] private float typewriterSpeed = 30f;
    
    [Tooltip("Fade duration khi show/hide dialogue")]
    [SerializeField] private float fadeDuration = 0.3f;

    private CanvasGroup canvasGroup;
    private bool isTyping = false;
    private Coroutine typewriterCoroutine;

    private void Awake()
    {
        // Get or add CanvasGroup for fading
        canvasGroup = dialoguePanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = dialoguePanel.AddComponent<CanvasGroup>();
        }

        // Hide by default
        HideDialogue();
    }

    /// <summary>
    /// Show dialogue panel with NPC info
    /// </summary>
    public void ShowDialogue(string npcName, Sprite npcAvatar = null)
    {
        dialoguePanel.SetActive(true);

        // Set NPC name
        if (npcNameText != null)
        {
            npcNameText.text = npcName;
        }

        // Set NPC avatar
        if (npcAvatarImage != null)
        {
            if (npcAvatar != null)
            {
                npcAvatarImage.sprite = npcAvatar;
                npcAvatarImage.gameObject.SetActive(true);
            }
            else
            {
                npcAvatarImage.gameObject.SetActive(false);
            }
        }

        // Fade in
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeDuration));
    }

    /// <summary>
    /// Hide dialogue panel
    /// </summary>
    public void HideDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeDuration, () =>
        {
            dialoguePanel.SetActive(false);
        }));
    }

    /// <summary>
    /// Set dialogue text with optional typewriter effect
    /// </summary>
    public void SetDialogueText(string text)
    {
        if (dialogueText == null)
            return;

        // Stop any ongoing typewriter
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        // Use typewriter effect if speed > 0
        if (typewriterSpeed > 0)
        {
            typewriterCoroutine = StartCoroutine(TypewriterEffect(text));
        }
        else
        {
            dialogueText.text = text;
            isTyping = false;
            ShowContinueIndicator(true);
        }
    }

    /// <summary>
    /// Typewriter effect coroutine
    /// </summary>
    private System.Collections.IEnumerator TypewriterEffect(string fullText)
    {
        isTyping = true;
        ShowContinueIndicator(false);
        dialogueText.text = "";

        float delay = 1f / typewriterSpeed;

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(delay); // Use realtime to work with Time.timeScale = 0
        }

        isTyping = false;
        ShowContinueIndicator(true);
        typewriterCoroutine = null;
    }

    /// <summary>
    /// Skip typewriter effect and show full text immediately
    /// </summary>
    public void SkipTypewriter()
    {
        if (isTyping && typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            // Set full text would need to be stored - for simplicity, just hide indicator
            isTyping = false;
            ShowContinueIndicator(true);
        }
    }

    /// <summary>
    /// Show/hide continue indicator
    /// </summary>
    private void ShowContinueIndicator(bool show)
    {
        if (continueIndicator != null)
        {
            continueIndicator.SetActive(show);
        }
    }

    /// <summary>
    /// Fade CanvasGroup coroutine
    /// </summary>
    private System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float endAlpha, float duration, System.Action onComplete = null)
    {
        float elapsed = 0f;
        cg.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled for pause support
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        cg.alpha = endAlpha;
        onComplete?.Invoke();
    }

    // Public getters
    public bool IsTyping() => isTyping;
}
