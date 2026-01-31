using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Main NPC Controller
/// Handles player detection, interaction, dialogue triggering, and item drops
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class NPCController : MonoBehaviour
{
    [Header("NPC Data")]
    [Tooltip("Dialogue data cho NPC này")]
    [SerializeField] private DialogueData dialogueData;
    
    [Tooltip("Danh sách items có thể drop")]
    [SerializeField] private List<ItemDropData> itemDrops = new List<ItemDropData>();

    [Header("Interaction Settings")]
    [Tooltip("Layer của Player (để phát hiện player)")]
    [SerializeField] private LayerMask playerLayer;
    
    [Tooltip("Tag của Player (fallback nếu không dùng layer)")]
    [SerializeField] private string playerTag = "Player";
    
    [Tooltip("Phím để tương tác")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("References")]
    [Tooltip("Interaction prompt UI (sẽ tự tìm nếu không gán)")]
    [SerializeField] private InteractionPrompt interactionPrompt;

    [Header("Item Drop Settings")]
    [Tooltip("Drop items ngay sau khi dialogue kết thúc?")]
    [SerializeField] private bool dropItemsAfterDialogue = true;

    // State
    private bool playerInRange = false;
    private bool hasInteracted = false;
    private Transform playerTransform;

    private void Awake()
    {
        // Ensure collider is trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Find interaction prompt if not assigned
        if (interactionPrompt == null)
        {
            interactionPrompt = FindFirstObjectByType<InteractionPrompt>();
        }
    }

    private void Update()
    {
        HandleInteraction();
    }

    /// <summary>
    /// Handle player interaction input
    /// </summary>
    private void HandleInteraction()
    {
        // Only allow interaction if player in range and dialogue not active
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            // Don't interact if dialogue already active
            if (DialogueSystem.Instance != null && DialogueSystem.Instance.IsDialogueActive())
                return;

            StartInteraction();
        }
    }

    /// <summary>
    /// Start interaction with NPC
    /// </summary>
    private void StartInteraction()
    {
        // Check if can repeat dialogue
        if (hasInteracted && dialogueData != null && !dialogueData.canRepeat)
        {
            Debug.Log($"NPCController: {dialogueData.npcName} has already been talked to and cannot repeat");
            return;
        }

        // Start dialogue if available
        if (dialogueData != null && dialogueData.IsValid())
        {
            if (DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.StartDialogue(dialogueData, this);
                hasInteracted = true;

                // Hide interaction prompt during dialogue
                if (interactionPrompt != null)
                {
                    interactionPrompt.Hide();
                }
            }
            else
            {
                Debug.LogWarning("NPCController: DialogueSystem not found in scene!");
            }
        }
        else
        {
            Debug.LogWarning("NPCController: No valid dialogue data assigned!");
        }
    }

    /// <summary>
    /// Called by DialogueSystem when dialogue completes
    /// </summary>
    public void OnDialogueComplete()
    {
        Debug.Log($"NPCController: Dialogue completed for {gameObject.name}");

        // Drop items if enabled
        if (dropItemsAfterDialogue && itemDrops.Count > 0)
        {
            DropItems();
        }

        // Show prompt again if player still in range and can repeat
        if (playerInRange && dialogueData != null && dialogueData.canRepeat)
        {
            ShowInteractionPrompt();
        }
    }

    /// <summary>
    /// Drop items from this NPC
    /// </summary>
    private void DropItems()
    {
        if (ItemDropSystem.Instance != null)
        {
            ItemDropSystem.Instance.DropItems(transform.position, itemDrops);
        }
        else
        {
            Debug.LogWarning("NPCController: ItemDropSystem not found in scene!");
        }
    }

    /// <summary>
    /// Show interaction prompt
    /// </summary>
    private void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.Show(transform);
        }
    }

    /// <summary>
    /// Hide interaction prompt
    /// </summary>
    private void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.Hide();
        }
    }

    // Collision detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if it's the player
        if (IsPlayer(collision))
        {
            playerInRange = true;
            playerTransform = collision.transform;
            
            // Show prompt only if can interact
            if (!hasInteracted || (dialogueData != null && dialogueData.canRepeat))
            {
                ShowInteractionPrompt();
            }

            Debug.Log($"NPCController: Player entered range of {gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            playerInRange = false;
            playerTransform = null;
            HideInteractionPrompt();

            Debug.Log($"NPCController: Player left range of {gameObject.name}");
        }
    }

    /// <summary>
    /// Check if collider is the player
    /// </summary>
    private bool IsPlayer(Collider2D collision)
    {
        // Check by layer
        if (playerLayer != 0 && ((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            return true;
        }

        // Fallback: check by tag
        if (!string.IsNullOrEmpty(playerTag) && collision.CompareTag(playerTag))
        {
            return true;
        }

        return false;
    }

    // Public methods for external control
    public void SetDialogueData(DialogueData data) => dialogueData = data;
    public void AddItemDrop(ItemDropData item) => itemDrops.Add(item);
    public void ClearItemDrops() => itemDrops.Clear();
    public bool HasInteracted() => hasInteracted;
    public void ResetInteraction() => hasInteracted = false;
}
