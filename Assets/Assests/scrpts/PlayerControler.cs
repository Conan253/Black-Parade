using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;   // optional new input support
#endif

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;                // horizontal speed (world units/sec)

    [Header("Jump")]
    public float jumpForce = 12f;               // try 10–14 for PPU=100
    public float coyoteTime = 0.1f;             // grace time after leaving ground
    public float jumpBufferTime = 0.1f;         // press jump slightly before landing
    public bool variableJumpHeight = true;      // release jump early to cut height

    [Header("Ground Check")]
    public Transform groundCheck;               // empty child under feet
    public float groundCheckRadius = 0.12f;     // ~0.1–0.2
    public LayerMask groundLayer;               // set to your Ground layer

    [Header("Audio")]
    public AudioClip jumpSfx;
    public AudioClip[] footstepSfx;             // drop multiple clips for variety
    [Range(0f,1f)] public float sfxVolume = 0.7f;
    public float stepInterval = 0.3f;           // time between footstep sounds

    [Header("Animator (optional)")]
    public Animator animator;                   // assign if you have one
    // expected parameters (optional): Float "Speed", Bool "isGrounded", Float "vy"

    [Header("Pixel Perfect (optional)")]
    public bool snapToPixelGrid = true;
    public float pixelsPerUnit = 100f;          // match your sprite PPU

    // ---- private ----
    Rigidbody2D rb;
    SpriteRenderer sr;

    float moveInput;                // -1, 0, 1
    bool isGrounded;
    float coyoteTimer;
    float jumpBufferTimer;
    float stepTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (!animator) animator = GetComponent<Animator>();

        // recommended Rigidbody2D setup
        rb.gravityScale = Mathf.Max(1f, rb.gravityScale);
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // --- input ---
        moveInput = Input.GetAxisRaw("Horizontal");     // old Input System

        bool jumpPressed = Input.GetButtonDown("Jump"); // Space by default
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpPressed = true;
#endif

        // --- sprite flip ---
        if (moveInput > 0.01f) sr.flipX = false;
        else if (moveInput < -0.01f) sr.flipX = true;

        // --- grounded check ---
        isGrounded = groundCheck
            ? Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer)
            : isGrounded;

        // --- timers (coyote & buffer) ---
        coyoteTimer = isGrounded ? coyoteTime : Mathf.Max(0, coyoteTimer - Time.deltaTime);
        if (jumpPressed) jumpBufferTimer = jumpBufferTime;
        else             jumpBufferTimer = Mathf.Max(0, jumpBufferTimer - Time.deltaTime);

        // --- attempt jump ---
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            DoJump();
            jumpBufferTimer = 0f;   // consume
            coyoteTimer = 0f;
        }

        // --- variable jump height ---
        if (variableJumpHeight && Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        // --- footsteps ---
        if (isGrounded && Mathf.Abs(moveInput) > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f) { PlayFootstep(); stepTimer = stepInterval; }
        }
        else stepTimer = 0f;

        // --- animator (optional) ---
        if (animator)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("isGrounded", isGrounded);
            animator.SetFloat("vy", rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        // horizontal movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // optional pixel snap on position (prevents sub-pixel jitter)
        if (snapToPixelGrid && pixelsPerUnit > 0f)
        {
            var p = transform.position;
            p.x = Mathf.Round(p.x * pixelsPerUnit) / pixelsPerUnit;
            transform.position = p;
        }
    }

    void DoJump()
    {
        // clear downward velocity for consistent jump
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (jumpSfx) AudioSource.PlayClipAtPoint(jumpSfx, transform.position, sfxVolume);
    }

    void PlayFootstep()
    {
        if (footstepSfx != null && footstepSfx.Length > 0)
        {
            var clip = footstepSfx[Random.Range(0, footstepSfx.Length)];
            AudioSource.PlayClipAtPoint(clip, transform.position, sfxVolume * 0.6f);
        }
    }

    // visualize ground check in Scene view
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
