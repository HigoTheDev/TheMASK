using UnityEngine;

/// <summary>
/// ScriptableObject containing 3 DialogueData assets - one for each mask type
/// Used by NPCs to provide different dialogue based on player's equipped mask
/// </summary>
[CreateAssetMenu(fileName = "NewMaskDialogueSet", menuName = "TheMASK/Mask Dialogue Set", order = 3)]
public class MaskDialogueSet : ScriptableObject
{
    [Header("Dialogue Variants")]
    [Tooltip("Dialogue khi player đeo Honesty Mask (Mask 1)")]
    public DialogueData honestyDialogue;
    
    [Tooltip("Dialogue khi player đeo Kindness Mask (Mask 2)")]
    public DialogueData kindnessDialogue;
    
    [Tooltip("Dialogue khi player đeo Indifference Mask (Mask 3)")]
    public DialogueData indifferenceDialogue;

    /// <summary>
    /// Get appropriate dialogue for given mask type
    /// </summary>
    public DialogueData GetDialogueForMask(MaskType maskType)
    {
        switch (maskType)
        {
            case MaskType.HONESTY:
                return honestyDialogue;
            
            case MaskType.KINDNESS:
                return kindnessDialogue;
            
            case MaskType.INDIFFERENCE:
                return indifferenceDialogue;
            
            default:
                Debug.LogWarning($"MaskDialogueSet: No dialogue for mask type {maskType}, returning honesty dialogue");
                return honestyDialogue;
        }
    }

    /// <summary>
    /// Validate that all dialogues are assigned
    /// </summary>
    public bool IsValid()
    {
        return honestyDialogue != null 
            && kindnessDialogue != null 
            && indifferenceDialogue != null;
    }

    /// <summary>
    /// Get dialogue count (should always be 3)
    /// </summary>
    public int GetDialogueCount()
    {
        int count = 0;
        if (honestyDialogue != null) count++;
        if (kindnessDialogue != null) count++;
        if (indifferenceDialogue != null) count++;
        return count;
    }
}
