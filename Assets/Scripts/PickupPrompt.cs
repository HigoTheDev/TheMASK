using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI prompt for pickup/drop actions
/// Shows "Press G to pickup" or "Press G to drop"
/// </summary>
public class PickupPrompt : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text hiển thị prompt message")]
    [SerializeField] private TextMeshProUGUI promptText;
    
    [Tooltip("Panel chứa prompt")]
    [SerializeField] private GameObject promptPanel;
    
    [Tooltip("Icon tùy chọn (ví dụ: icon phím G)")]
    [SerializeField] private Image promptIcon;

    [Header("Settings")]
    [Tooltip("Message khi có thể pickup")]
    [SerializeField] private string pickupMessage = "Press G to pickup";
    
    [Tooltip("Message khi đang cầm item (để drop)")]
    [SerializeField] private string dropMessage = "Press G to drop";
    
    [Tooltip("Offset từ target (WorldSpace)")]
    [SerializeField] private Vector3 offsetFromTarget = new Vector3(0, 1f, 0);

    [Header("Animation")]
    [Tooltip("Có bounce animation không?")]
    [SerializeField] private bool animateBounce = true;
    
    [Tooltip("Tốc độ bounce")]
    [SerializeField] private float bounceSpeed = 2f;
    
    [Tooltip("Độ cao bounce")]
    [SerializeField] private float bounceHeight = 0.1f;

    private Transform targetTransform;
    private Vector3 initialLocalPosition;
    private bool isVisible = false;
    private bool isDropMode = false;
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
        Hide();
    }

    private void Update()
    {
        if (isVisible && targetTransform != null)
        {
            UpdatePosition();
            
            if (animateBounce)
            {
                AnimateBounce();
            }
        }
    }

    /// <summary>
    /// Show pickup prompt at item position
    /// </summary>
    public void ShowPickupPrompt(Transform itemTransform)
    {
        targetTransform = itemTransform;
        isDropMode = false;
        isVisible = true;

        if (promptText != null)
        {
            promptText.text = pickupMessage;
        }

        promptPanel.SetActive(true);
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        UpdatePosition();
    }

    /// <summary>
    /// Show drop prompt at player position
    /// </summary>
    public void ShowDropPrompt(Transform playerTransform)
    {
        targetTransform = playerTransform;
        isDropMode = true;
        isVisible = true;

        if (promptText != null)
        {
            promptText.text = dropMessage;
        }

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
    /// Set custom pickup message
    /// </summary>
    public void SetPickupMessage(string message)
    {
        pickupMessage = message;
        if (!isDropMode && promptText != null)
        {
            promptText.text = message;
        }
    }

    /// <summary>
    /// Set custom drop message
    /// </summary>
    public void SetDropMessage(string message)
    {
        dropMessage = message;
        if (isDropMode && promptText != null)
        {
            promptText.text = message;
        }
    }
}
