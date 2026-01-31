using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Singleton system to track player's mask choices throughout Midnight Walk level
/// Records which mask was used with each NPC and calculates ending
/// </summary>
public class MaskChoiceTracker : MonoBehaviour
{
    public static MaskChoiceTracker Instance { get; private set; }

    // Track mask choice for each NPC
    private Dictionary<string, MaskType> npcChoices = new Dictionary<string, MaskType>();

    // Events
    public delegate void ChoiceRecorded(string npcID, MaskType maskType);
    public event ChoiceRecorded OnChoiceRecorded;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Record mask choice for an NPC
    /// </summary>
    public void RecordChoice(string npcID, MaskType maskType)
    {
        if (string.IsNullOrEmpty(npcID))
        {
            Debug.LogWarning("MaskChoiceTracker: Invalid NPC ID!");
            return;
        }

        // Record or update choice
        if (npcChoices.ContainsKey(npcID))
        {
            Debug.LogWarning($"MaskChoiceTracker: Overwriting choice for {npcID}");
            npcChoices[npcID] = maskType;
        }
        else
        {
            npcChoices.Add(npcID, maskType);
        }

        OnChoiceRecorded?.Invoke(npcID, maskType);
        Debug.Log($"MaskChoiceTracker: Recorded {maskType} for {npcID}");
    }

    /// <summary>
    /// Get mask choice for specific NPC
    /// </summary>
    public MaskType GetChoice(string npcID)
    {
        if (npcChoices.ContainsKey(npcID))
        {
            return npcChoices[npcID];
        }
        return MaskType.NONE;
    }

    /// <summary>
    /// Check if choice was recorded for NPC
    /// </summary>
    public bool HasChoice(string npcID)
    {
        return npcChoices.ContainsKey(npcID);
    }

    /// <summary>
    /// Count how many times a specific mask was used
    /// </summary>
    public int GetMaskCount(MaskType maskType)
    {
        int count = 0;
        foreach (var choice in npcChoices.Values)
        {
            if (choice == maskType)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Get total number of choices made
    /// </summary>
    public int GetTotalChoices()
    {
        return npcChoices.Count;
    }

    /// <summary>
    /// Calculate ending based on choices
    /// Good: 2+ KIND
    /// Bad: 2+ INDIFFERENT
    /// Neutral: Everything else (balanced or 2 HONEST)
    /// </summary>
    public EndingType CalculateEnding()
    {
        int kindCount = GetMaskCount(MaskType.KINDNESS);
        int indiffCount = GetMaskCount(MaskType.INDIFFERENCE);
        int honestCount = GetMaskCount(MaskType.HONESTY);

        // Good ending: 2 or more KIND
        if (kindCount >= 2)
        {
            return EndingType.GOOD;
        }

        // Bad ending: 2 or more INDIFFERENT
        if (indiffCount >= 2)
        {
            return EndingType.BAD;
        }

        // Neutral: everything else (balanced or 2 HONEST)
        return EndingType.NEUTRAL;
    }

    /// <summary>
    /// Reset all choices (for replay)
    /// </summary>
    public void ResetChoices()
    {
        npcChoices.Clear();
        Debug.Log("MaskChoiceTracker: Choices reset");
    }

    /// <summary>
    /// Get all choices for debugging
    /// </summary>
    public Dictionary<string, MaskType> GetAllChoices()
    {
        return new Dictionary<string, MaskType>(npcChoices);
    }

    /// <summary>
    /// Debug log current state
    /// </summary>
    public void LogCurrentState()
    {
        Debug.Log("=== Mask Choice Tracker State ===");
        Debug.Log($"Total choices: {GetTotalChoices()}");
        Debug.Log($"HONESTY: {GetMaskCount(MaskType.HONESTY)}");
        Debug.Log($"KINDNESS: {GetMaskCount(MaskType.KINDNESS)}");
        Debug.Log($"INDIFFERENCE: {GetMaskCount(MaskType.INDIFFERENCE)}");
        Debug.Log($"Predicted ending: {CalculateEnding()}");
        Debug.Log("=================================");
    }
}

/// <summary>
/// Enum for ending types
/// </summary>
public enum EndingType
{
    GOOD,       // 2+ KIND
    NEUTRAL,    // Balanced or 2 HONEST
    BAD         // 2+ INDIFFERENT
}
