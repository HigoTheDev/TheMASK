using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;

    // Components
    private Rigidbody2D rb;
    private Animator animator;

    // Animation parameter hashes
    private int isRunningHash;
    private int isJumpingHash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Cache parameter hashes
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
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

        // Flip sprite based on direction
        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleJump()
    {
        // Jump when space is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
}
