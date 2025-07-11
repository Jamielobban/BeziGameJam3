using UnityEngine;

public class ScreenRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationAcceleration = 30f;
    public float maxRotationSpeed = 180f;
    public float gravityStrength = 25f;

    private float currentRotation = 0f;
    private float currentRotationSpeed = 0f;
    private float rotationDirection = 0f;

    public float CurrentRotation => currentRotation;

    void Start()
    {
        UpdateGravity();
    }
    
    public void SetRotationDirection(float direction)
    {
        rotationDirection = direction;
    }

    private void Update()
    {
        HandleRotation();
    }

    private void HandleRotation()
    {
        if (rotationDirection != 0f)
        {
            currentRotationSpeed += rotationAcceleration * Time.deltaTime;
            currentRotationSpeed = Mathf.Min(currentRotationSpeed, maxRotationSpeed);
        }
        else
        {
            currentRotationSpeed = 0f;
        }

        if (currentRotationSpeed > 0f)
        {
            float rotationAmount = rotationDirection * currentRotationSpeed * Time.deltaTime;
            currentRotation += rotationAmount;
            Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
            UpdateGravity();
        }
    }

    private void UpdateGravity()
    {
        float radians = currentRotation * Mathf.Deg2Rad;
        Physics2D.gravity = new Vector2(Mathf.Sin(radians), -Mathf.Cos(radians)) * gravityStrength;
    }
}
