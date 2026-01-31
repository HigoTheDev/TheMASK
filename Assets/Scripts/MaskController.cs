using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages mask inventory, selection, and switching
/// Press 1/2/3 to select different masks
/// </summary>
public class MaskController : MonoBehaviour
{
    [Header("Mask Inventory")]
    [SerializeField] private List<MaskData> availableMasks = new List<MaskData>();

    // Current state
    private int selectedMaskIndex = 0;
    private MaskData currentMask;

    // Events
    public delegate void MaskSelectionChanged(MaskData newMask, int index);
    public event MaskSelectionChanged OnMaskSelectionChanged;

    private void Awake()
    {
        // Initialize with first mask if available
        if (availableMasks.Count > 0)
        {
            currentMask = availableMasks[0];
        }
    }

    private void Update()
    {
        HandleMaskSelection();
    }

    private void HandleMaskSelection()
    {
        // Check for number key inputs (1, 2, 3, etc.)
        for (int i = 0; i < availableMasks.Count && i < 9; i++)
        {
            // KeyCode.Alpha1 = 49, Alpha2 = 50, etc.
            KeyCode key = KeyCode.Alpha1 + i;
            
            if (Input.GetKeyDown(key))
            {
                SelectMask(i);
            }
        }
    }

    public void SelectMask(int index)
    {
        if (index < 0 || index >= availableMasks.Count)
        {
            Debug.LogWarning($"Invalid mask index: {index}");
            return;
        }

        selectedMaskIndex = index;
        currentMask = availableMasks[index];

        // Notify listeners
        OnMaskSelectionChanged?.Invoke(currentMask, selectedMaskIndex);

        Debug.Log($"Mask selected: {currentMask.maskName} (Index: {selectedMaskIndex})");
    }

    public void AddMask(MaskData newMask)
    {
        if (!availableMasks.Contains(newMask))
        {
            availableMasks.Add(newMask);
            Debug.Log($"Mask added: {newMask.maskName}");
        }
    }

    public void RemoveMask(MaskData mask)
    {
        availableMasks.Remove(mask);
        
        // If current mask was removed, select first available
        if (currentMask == mask && availableMasks.Count > 0)
        {
            SelectMask(0);
        }
    }

    // Getters
    public MaskData GetCurrentMask() => currentMask;
    public int GetSelectedIndex() => selectedMaskIndex;
    public int GetMaskCount() => availableMasks.Count;
    public bool HasMasks() => availableMasks.Count > 0;
}

/// <summary>
/// Data container for mask properties
/// </summary>
[System.Serializable]
public class MaskData
{
    public string maskName = "Mask 1";
    public Sprite maskSprite;
    public Color maskColor = Color.white;
    
    [Header("World Effects")]
    [Tooltip("What changes when this mask is worn")]
    public string worldEffect = "Changes platforms visibility";
    
    // Add more properties as needed for different masks
    public int maskID = 0;
}
