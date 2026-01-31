using UnityEngine;
using System;

/// <summary>
/// Singleton manager for dialogue system
/// Handles dialogue progression and game state during dialogue
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;
    
    [Header("Settings")]
    [Tooltip("Có pause game khi dialogue đang active không?")]
    [SerializeField] private bool pauseGameDuringDialogue = true;

    // Current dialogue state
    private DialogueData currentDialogue;
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private NPCController currentNPC;

    // Events
    public event Action OnDialogueStart;
    public event Action OnDialogueEnd;
    public event Action<string, int, int> OnDialogueLineChanged; // line, current index, total lines

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

    private void Update()
    {
        // Progress dialogue when player presses key
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }

    /// <summary>
    /// Start a new dialogue
    /// </summary>
    public void StartDialogue(DialogueData dialogue, NPCController npc)
    {
        if (dialogue == null || !dialogue.IsValid())
        {
            Debug.LogWarning("DialogueSystem: Invalid dialogue data!");
            return;
        }

        if (isDialogueActive)
        {
            Debug.LogWarning("DialogueSystem: Dialogue already active!");
            return;
        }

        currentDialogue = dialogue;
        currentNPC = npc;
        currentLineIndex = 0;
        isDialogueActive = true;

        // Pause game if enabled
        if (pauseGameDuringDialogue)
        {
            Time.timeScale = 0f;
        }

        // Initialize UI
        if (dialogueUI != null)
        {
            dialogueUI.ShowDialogue(dialogue.npcName, dialogue.npcAvatar);
            DisplayCurrentLine();
        }

        OnDialogueStart?.Invoke();
        Debug.Log($"DialogueSystem: Started dialogue with {dialogue.npcName}");
    }

    /// <summary>
    /// Progress to next dialogue line or end dialogue
    /// </summary>
    public void NextLine()
    {
        if (!isDialogueActive || currentDialogue == null)
            return;

        currentLineIndex++;

        // Check if we've reached the end
        if (currentLineIndex >= currentDialogue.GetLineCount())
        {
            EndDialogue();
        }
        else
        {
            DisplayCurrentLine();
        }
    }

    /// <summary>
    /// Display current dialogue line
    /// </summary>
    private void DisplayCurrentLine()
    {
        if (currentDialogue == null || dialogueUI == null)
            return;

        string line = currentDialogue.GetLine(currentLineIndex);
        dialogueUI.SetDialogueText(line);

        OnDialogueLineChanged?.Invoke(line, currentLineIndex, currentDialogue.GetLineCount());
    }

    /// <summary>
    /// End the current dialogue
    /// </summary>
    public void EndDialogue()
    {
        if (!isDialogueActive)
            return;

        isDialogueActive = false;

        // Resume game
        if (pauseGameDuringDialogue)
        {
            Time.timeScale = 1f;
        }

        // Hide UI
        if (dialogueUI != null)
        {
            dialogueUI.HideDialogue();
        }

        // Notify NPC that dialogue ended
        if (currentNPC != null)
        {
            currentNPC.OnDialogueComplete();
        }

        OnDialogueEnd?.Invoke();
        Debug.Log("DialogueSystem: Dialogue ended");

        currentDialogue = null;
        currentNPC = null;
        currentLineIndex = 0;
    }

    // Public getters
    public bool IsDialogueActive() => isDialogueActive;
    public DialogueData GetCurrentDialogue() => currentDialogue;
    public int GetCurrentLineIndex() => currentLineIndex;
}
