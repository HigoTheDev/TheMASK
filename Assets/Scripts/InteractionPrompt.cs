using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI prompt that appears when player is near an interactable NPC
/// Shows "Press F to interact" or similar message
/// </summary>
public class InteractionPrompt : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text hiển thị prompt message")]
    [SerializeField] private TextMeshProUGUI promptText;
    
    [Tooltip("Panel chứa prompt (để show/hide)")]
    [SerializeField] private GameObject promptPanel;
    
    [Tooltip("Icon/Image tùy chọn (ví dụ: icon phím F)")]
    [SerializeField] private Image promptIcon;

    [Header("Settings")]
    [Tooltip("Message hiển thị khi có thể interact")]
    [SerializeField] private string interactMessage = "Press F to interact";
    
    [Tooltip("Khoảng cách offset từ NPC (WorldSpace canvas)")]
    [SerializeField] private Vector3 offsetFromTarget = new Vector3(0, 1.5f, 0);

    [Header("Animation")]
    [Tooltip("Có animate prompt lên xuống không?")]
    [SerializeField] private bool animateBounce = true;
    
    [Tooltip("Tốc độ bounce animation")]
    [SerializeField] private float bounceSpeed = 2f;
    
    [Tooltip("Độ cao bounce")]
    [SerializeField] private float bounceHeight = 0.1f;

    private Transform targetTransform; // NPC transform to follow
    private Vector3 initialLocalPosition;
    private bool isVisible = false;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Get or add CanvasGroup
        canvasGroup = promptPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = promptPanel.AddComponent<CanvasGroup>();
        }

        initialLocalPosition = promptPanel.transform.localPosition;

        // Set initial message
        if (promptText != null)
        {
            promptText.text = interactMessage;
        }

        // Hide by default
        Hide();
    }

    private void Update()
    {
        // Follow target if set
        if (isVisible && targetTransform != null)
        {
            UpdatePosition();
        }

        // Animate bounce
        if (isVisible && animateBounce)
        {
            AnimateBounce();
        }
    }

    /// <summary>
    /// Show prompt at target NPC position
    /// </summary>
    public void Show(Transform npcTransform)
    {
        targetTransform = npcTransform;
        isVisible = true;
        promptPanel.SetActive(true);
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        UpdatePosition();
    }

    /// <summary>
    /// Hide prompt
    /// </summary>
    public void Hide()
    {
        isVisible = false;
        promptPanel.SetActive(false);
        targetTransform = null;
    }

    /// <summary>
    /// Update prompt position to follow target
    /// </summary>
    private void UpdatePosition()
    {
        if (targetTransform == null)
            return;

        // Position prompt above NPC
        transform.position = targetTransform.position + offsetFromTarget;
    }

    /// <summary>
    /// Animate bounce effect
    /// </summary>
    private void AnimateBounce()
    {
        float bounce = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        promptPanel.transform.localPosition = initialLocalPosition + new Vector3(0, bounce, 0);
    }

    /// <summary>
    /// Set custom message
    /// </summary>
    public void SetMessage(string message)
    {
        interactMessage = message;
        if (promptText != null)
        {
            promptText.text = message;
        }
    }

    /// <summary>
    /// Set custom icon
    /// </summary>
    public void SetIcon(Sprite icon)
    {
        if (promptIcon != null)
        {
            promptIcon.sprite = icon;
            promptIcon.gameObject.SetActive(icon != null);
        }
    }
}
