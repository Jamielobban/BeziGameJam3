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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;  // Disable built-in gravity
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

        // Horizontal movement (relative to gravity)
        moveInput.y = 0f;
        Vector2 rightDir = new Vector2(-Physics2D.gravity.y, Physics2D.gravity.x).normalized;
        float horizontalSpeed = moveSpeed * moveInput.x;

        float controlFactor = IsGrounded() ? 1f : airControlFactor;
        Vector2 currentHorizontalVel = Vector2.Dot(rb.linearVelocity, rightDir) * rightDir;
        Vector2 targetVelocity = rightDir * horizontalSpeed;

        Vector2 desiredVelocity = Vector2.Lerp(currentHorizontalVel, targetVelocity, controlFactor);
        if (Mathf.Approximately(moveInput.x, 0f))
        {
            desiredVelocity = Vector2.Lerp(currentHorizontalVel, Vector2.zero, decelerationFactor);
        }

        rb.linearVelocity = desiredVelocity + Vector2.Dot(rb.linearVelocity, Physics2D.gravity.normalized) * Physics2D.gravity.normalized;
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            Vector2 jumpDirection = -Physics2D.gravity.normalized;
            rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);
        }
    }

    private bool IsGrounded()
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
}
