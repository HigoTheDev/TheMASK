using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages mask selection and properties
/// Press 1/2/3 to switch between different colored masks
/// Each mask color can have unique world-altering properties
/// </summary>
public class MaskController : MonoBehaviour
{
    [Header("Mask Colors")]
    [SerializeField] private List<MaskData> masks = new List<MaskData>();

    [Header("Visual Reference")]
    [SerializeField] private SpriteRenderer maskVisualRenderer;

    // Current state
    private int selectedMaskIndex = 0;
    private MaskData currentMask;

    // Events for other systems to react to mask changes
    public delegate void MaskChanged(MaskData newMask, int index);
    public event MaskChanged OnMaskChanged;

    private void Awake()
    {
        // Initialize with first mask if available
        if (masks.Count > 0)
        {
            currentMask = masks[0];
            ApplyMaskVisual();
        }
        else
        {
            Debug.LogWarning("MaskController: No masks configured!");
        }
    }

    private void Update()
    {
        HandleMaskSelection();
    }

    private void HandleMaskSelection()
    {
        // Listen for 1, 2, 3 keys to switch masks
        if (Input.GetKeyDown(KeyCode.Alpha1) && masks.Count > 0)
        {
            SelectMask(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && masks.Count > 1)
        {
            SelectMask(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && masks.Count > 2)
        {
            SelectMask(2);
        }
    }

    public void SelectMask(int index)
    {
        if (index < 0 || index >= masks.Count)
        {
            Debug.LogWarning($"Invalid mask index: {index}");
            return;
        }

        selectedMaskIndex = index;
        currentMask = masks[index];

        ApplyMaskVisual();

        // Notify other systems
        OnMaskChanged?.Invoke(currentMask, selectedMaskIndex);

        Debug.Log($"Selected: {currentMask.maskName} (Color: {currentMask.maskColor})");
    }

    private void ApplyMaskVisual()
    {
        if (maskVisualRenderer != null && currentMask != null)
        {
            maskVisualRenderer.color = currentMask.maskColor;
        }
    }

    // Public methods
    public MaskData GetCurrentMask() => currentMask;
    public int GetSelectedIndex() => selectedMaskIndex;
    public int GetMaskCount() => masks.Count;
    public bool HasMasks() => masks.Count > 0;
    
    // Get mask data by index (useful for UI)
    public MaskData GetMaskByIndex(int index)
    {
        if (index >= 0 && index < masks.Count)
        {
            return masks[index];
        }
        return null;
    }
    
    // For future: Get mask-specific properties
    public T GetMaskProperty<T>(string propertyName) where T : class
    {
        // Extensible for future custom properties per mask
        return null;
    }
}

/// <summary>
/// Data container for each mask type
/// Each mask is represented by a color and can have unique properties
/// </summary>
[System.Serializable]
public class MaskData
{
    [Header("Basic Info")]
    public string maskName = "Mask 1";
    public Color maskColor = Color.white;
    public int maskID = 0;

    [Header("Future Properties")]
    [Tooltip("Description of what this mask does")]
    public string maskDescription = "Default mask";
    
    // Extensible properties for future features
    // Example: Different platform visibility rules, special abilities, etc.
    
    // Uncomment and expand as needed:
    // public bool affectsPlatforms = true;
    // public bool affectsEnemies = false;
    // public float speedModifier = 1.0f;
    // public GameObject particleEffect;
}
