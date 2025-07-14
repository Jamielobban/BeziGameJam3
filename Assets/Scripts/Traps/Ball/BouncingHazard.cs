using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BouncingHazard : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 initialDirection = Vector2.up;

    public Vector2 spawnPosition;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Cache initial spawn point if not set manually
        if (spawnPosition == Vector2.zero)
            spawnPosition = transform.position;

        StartCoroutine(LaunchWithDelay(0.4f));
    }

    private IEnumerator LaunchWithDelay(float delay)
    {
        rb.linearVelocity = Vector2.zero; // stop movement
        yield return new WaitForSeconds(delay);
        rb.linearVelocity = initialDirection.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            // Reset hazard
            transform.position = spawnPosition;
            rb.linearVelocity = Vector2.zero;

            // Restart after delay
            StartCoroutine(LaunchWithDelay(0.4f));

            // Handle player death
            FindFirstObjectByType<GameManager>().SoftReset();
        }
    }
}
