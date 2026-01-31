using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject to store dialogue data for NPCs
/// Create via: Right-click in Project > Create > TheMASK > Dialogue Data
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "TheMASK/Dialogue Data", order = 1)]
public class DialogueData : ScriptableObject
{
    [Header("NPC Information")]
    [Tooltip("Tên của NPC (hiển thị trong dialogue)")]
    public string npcName = "NPC";
    
    [Tooltip("Avatar/Portrait của NPC (tùy chọn)")]
    public Sprite npcAvatar;

    [Header("Dialogue Content")]
    [Tooltip("Danh sách các câu thoại. Mỗi dòng là 1 câu.")]
    [TextArea(3, 10)]
    public List<string> dialogueLines = new List<string>();

    [Header("Settings")]
    [Tooltip("Có thể tương tác lại với NPC sau khi dialogue kết thúc?")]
    public bool canRepeat = true;
    
    [Tooltip("Khoảng thời gian chờ giữa các câu thoại tự động (0 = phải click)")]
    public float autoProgressDelay = 0f;

    /// <summary>
    /// Validate dialogue data
    /// </summary>
    public bool IsValid()
    {
        return dialogueLines != null && dialogueLines.Count > 0 && !string.IsNullOrEmpty(npcName);
    }

    /// <summary>
    /// Get total number of dialogue lines
    /// </summary>
    public int GetLineCount()
    {
        return dialogueLines != null ? dialogueLines.Count : 0;
    }

    /// <summary>
    /// Get specific dialogue line by index
    /// </summary>
    public string GetLine(int index)
    {
        if (dialogueLines != null && index >= 0 && index < dialogueLines.Count)
        {
            return dialogueLines[index];
        }
        return string.Empty;
    }
}
