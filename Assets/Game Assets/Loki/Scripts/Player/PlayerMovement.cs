
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private SpriteRenderer playerSprite;

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

    [Header("Slope Movement")]
    [SerializeField] private float slopeCheckDistance = 0.35f;
    [SerializeField] private float maxSlopeAngle = 50f;

    [Header("Sprint")]
    [SerializeField] private float sprintSpeedMultiplier = 1.7f;
    [SerializeField] private float sprintDuration = 2.5f;
    [SerializeField] private float sprintCooldown = 3f;

    [Header("Stair Movement")]
    private bool isInStairZone;
    private float stairSpeed;

    public bool IsInStairZone => isInStairZone;

    private Rigidbody2D rb;
    private float baseGravityScale;

    // Input
    private Vector2 moveInput;
    private bool sprintHeld;

    // Ground
    private bool isGrounded;
    private float coyoteCounter;
    private float jumpBufferCounter;

    // Slope
    private bool isOnSlope;
    private float slopeAngle;
    private Vector2 slopeNormal = Vector2.up;
    private Vector2 slopeTangent = Vector2.right;

    // Sprint
    private bool isSprinting;
    private bool sprintOnCooldown;
    private bool sprintLockedUntilRelease;

    private float sprintTimer;
    private float sprintCooldownTimer;

    public bool IsGrounded => isGrounded;
    public bool IsSprinting => isSprinting;
    public bool IsOnSlope => isOnSlope;

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
        CheckSlope();

        UpdateJumpBuffer();
        UpdateSprint();
    }

    private void FixedUpdate()
    {
        if(UIManager.Instance.isPanelOpen) return;

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

        if (!pressed)
            sprintLockedUntilRelease = false;
    }

    // ==========================================
    // MOVEMENT
    // ==========================================

    private void HandleMovement()
    {
        if (isInStairZone)
        {
            // A/D controls horizontal movement.
            // W/S controls vertical stair movement.

            rb.linearVelocity = new Vector2(
                moveInput.x * moveSpeed,
                moveInput.y * stairSpeed
            );

            if (playerAnimator != null)
            {
                playerAnimator.SetFloat(
                    "Speed",
                    moveInput.magnitude
                );
            }

            // Preserve sprite flipping.
            if (moveInput.x > 0)
                playerSprite.flipX = false;
            else if (moveInput.x < 0)
                playerSprite.flipX = true;

            return;
        }

        float targetSpeed = moveInput.x * moveSpeed;

        if (isSprinting)
            targetSpeed *= sprintSpeedMultiplier;

        // Slope movement
        if (isGrounded && isOnSlope)
        {
            if (Mathf.Abs(moveInput.x) > 0.01f)
            {
                // Move along the slope instead of horizontally.
                Vector2 slopeVelocity = slopeTangent * targetSpeed;

                rb.linearVelocity = slopeVelocity;
            }
            else
            {
                // Prevent sliding when standing still.
                rb.linearVelocity = Vector2.zero;
            }
        }
        else
        {
            // Normal horizontal movement
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

        // Animation
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat(
                "Speed",
                rb.linearVelocity.magnitude
            );
        }

        // Sprite direction
        if (moveInput.x > 0)
        {
            playerSprite.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            playerSprite.flipX = true;
        }
    }

    // ==========================================
    // GROUND CHECK
    // ==========================================

    private void CheckGround()
    {
        if (groundCheck == null)
            return;

        bool overlapGround = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayers
        );

        // Also check ground using a raycast.
        Vector2 origin = groundCheck.position + Vector3.up * 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.down,
            slopeCheckDistance,
            groundLayers
        );

        isGrounded = overlapGround || hit.collider != null;

        if (playerAnimator != null)
            playerAnimator.SetBool("Grounded", isGrounded);

        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter = Mathf.Max(
                0f,
                coyoteCounter - Time.deltaTime
            );
    }

    // ==========================================
    // SLOPE CHECK
    // ==========================================

    private void CheckSlope()
    {
        isOnSlope = false;
        slopeAngle = 0f;

        if (groundCheck == null || !isGrounded)
            return;

        Vector2 origin = groundCheck.position + Vector3.up * 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            Vector2.down,
            slopeCheckDistance,
            groundLayers
        );

        if (hit.collider == null)
            return;

        slopeNormal = hit.normal;

        slopeAngle = Vector2.Angle(
            slopeNormal,
            Vector2.up
        );

        if (slopeAngle > 0.1f && slopeAngle <= maxSlopeAngle)
        {
            isOnSlope = true;

            // Calculate the direction along the slope.
            slopeTangent = new Vector2(
                slopeNormal.y,
                -slopeNormal.x
            ).normalized;
        }
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

        jumpBufferCounter = 0f;
        coyoteCounter = 0f;

        isOnSlope = false;

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            0f
        );

        rb.AddForce(
            Vector2.up * jumpForce,
            ForceMode2D.Impulse
        );

        if (playerAnimator != null)
            playerAnimator.SetTrigger("Jump");
    }

    // ==========================================
    // BETTER GRAVITY
    // ==========================================

    private void HandleBetterGravity()
    {
        // Don't apply extra falling gravity while walking
        // down a slope.
        if (!isGrounded && rb.linearVelocity.y < 0f)
        {
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
        if (sprintOnCooldown)
        {
            sprintCooldownTimer -= Time.deltaTime;

            if (sprintCooldownTimer <= 0f)
            {
                sprintCooldownTimer = 0f;
                sprintOnCooldown = false;

                sprintTimer = sprintDuration;
            }

            isSprinting = false;
            return;
        }

        isSprinting =
            sprintHeld &&
            sprintTimer > 0f &&
            !sprintLockedUntilRelease;

        if (!isSprinting)
            return;

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

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(
            groundCheck.position + Vector3.up * 0.1f,
            groundCheck.position + Vector3.down * slopeCheckDistance
        );
    }

    public void EnterStairZone(float speed)
    {
        isInStairZone = true;
        stairSpeed = speed;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    public void ExitStairZone()
    {
        isInStairZone = false;

        rb.gravityScale = baseGravityScale;
        rb.linearVelocity = Vector2.zero;
    }
}
