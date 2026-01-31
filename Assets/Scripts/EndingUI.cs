using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// UI component to display ending based on player choices
/// Shows fade to black + ending text
/// </summary>
public class EndingUI : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Panel chứa ending UI")]
    [SerializeField] private GameObject endingPanel;
    
    [Tooltip("Text hiển thị ending message")]
    [SerializeField] private TextMeshProUGUI endingText;
    
    [Tooltip("Black overlay cho fade effect")]
    [SerializeField] private Image blackOverlay;
    
    [Tooltip("(Optional) Play Again button")]
    [SerializeField] private Button playAgainButton;

    [Header("Animation Settings")]
    [Tooltip("Fade to black duration")]
    [SerializeField] private float fadeDuration = 2f;
    
    [Tooltip("Text fade in duration")]
    [SerializeField] private float textFadeDuration = 1.5f;
    
    [Tooltip("Delay before showing text")]
    [SerializeField] private float textDelay = 1f;

    [Header("Ending Texts")]
    [SerializeField] [TextArea(3, 6)] 
    private string goodEndingText = "Trong đêm vắng,\nchỉ một chút dịu dàng\ncũng đủ làm người ta nhớ.";
    
    [SerializeField] [TextArea(3, 6)]
    private string neutralEndingText = "Bạn đã nói những điều thật.\nNhưng không phải đêm nào\ncũng cần sự thật.";
    
    [SerializeField] [TextArea(3, 6)]
    private string badEndingText = "Bạn đi hết con đường.\nNhưng chẳng ai nhớ\nbạn đã từng đi qua.";

    private CanvasGroup endingTextCanvasGroup;

    private void Awake()
    {
        // Get or add CanvasGroup to text
        endingTextCanvasGroup = endingText.GetComponent<CanvasGroup>();
        if (endingTextCanvasGroup == null)
        {
            endingTextCanvasGroup = endingText.gameObject.AddComponent<CanvasGroup>();
        }

        // Hide panel by default
        if (endingPanel != null)
        {
            endingPanel.SetActive(false);
        }

        // Setup play again button
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            playAgainButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Show ending UI with specified ending type
    /// </summary>
    public void ShowEnding(EndingType endingType)
    {
        endingPanel.SetActive(true);
        StartCoroutine(EndingSequence(endingType));
    }

    /// <summary>
    /// Ending animation sequence
    /// </summary>
    private IEnumerator EndingSequence(EndingType endingType)
    {
        // Step 1: Fade to black
        yield return StartCoroutine(FadeToBlack());

        // Step 2: Set ending text based on type
        SetEndingText(endingType);

        // Step 3: Wait a bit
        yield return new WaitForSeconds(textDelay);

        // Step 4: Fade in text
        yield return StartCoroutine(FadeInText());

        // Step 5: Show play again button after a delay
        yield return new WaitForSeconds(2f);
        if (playAgainButton != null)
        {
            playAgainButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Fade black overlay to full opacity
    /// </summary>
    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        Color startColor = blackOverlay.color;
        Color targetColor = new Color(0, 0, 0, 1); // Fully black

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            blackOverlay.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        blackOverlay.color = targetColor;
    }

    /// <summary>
    /// Fade in ending text
    /// </summary>
    private IEnumerator FadeInText()
    {
        float elapsed = 0f;
        endingTextCanvasGroup.alpha = 0f;

        while (elapsed < textFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / textFadeDuration;
            endingTextCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        endingTextCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Set ending text based on ending type
    /// </summary>
    private void SetEndingText(EndingType endingType)
    {
        switch (endingType)
        {
            case EndingType.GOOD:
                endingText.text = goodEndingText;
                break;
            
            case EndingType.NEUTRAL:
                endingText.text = neutralEndingText;
                break;
            
            case EndingType.BAD:
                endingText.text = badEndingText;
                break;
        }

        Debug.Log($"EndingUI: Showing {endingType} ending");
    }

    /// <summary>
    /// Called when Play Again button clicked
    /// </summary>
    private void OnPlayAgainClicked()
    {
        // Reset choices
        if (MaskChoiceTracker.Instance != null)
        {
            MaskChoiceTracker.Instance.ResetChoices();
        }

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
