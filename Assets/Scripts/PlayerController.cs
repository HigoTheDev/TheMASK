using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;

    [Header("Mask System")]
    [SerializeField] private MaskController maskController;
    [SerializeField] private MaskUIController maskUIController; // UI controller reference
    [SerializeField] private GameObject maskVisual; // Visual representation of mask on player

    // Components
    private Rigidbody2D rb;
    private Animator animator;

    // Mask state
    private bool isMaskWorn = false;

    // Store initial scale to preserve size when flipping
    private Vector3 initialScale;

    // Animation parameter hashes
    private int isRunningHash;
    private int isJumpingHash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Store the initial scale (this preserves your inspector settings)
        initialScale = transform.localScale;

        // Cache parameter hashes
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");

        // Initialize mask visual as hidden
        if (maskVisual != null)
        {
            maskVisual.SetActive(false);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
        HandleMaskToggle();
        UpdateAnimations();
    }

    private void HandleMovement()
    {
        // Get input
        float moveInput = 0f;
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveInput = 1f;

        // Apply horizontal movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Flip sprite based on direction (preserve Y and Z scale)
        if (moveInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
    }

    private void HandleJump()
    {
        // Jump when space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void HandleMaskToggle()
    {
        // Toggle mask with E key
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleMask();
        }
    }

    private void ToggleMask()
    {
        isMaskWorn = !isMaskWorn;

        // Update visual
        if (maskVisual != null)
        {
            maskVisual.SetActive(isMaskWorn);
        }
        
        // Notify UI Controller
        if (maskUIController != null)
        {
            maskUIController.SetMaskWorn(isMaskWorn);
        }

        // Log for debugging
        if (maskController != null && maskController.HasMasks())
        {
            MaskData currentMask = maskController.GetCurrentMask();
            string status = isMaskWorn ? "worn" : "removed";
            Debug.Log($"Mask {status}: {currentMask.maskName}");
        }
        
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        // isRunning = true when moving horizontally
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool(isRunningHash, isMoving);

        // isJumping = true only when Y velocity is significant (actually jumping/falling)
        bool isInAir = Mathf.Abs(rb.linearVelocity.y) > 1.0f;
        animator.SetBool(isJumpingHash, isInAir);
    }

    // Public getters
    public bool IsMaskWorn() => isMaskWorn;
}
