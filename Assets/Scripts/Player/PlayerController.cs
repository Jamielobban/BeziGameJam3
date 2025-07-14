using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public float airControlFactor = 0.3f;
    public float decelerationFactor = 0.2f;
    public float airDecelerationFactor = 0.1f;
    public float maxAirSpeedMult = 0f;

    public float fallMultiplier;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public Vector2 groundCheckOffset = Vector2.zero;

    [Header("Coyote Time")]
    public float coyoteTimeDuration = 0.3f;

    [Header("Feedbacks")]
    public MMF_Player jumpFeedbacks;
    public MMF_Player landFeedbacks;

    [Header("Debug")]
    public Transform groundCheckVisual;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool wasGroundedLastFrame = true;
    private bool hasJumpedThisFrame = false;
    private float coyoteTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //rb.gravityScale = 1f; // Use Unity's built-in gravity
    }

    private void FixedUpdate()
    {
        bool isGrounded = CheckIfGrounded();

        if (isGrounded)
        {
            coyoteTimer = coyoteTimeDuration;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
        }

        if (!wasGroundedLastFrame && isGrounded)
        {
            landFeedbacks?.PlayFeedbacks();
            hasJumpedThisFrame = false;
        }

        wasGroundedLastFrame = isGrounded;

        ApplyMovement();

        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }

        if (groundCheckVisual != null)
        {
            Vector2 origin = (Vector2)transform.position + groundCheckOffset;
            groundCheckVisual.position = origin + Vector2.down * groundCheckDistance;
            groundCheckVisual.rotation = Quaternion.identity;
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void ApplyMovement()
    {
        // Gravity
        Vector2 gravityForce = Physics2D.gravity * rb.mass;
        if (Vector2.Dot(rb.linearVelocity, Physics2D.gravity.normalized) > 0)
        {
            gravityForce *= fallMultiplier;
        }
        rb.AddForce(gravityForce, ForceMode2D.Force);

        // Directional vectors
        Vector2 gravityDir = Physics2D.gravity.normalized;
        Vector2 rightDir = new Vector2(-gravityDir.y, gravityDir.x).normalized;

        // Reproject velocity into new gravity/right space (Option 2)
        Vector2 currentVel = rb.linearVelocity;
        float verticalComponent = Vector2.Dot(currentVel, gravityDir);
        float horizontalComponent = Vector2.Dot(currentVel, rightDir);

        // Clamp horizontal speed to avoid runaway buildup
        horizontalComponent = Mathf.Clamp(horizontalComponent, -moveSpeed, moveSpeed);

        // Reconstruct velocity based on current orientation
        rb.linearVelocity = gravityDir * verticalComponent + rightDir * horizontalComponent;

        // Movement input
        float horizontalSpeed = moveSpeed * moveInput.x;

        Vector2 currentHorizontalVel = Vector2.Dot(rb.linearVelocity, rightDir) * rightDir;
        Vector2 targetHorizontalVel = rightDir * horizontalSpeed;
        Vector2 verticalVel = Vector2.Dot(rb.linearVelocity, gravityDir) * gravityDir;

        if (wasGroundedLastFrame)
        {
            Vector2 finalVel = Vector2.Lerp(currentHorizontalVel, targetHorizontalVel, 1f);
            rb.linearVelocity = finalVel + verticalVel;
        }
        else
        {
            Vector2 airAccel = (targetHorizontalVel - currentHorizontalVel) * airControlFactor;

            if (!Mathf.Approximately(moveInput.x, 0f))
            {
                rb.AddForce(airAccel, ForceMode2D.Force);

                float maxAirSpeed = moveSpeed * maxAirSpeedMult;
                Vector2 newHorizontalVel = Vector2.Dot(rb.linearVelocity, rightDir) * rightDir;
                if (newHorizontalVel.magnitude > maxAirSpeed)
                {
                    newHorizontalVel = newHorizontalVel.normalized * maxAirSpeed;
                    rb.linearVelocity = newHorizontalVel + verticalVel;
                }
            }
            else
            {
                Vector2 decel = Vector2.Lerp(currentHorizontalVel, Vector2.zero, airDecelerationFactor);
                rb.linearVelocity = decel + verticalVel;
            }
        }

        if (wasGroundedLastFrame && Mathf.Approximately(moveInput.x, 0f))
        {
            Vector2 decel = Vector2.Lerp(currentHorizontalVel, Vector2.zero, decelerationFactor);
            rb.linearVelocity = decel + verticalVel;
        }
    }

    public void Jump()
    {
        if (coyoteTimer > 0f && !hasJumpedThisFrame)
        {
            jumpFeedbacks?.PlayFeedbacks();
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            coyoteTimer = 0f;
            hasJumpedThisFrame = true;
        }
    }

    private bool CheckIfGrounded()
    {
        Vector2 origin = (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EdgeTile tile = other.GetComponentInParent<EdgeTile>();
        if (tile != null)
        {
            tile.Activate();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 origin = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);
    }
}
