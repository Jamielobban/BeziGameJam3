using UnityEngine;

public class WorldRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Transform levelRoot;         // The entire level to rotate
    public Transform player;            // Optional: for future use
    public float maxRotationSpeed = 90f; // Max degrees/sec
    public float acceleration = 300f;    // Degrees/sec²

    private float rotationInput = 0f;
    private float currentSpeed = 0f;

    void Update()
    {
        if (rotationInput != 0f)
        {
            // Build up rotation speed with acceleration
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxRotationSpeed);
        }
        else
        {
            // Reset speed if no input
            currentSpeed = 0f;
        }

        if (currentSpeed > 0f && levelRoot != null)
        {
            float angle = rotationInput * currentSpeed * Time.deltaTime;
            levelRoot.Rotate(0f, 0f, angle);
        }
    }

    public void SetRotationDirection(float direction)
    {
        // -1 = CCW, +1 = CW, 0 = idle
        rotationInput = direction;
    }

    public void ResetRotation()
    {
        if (levelRoot != null)
        {
            levelRoot.rotation = Quaternion.identity;
        }
        currentSpeed = 0f;
        rotationInput = 0f;
    }
}
