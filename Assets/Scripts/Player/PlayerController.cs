using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public float fallMultiplier = 2f;
    public float airControlFactor = 0.3f;
    public float decelerationFactor = 0.2f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public Vector2 groundCheckOffset = Vector2.zero;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float currentRotation;

    public MMF_Player jumpFeedbacks;
    public MMF_Player landFeedbacks;

    private bool wasGroundedLastFrame = true;

    public float maxAirSpeedMult = 0f;

    [Header("Coyote Time")]
    public float coyoteTimeDuration = 0.3f;
    private float coyoteTimer = 0f;
    private bool hasJumpedThisFrame = false;
    [Header("Air Settings")]
    public float airDecelerationFactor = 0.1f; // tweakable in inspector

    [Header("Debug / Ground Check Helper")]
    public Transform groundCheckVisual;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;  // Disable built-in gravity
    }

    private void FixedUpdate()
    {
        bool isActuallyGrounded = CheckIfGrounded();

        if (isActuallyGrounded)
        {
            coyoteTimer = coyoteTimeDuration;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
        }

        if (!wasGroundedLastFrame && isActuallyGrounded)
        {
            landFeedbacks?.PlayFeedbacks();
            hasJumpedThisFrame = false;
        }

        wasGroundedLastFrame = isActuallyGrounded;

        ApplyMovement();

        if (groundCheckVisual != null)
        {
            Vector2 gravityDir = Physics2D.gravity.normalized;
            Vector2 rotatedOffset = Quaternion.Euler(0, 0, currentRotation) * groundCheckOffset;
            Vector2 origin = (Vector2)transform.position + rotatedOffset;

            groundCheckVisual.position = origin + gravityDir * groundCheckDistance;

            float angle = Mathf.Atan2(gravityDir.y, gravityDir.x) * Mathf.Rad2Deg;
            groundCheckVisual.rotation = Quaternion.Euler(0, 0, angle + 90f); 
        }
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void SetRotation(float rotation)
    {
        currentRotation = rotation;
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
            Vector2 jumpDirection = -Physics2D.gravity.normalized;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
            coyoteTimer = 0f;
            hasJumpedThisFrame = true; // prevent another jump in the same frame
        }
    }

    private bool CheckIfGrounded()
    {
        Vector2 rotatedOffset = Quaternion.Euler(0, 0, currentRotation) * groundCheckOffset;
        Vector2 origin = (Vector2)transform.position + rotatedOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Physics2D.gravity.normalized, groundCheckDistance, groundLayer);
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
        float rotationAngle = Application.isPlaying ? currentRotation : transform.eulerAngles.z;

        Vector2 gravityDir = Physics2D.gravity.normalized;
        Vector2 rotatedOffset = Quaternion.Euler(0, 0, rotationAngle) * groundCheckOffset;
        Vector2 origin = (Vector2)transform.position + rotatedOffset;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + gravityDir * groundCheckDistance);
    }
}
