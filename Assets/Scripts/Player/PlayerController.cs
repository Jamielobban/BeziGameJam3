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

    [Header("Wall Jump")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.2f;
    public Vector2 wallCheckOffsetLeft = new Vector2(-0.5f, 0f);
    public Vector2 wallCheckOffsetRight = new Vector2(0.5f, 0f);

    [Header("Coyote Time")]
    public float coyoteTimeDuration = 0.3f;

    [Header("Feedbacks")]
    public MMF_Player jumpFeedbacks;
    public MMF_Player landFeedbacks;
    public MMF_Player activateFeedbacks;

    [Header("Debug")]
    public Transform groundCheckVisual;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool wasGroundedLastFrame = true;
    private bool hasJumpedThisFrame = false;
    private bool wasTouchingWallLastFrame = false;
    private float coyoteTimer = 0f;

    public bool isTouchingWall = false;
    private Vector2 lastWallNormal;

    public bool isHittable = true;

    private float wallJumpSuppressElapsed = 0f;
    [Header("Wall Jump Settings")]
    public float wallJumpForce = 18f;
    public Vector2 wallJumpDirection = new Vector2(1, 2); 
    public float wallJumpSuppressTime = 0.2f;
    private float wallJumpTimer = 0f;
    private Vector2 wallJumpVelocity;
    public float wallJumpInputThreshold = 0.5f; 
    
    public float minAirTimeBeforeLandFeedback = 0.1f; 

    private float airTime = 0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isHittable = true;
    }

    private void FixedUpdate()
    {
        bool isGrounded = CheckIfGrounded();
        CheckWallContact();

        if (isTouchingWall && !wasTouchingWallLastFrame)
        {
            hasJumpedThisFrame = false;
        }

       if (!wasGroundedLastFrame && isGrounded)
        {
            if (airTime >= minAirTimeBeforeLandFeedback)
            {
                landFeedbacks?.PlayFeedbacks();
            }

            hasJumpedThisFrame = false;
        }
        if (isGrounded)
        {
            coyoteTimer = coyoteTimeDuration;
            airTime = 0f;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
            airTime += Time.fixedDeltaTime;
            Debug.Log(airTime);
        }

        if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.fixedDeltaTime;
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
        wasTouchingWallLastFrame = isTouchingWall;
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void ApplyMovement()
    {

        if (wallJumpTimer > 0f)
        {
            return;
        }
        Vector2 gravityForce = Physics2D.gravity * rb.mass;
        if (Vector2.Dot(rb.linearVelocity, Physics2D.gravity.normalized) > 0)
        {
            gravityForce *= fallMultiplier;
        }
        rb.AddForce(gravityForce, ForceMode2D.Force);

        Vector2 gravityDir = Physics2D.gravity.normalized;
        Vector2 rightDir = new Vector2(-gravityDir.y, gravityDir.x).normalized;

        Vector2 currentVel = rb.linearVelocity;
        float verticalComponent = Vector2.Dot(currentVel, gravityDir);
        float horizontalComponent = Vector2.Dot(currentVel, rightDir);

        horizontalComponent = Mathf.Clamp(horizontalComponent, -moveSpeed, moveSpeed);
        rb.linearVelocity = gravityDir * verticalComponent + rightDir * horizontalComponent;

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
        bool pressingIntoLeftWall = touchingLeftWall && moveInput.x < -0.1f;
        bool pressingIntoRightWall = touchingRightWall && moveInput.x > 0.1f;
        bool wantsWallJump = pressingIntoLeftWall || pressingIntoRightWall;

        bool isGrounded = coyoteTimer > 0f;

        if (isGrounded && !hasJumpedThisFrame)
        {
            jumpFeedbacks?.PlayFeedbacks();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            hasJumpedThisFrame = true;
            return;
        }

        if (!isGrounded && wantsWallJump)
        {
            jumpFeedbacks?.PlayFeedbacks();

            float xDir = moveInput.x > 0 ? -1f : 1f;
            Vector2 jumpDir = new Vector2(xDir * 0.7f, 1f).normalized;

            rb.linearVelocity = Vector2.zero;
            rb.linearVelocity = jumpDir * wallJumpForce;

            wallJumpTimer = wallJumpSuppressTime;
            hasJumpedThisFrame = true;
        }
    }

    private bool CheckIfGrounded()
    {
        Vector2 origin = (Vector2)transform.position + groundCheckOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    public bool touchingLeftWall;
    public bool touchingRightWall;

    private void CheckWallContact()
    {
        isTouchingWall = false;
        touchingLeftWall = false;
        touchingRightWall = false;

        Vector2 leftOrigin = (Vector2)transform.position + wallCheckOffsetLeft;
        Vector2 rightOrigin = (Vector2)transform.position + wallCheckOffsetRight;

        RaycastHit2D leftHit = Physics2D.Raycast(leftOrigin, Vector2.left, wallCheckDistance, wallLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightOrigin, Vector2.right, wallCheckDistance, wallLayer);

        if (leftHit.collider != null)
        {
            isTouchingWall = true;
            touchingLeftWall = true;
            lastWallNormal = leftHit.normal;
        }
        else if (rightHit.collider != null)
        {
            isTouchingWall = true;
            touchingRightWall = true;
            lastWallNormal = rightHit.normal;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EdgeTile tile = other.GetComponentInParent<EdgeTile>();
        if (tile != null)
        {
            if (!tile.activated)
            {
                activateFeedbacks?.PlayFeedbacks();
            }
            tile.Activate();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 origin = (Vector2)transform.position + groundCheckOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + Vector2.down * groundCheckDistance);

        Vector2 leftOrigin = (Vector2)transform.position + wallCheckOffsetLeft;
        Vector2 rightOrigin = (Vector2)transform.position + wallCheckOffsetRight;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(leftOrigin, leftOrigin + Vector2.left * wallCheckDistance);
        Gizmos.DrawLine(rightOrigin, rightOrigin + Vector2.right * wallCheckDistance);
    }
}
