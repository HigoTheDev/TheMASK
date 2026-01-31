using UnityEngine;

/// <summary>
/// Singleton manager for determining and triggering endings
/// based on player's mask choices throughout the level
/// </summary>
public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("Ending UI component")]
    [SerializeField] private EndingUI endingUI;

    [Header("Settings")]
    [Tooltip("Disable player controls when showing ending?")]
    [SerializeField] private bool disablePlayerOnEnding = true;

    private bool endingTriggered = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Find EndingUI if not assigned
        if (endingUI == null)
        {
            endingUI = FindFirstObjectByType<EndingUI>();
        }
    }

    /// <summary>
    /// Trigger ending sequence
    /// Calculates ending based on MaskChoiceTracker
    /// </summary>
    public void TriggerEnding()
    {
        if (endingTriggered)
        {
            Debug.LogWarning("EndingManager: Ending already triggered!");
            return;
        }

        if (MaskChoiceTracker.Instance == null)
        {
            Debug.LogError("EndingManager: MaskChoiceTracker not found!");
            return;
        }

        // Calculate ending
        EndingType ending = MaskChoiceTracker.Instance.CalculateEnding();
        
        // Log choices for debugging
        MaskChoiceTracker.Instance.LogCurrentState();

        // Disable player if needed
        if (disablePlayerOnEnding)
        {
            DisablePlayer();
        }

        // Show ending
        if (endingUI != null)
        {
            endingUI.ShowEnding(ending);
            endingTriggered = true;
        }
        else
        {
            Debug.LogError("EndingManager: EndingUI not found!");
        }
    }

    /// <summary>
    /// Disable player controls
    /// </summary>
    private void DisablePlayer()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.enabled = false;
            Debug.Log("EndingManager: Player controls disabled");
        }
    }

    /// <summary>
    /// Check if ending has been triggered
    /// </summary>
    public bool IsEndingTriggered() => endingTriggered;
}
