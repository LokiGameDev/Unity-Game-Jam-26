
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform groundCheck;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.12f;

    [Header("Gravity")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeedMultiplier = 1.7f;
    [SerializeField] private float sprintDuration = 2.5f;
    [SerializeField] private float sprintCooldown = 3f;

    private Rigidbody2D rb;
    private float baseGravityScale;

    // Input
    private Vector2 moveInput;
    private bool sprintHeld;

    // Ground
    private bool isGrounded;
    private float coyoteCounter;
    private float jumpBufferCounter;

    // Sprint
    private bool isSprinting;
    private bool sprintOnCooldown;
    private bool sprintLockedUntilRelease;

    private float sprintTimer;
    private float sprintCooldownTimer;

    // Public properties
    public bool IsGrounded => isGrounded;
    public bool IsSprinting => isSprinting;

    public float SprintRemaining => sprintTimer;
    public float SprintCooldownRemaining => sprintCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        baseGravityScale = rb.gravityScale;
        sprintTimer = sprintDuration;
    }

    private void OnEnable()
    {
        if (inputReader == null)
            return;

        inputReader.MoveEvent += HandleMoveInput;
        inputReader.JumpEvent += HandleJumpInput;
        inputReader.SprintEvent += HandleSprintInput;
    }

    private void OnDisable()
    {
        if (inputReader == null)
            return;

        inputReader.MoveEvent -= HandleMoveInput;
        inputReader.JumpEvent -= HandleJumpInput;
        inputReader.SprintEvent -= HandleSprintInput;
    }

    private void Update()
    {
        CheckGround();
        UpdateJumpBuffer();
        UpdateSprint();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        TryPerformJump();
        HandleBetterGravity();
    }

    // ==========================================
    // INPUT
    // ==========================================

    private void HandleMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void HandleJumpInput()
    {
        jumpBufferCounter = jumpBufferTime;
    }

    private void HandleSprintInput(bool pressed)
    {
        sprintHeld = pressed;

        // Player must release sprint before
        // being allowed to sprint again.

        if (!pressed)
            sprintLockedUntilRelease = false;
    }

    // ==========================================
    // MOVEMENT
    // ==========================================

    private void HandleMovement()
    {
        float targetSpeed = moveInput.x * moveSpeed;

        if (isSprinting)
            targetSpeed *= sprintSpeedMultiplier;

        float currentSpeed = rb.linearVelocity.x;

        float rate = Mathf.Abs(moveInput.x) > 0.01f
            ? acceleration
            : deceleration;

        float newSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            rate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(
            newSpeed,
            rb.linearVelocity.y
        );
    }

    // ==========================================
    // GROUND CHECK
    // ==========================================

    private void CheckGround()
    {
        if (groundCheck == null)
            return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayers
        );

        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;
    }

    // ==========================================
    // JUMP
    // ==========================================

    private void UpdateJumpBuffer()
    {
        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;
    }

    private void TryPerformJump()
    {
        if (jumpBufferCounter <= 0f)
            return;

        if (coyoteCounter <= 0f)
            return;

        // Consume jump.
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;

        // Reset existing vertical velocity.
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            0f
        );

        // Apply instant jump force.
        rb.AddForce(
            Vector2.up * jumpForce,
            ForceMode2D.Impulse
        );
    }

    // ==========================================
    // BETTER GRAVITY
    // ==========================================

    private void HandleBetterGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            // Faster falling for a snappy jump.
            rb.gravityScale = baseGravityScale * fallMultiplier;
        }
        else
        {
            rb.gravityScale = baseGravityScale;
        }
    }

    // ==========================================
    // SPRINT
    // ==========================================

    private void UpdateSprint()
    {
        // Cooldown handling.
        if (sprintOnCooldown)
        {
            sprintCooldownTimer -= Time.deltaTime;

            if (sprintCooldownTimer <= 0f)
            {
                sprintCooldownTimer = 0f;
                sprintOnCooldown = false;

                // Refill stamina.
                sprintTimer = sprintDuration;
            }

            isSprinting = false;
            return;
        }

        // Check whether sprint is allowed.
        isSprinting =
            sprintHeld &&
            sprintTimer > 0f &&
            !sprintLockedUntilRelease;

        if (!isSprinting)
            return;

        // Consume sprint stamina.
        sprintTimer -= Time.deltaTime;

        if (sprintTimer <= 0f)
        {
            sprintTimer = 0f;
            isSprinting = false;

            StartSprintCooldown();
        }
    }

    private void StartSprintCooldown()
    {
        sprintOnCooldown = true;
        sprintCooldownTimer = sprintCooldown;

        // Prevent holding the button from
        // automatically restarting sprint.
        sprintLockedUntilRelease = true;
    }

    // ==========================================
    // DEBUG
    // ==========================================

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}
