using UnityEngine;

/// <summary>
/// Level manager for Midnight Walk
/// Tracks progress through the level and triggers ending when complete
/// </summary>
public class MidnightWalkManager : MonoBehaviour
{
    public static MidnightWalkManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Số NPC cần interact để kết thúc level")]
    [SerializeField] private int requiredNPCCount = 3;
    
    [Tooltip("Tự động trigger ending khi đủ NPC?")]
    [SerializeField] private bool autoTriggerEnding = true;

    [Header("References")]
    [Tooltip("List tất cả NPCs quan trọng trong level")]
    [SerializeField] private NPCController[] levelNPCs;

    // State
    private int npcInteractedCount = 0;
    private bool levelComplete = false;

    // Events
    public delegate void NPCInteracted(int count, int required);
    public event NPCInteracted OnNPCInteracted;

    public delegate void LevelCompleted();
    public event LevelCompleted OnLevelCompleted;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Subscribe to dialogue system events to track NPC interactions
        if (DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.OnDialogueEnded += OnDialogueEnded;
        }

        Debug.Log($"MidnightWalkManager: Level started. Need to interact with {requiredNPCCount} NPCs");
    }

    private void OnDestroy()
    {
        // Unsubscribe
        if (DialogueSystem.Instance != null)
        {
            DialogueSystem.Instance.OnDialogueEnded -= OnDialogueEnded;
        }
    }

    /// <summary>
    /// Called when any dialogue ends
    /// </summary>
    private void OnDialogueEnded()
    {
        // Check if this was a valid level NPC
        // Note: This is simplified. In production, you'd track which specific NPCs were interacted with
        RegisterNPCInteraction();
    }

    /// <summary>
    /// Register that player interacted with an NPC
    /// </summary>
    public void RegisterNPCInteraction()
    {
        if (levelComplete)
            return;

        npcInteractedCount++;
        OnNPCInteracted?.Invoke(npcInteractedCount, requiredNPCCount);

        Debug.Log($"MidnightWalkManager: NPC interactions: {npcInteractedCount}/{requiredNPCCount}");

        // Check if level complete
        if (npcInteractedCount >= requiredNPCCount)
        {
            CompleteLevel();
        }
    }

    /// <summary>
    /// Mark level as complete and trigger ending
    /// </summary>
    private void CompleteLevel()
    {
        if (levelComplete)
            return;

        levelComplete = true;
        OnLevelCompleted?.Invoke();

        Debug.Log("MidnightWalkManager: Level complete!");

        if (autoTriggerEnding)
        {
            if (EndingManager.Instance != null)
            {
                EndingManager.Instance.TriggerEnding();
            }
            else
            {
                Debug.LogError("MidnightWalkManager: EndingManager not found!");
            }
        }
    }

    // Getters
    public int GetNPCInteractedCount() => npcInteractedCount;
    public int GetRequiredNPCCount() => requiredNPCCount;
    public bool IsLevelComplete() => levelComplete;
    public float GetProgress() => (float)npcInteractedCount / requiredNPCCount;
}
