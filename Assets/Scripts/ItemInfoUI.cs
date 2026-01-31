using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI Panel to display item story information when player holds an item
/// Shows at corner of screen with fade animation
/// </summary>
public class ItemInfoUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Panel chứa toàn bộ item info UI")]
    [SerializeField] private GameObject infoPanel;
    
    [Tooltip("Image hiển thị icon item")]
    [SerializeField] private Image itemIconImage;
    
    [Tooltip("Text hiển thị tên item")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    
    [Tooltip("Text hiển thị story title")]
    [SerializeField] private TextMeshProUGUI storyTitleText;
    
    [Tooltip("Text hiển thị story description")]
    [SerializeField] private TextMeshProUGUI storyDescriptionText;
    
    [Tooltip("(Optional) Button để xem full lore")]
    [SerializeField] private GameObject readMoreButton;

    [Header("Animation Settings")]
    [Tooltip("Fade duration khi show/hide")]
    [SerializeField] private float fadeDuration = 0.3f;
    
    [Tooltip("Slide distance khi animate")]
    [SerializeField] private float slideDistance = 50f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private ItemDropData currentItemData;

    private void Awake()
    {
        // Get or add CanvasGroup
        canvasGroup = infoPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = infoPanel.AddComponent<CanvasGroup>();
        }

        rectTransform = infoPanel.GetComponent<RectTransform>();
        
        // Calculate positions for slide animation
        if (rectTransform != null)
        {
            shownPosition = rectTransform.anchoredPosition;
            hiddenPosition = shownPosition + new Vector2(slideDistance, 0);
        }

        // Hide by default
        HideItemInfo();
    }

    /// <summary>
    /// Show item info panel with data
    /// </summary>
    public void ShowItemInfo(ItemDropData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("ItemInfoUI: Null item data!");
            return;
        }

        currentItemData = itemData;

        // Set item icon
        if (itemIconImage != null)
        {
            Sprite iconToUse = itemData.detailedIcon != null ? itemData.detailedIcon : itemData.itemIcon;
            if (iconToUse != null)
            {
                itemIconImage.sprite = iconToUse;
                itemIconImage.gameObject.SetActive(true);
            }
            else
            {
                itemIconImage.gameObject.SetActive(false);
            }
        }

        // Set item name
        if (itemNameText != null)
        {
            itemNameText.text = itemData.itemName;
        }

        // Set story title
        if (storyTitleText != null)
        {
            if (!string.IsNullOrEmpty(itemData.storyTitle))
            {
                storyTitleText.text = itemData.storyTitle;
                storyTitleText.gameObject.SetActive(true);
            }
            else
            {
                storyTitleText.gameObject.SetActive(false);
            }
        }

        // Set story description
        if (storyDescriptionText != null)
        {
            if (!string.IsNullOrEmpty(itemData.storyDescription))
            {
                storyDescriptionText.text = itemData.storyDescription;
                storyDescriptionText.gameObject.SetActive(true);
            }
            else
            {
                storyDescriptionText.gameObject.SetActive(false);
            }
        }

        // Show/hide read more button based on full lore
        if (readMoreButton != null)
        {
            readMoreButton.SetActive(!string.IsNullOrEmpty(itemData.fullLore));
        }

        // Animate in
        infoPanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimateShow());

        Debug.Log($"ItemInfoUI: Showing info for {itemData.itemName}");
    }

    /// <summary>
    /// Hide item info panel
    /// </summary>
    public void HideItemInfo()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateHide());
        currentItemData = null;
    }

    /// <summary>
    /// Animate show with fade and slide
    /// </summary>
    private System.Collections.IEnumerator AnimateShow()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Fade alpha
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            }

            // Slide position
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(hiddenPosition, shownPosition, t);
            }

            yield return null;
        }

        // Ensure final values
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
        if (rectTransform != null)
            rectTransform.anchoredPosition = shownPosition;
    }

    /// <summary>
    /// Animate hide with fade and slide
    /// </summary>
    private System.Collections.IEnumerator AnimateHide()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Fade alpha
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            }

            // Slide position
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(shownPosition, hiddenPosition, t);
            }

            yield return null;
        }

        // Ensure final values and hide panel
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
        if (rectTransform != null)
            rectTransform.anchoredPosition = hiddenPosition;
        
        infoPanel.SetActive(false);
    }

    /// <summary>
    /// Called when "Read More" button is clicked
    /// </summary>
    public void OnReadMoreClicked()
    {
        if (currentItemData != null && !string.IsNullOrEmpty(currentItemData.fullLore))
        {
            // TODO: Show full lore in expanded panel or separate window
            Debug.Log($"Full Lore:\n{currentItemData.fullLore}");
            
            // For now, just expand the description text to show full lore
            if (storyDescriptionText != null)
            {
                storyDescriptionText.text = currentItemData.fullLore;
            }
        }
    }
}
