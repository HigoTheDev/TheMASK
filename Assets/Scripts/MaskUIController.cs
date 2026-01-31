using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Manages the UI display for masks in the corner of the screen
/// Shows three mask icons with matching colors and dims the active one
/// </summary>
public class MaskUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform maskUIContainer;
    [SerializeField] private Image[] maskUISlots = new Image[3]; // UI images for each mask
    
    [Header("Settings")]
    [SerializeField] private float activeDimAmount = 0.4f; // How much to dim the active mask (0-1)
    [SerializeField] private float inactiveBrightness = 1f; // Brightness for inactive masks
    [SerializeField] private bool useAlphaForDim = true; // Use alpha channel for dimming
    
    [Header("Animation")]
    [SerializeField] private bool enableSwitchAnimation = true;
    [SerializeField] private float switchAnimationSpeed = 5f;
    
    private MaskController maskController;
    private int currentActiveMaskIndex = 0;
    private Color[] originalMaskColors = new Color[3];
    
    // Target colors for smooth animation
    private Color[] targetColors = new Color[3];
    
    private void Awake()
    {
        // Find MaskController
        maskController = FindObjectOfType<MaskController>();
        
        if (maskController == null)
        {
            Debug.LogError("MaskUIController: MaskController not found in scene!");
            return;
        }
        
        // Validate UI slots
        ValidateUISlots();
    }
    
    private void Start()
    {
        // Subscribe to mask change events
        if (maskController != null)
        {
            maskController.OnMaskChanged += OnMaskSwitched;
        }
        
        // Initialize UI with mask colors
        InitializeMaskUI();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (maskController != null)
        {
            maskController.OnMaskChanged -= OnMaskSwitched;
        }
    }
    
    private void Update()
    {
        // Smooth animation for color transitions
        if (enableSwitchAnimation)
        {
            AnimateMaskColors();
        }
    }
    
    private void ValidateUISlots()
    {
        // Check if all UI slots are assigned
        for (int i = 0; i < maskUISlots.Length; i++)
        {
            if (maskUISlots[i] == null)
            {
                Debug.LogWarning($"MaskUIController: Mask UI slot {i} is not assigned!");
            }
        }
    }
    
    private void InitializeMaskUI()
    {
        if (maskController == null || !maskController.HasMasks())
        {
            Debug.LogWarning("MaskUIController: No masks available to display");
            return;
        }
        
        int maskCount = Mathf.Min(maskController.GetMaskCount(), maskUISlots.Length);
        
        // Initialize each mask UI slot
        for (int i = 0; i < maskCount; i++)
        {
            if (maskUISlots[i] != null)
            {
                // Get the mask color from MaskController
                MaskData maskData = GetMaskDataByIndex(i);
                
                if (maskData != null)
                {
                    originalMaskColors[i] = maskData.maskColor;
                    
                    // Set initial color (dim if it's the active mask)
                    if (i == currentActiveMaskIndex)
                    {
                        targetColors[i] = GetDimmedColor(originalMaskColors[i]);
                    }
                    else
                    {
                        targetColors[i] = GetBrightColor(originalMaskColors[i]);
                    }
                    
                    maskUISlots[i].color = targetColors[i];
                    maskUISlots[i].gameObject.SetActive(true);
                }
            }
        }
        
        // Hide unused slots
        for (int i = maskCount; i < maskUISlots.Length; i++)
        {
            if (maskUISlots[i] != null)
            {
                maskUISlots[i].gameObject.SetActive(false);
            }
        }
    }
    
    private void OnMaskSwitched(MaskData newMask, int index)
    {
        // Update the active mask index
        int previousIndex = currentActiveMaskIndex;
        currentActiveMaskIndex = index;
        
        // Update UI colors
        UpdateMaskUI(previousIndex, currentActiveMaskIndex);
    }
    
    private void UpdateMaskUI(int previousIndex, int newIndex)
    {
        // Brighten the previous mask
        if (previousIndex >= 0 && previousIndex < maskUISlots.Length && maskUISlots[previousIndex] != null)
        {
            targetColors[previousIndex] = GetBrightColor(originalMaskColors[previousIndex]);
            
            if (!enableSwitchAnimation)
            {
                maskUISlots[previousIndex].color = targetColors[previousIndex];
            }
        }
        
        // Dim the new active mask
        if (newIndex >= 0 && newIndex < maskUISlots.Length && maskUISlots[newIndex] != null)
        {
            targetColors[newIndex] = GetDimmedColor(originalMaskColors[newIndex]);
            
            if (!enableSwitchAnimation)
            {
                maskUISlots[newIndex].color = targetColors[newIndex];
            }
        }
    }
    
    private void AnimateMaskColors()
    {
        // Smoothly interpolate each mask's color to its target
        for (int i = 0; i < maskUISlots.Length; i++)
        {
            if (maskUISlots[i] != null && maskUISlots[i].gameObject.activeSelf)
            {
                maskUISlots[i].color = Color.Lerp(
                    maskUISlots[i].color,
                    targetColors[i],
                    Time.deltaTime * switchAnimationSpeed
                );
            }
        }
    }
    
    private Color GetDimmedColor(Color baseColor)
    {
        if (useAlphaForDim)
        {
            // Dim using alpha channel
            return new Color(baseColor.r, baseColor.g, baseColor.b, activeDimAmount);
        }
        else
        {
            // Dim by reducing RGB values
            return baseColor * activeDimAmount;
        }
    }
    
    private Color GetBrightColor(Color baseColor)
    {
        if (useAlphaForDim)
        {
            // Full brightness with full alpha
            return new Color(baseColor.r, baseColor.g, baseColor.b, inactiveBrightness);
        }
        else
        {
            // Full brightness
            return baseColor * inactiveBrightness;
        }
    }
    
    // Helper method to get mask data by index
    private MaskData GetMaskDataByIndex(int index)
    {
        return maskController.GetMaskByIndex(index);
    }
    
    // Public method to manually refresh the UI
    public void RefreshMaskUI()
    {
        InitializeMaskUI();
    }
    
    // Public method to set a specific mask as active (for testing)
    public void SetActiveMask(int index)
    {
        if (index >= 0 && index < maskUISlots.Length)
        {
            UpdateMaskUI(currentActiveMaskIndex, index);
            currentActiveMaskIndex = index;
        }
    }
}
