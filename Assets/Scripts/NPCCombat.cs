using UnityEngine;

/// <summary>
/// Handles NPC combat behavior when player doesn't have mask
/// Attacks player continuously until mask is worn
/// </summary>
public class NPCCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [Tooltip("Damage per attack")]
    [SerializeField] private int attackDamage = 10;
    
    [Tooltip("Attack range")]
    [SerializeField] private float attackRange = 1.5f;
    
    [Tooltip("Time between attacks (seconds)")]
    [SerializeField] private float attackCooldown = 1f;
    
    [Tooltip("Should NPC chase player?")]
    [SerializeField] private bool chasePlayer = true;
    
    [Tooltip("Chase speed when player has no mask")]
    [SerializeField] private float chaseSpeed = 3f;

    [Header("Detection")]
    [Tooltip("Player tag for detection")]
    [SerializeField] private string playerTag = "Player";

    // State
    private bool isInCombatMode = false;
    private float attackTimer = 0f;
    private Transform playerTransform;
    private PlayerController playerController;

    // Original position (to return after combat)
    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (!isInCombatMode)
            return;

        if (playerController == null || !playerController.IsAlive())
        {
            ExitCombatMode();
            return;
        }

        // Check if player now has mask
        if (playerController.GetCurrentMaskType() != MaskType.NONE)
        {
            // Player equipped mask, stop combat
            ExitCombatMode();
            return;
        }

        // Update attack timer
        attackTimer -= Time.deltaTime;

        // Chase player if enabled
        if (chasePlayer && playerTransform != null)
        {
            ChasePlayer();
        }

        // Attack if in range
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= attackRange && attackTimer <= 0f)
        {
            AttackPlayer();
        }
    }

    /// <summary>
    /// Enter combat mode and start attacking player
    /// </summary>
    public void EnterCombatMode(PlayerController player)
    {
        if (player == null)
        {
            Debug.LogWarning("NPCCombat: Null player reference!");
            return;
        }

        isInCombatMode = true;
        playerController = player;
        playerTransform = player.transform;
        attackTimer = attackCooldown; // Set initial cooldown

        Debug.Log($"{gameObject.name}: Entering combat mode!");
    }

    /// <summary>
    /// Exit combat mode and stop attacking
    /// </summary>
    public void ExitCombatMode()
    {
        if (!isInCombatMode)
            return;

        isInCombatMode = false;
        playerController = null;
        playerTransform = null;

        Debug.Log($"{gameObject.name}: Exiting combat mode");
    }

    /// <summary>
    /// Chase player
    /// </summary>
    private void ChasePlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;

        // Face player
        if (direction.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    /// <summary>
    /// Attack player
    /// </summary>
    private void AttackPlayer()
    {
        if (playerController == null)
            return;

        playerController.TakeDamage(attackDamage);
        attackTimer = attackCooldown;

        Debug.Log($"{gameObject.name}: Attacked player for {attackDamage} damage!");

        // TODO: Play attack animation/sound
    }

    /// <summary>
    /// Return to original position
    /// </summary>
    public void ReturnToOriginalPosition()
    {
        // Smoothly return to original position
        transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * 2f);
    }

    // Getters
    public bool IsInCombatMode() => isInCombatMode;
    public float GetAttackRange() => attackRange;

    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
