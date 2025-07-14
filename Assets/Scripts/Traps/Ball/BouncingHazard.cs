using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BouncingHazard : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 initialDirection = Vector2.up;

    public Vector2 spawnPosition;
    private Rigidbody2D rb;
    private Vector2 lastVelocity;

    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (spawnPosition == Vector2.zero)
            spawnPosition = transform.position;

        StartCoroutine(LaunchWithDelay(0.4f));
    }

    private IEnumerator LaunchWithDelay(float delay)
    {
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(delay);

        Vector2 launchDir = initialDirection.normalized;
        rb.linearVelocity = launchDir * speed;
        lastVelocity = rb.linearVelocity;
    }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > 0.01f)
        {
            lastVelocity = rb.linearVelocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts.Length > 0)
        {
            ContactPoint2D contact = collision.contacts[0];
            Vector2 reflected = Vector2.Reflect(lastVelocity.normalized, contact.normal) * speed;

            Debug.Log($"Collision with: {collision.collider.name}");
            Debug.Log($"Incoming Velocity: {lastVelocity}");
            Debug.Log($"Contact Normal: {contact.normal}");
            Debug.Log($"Reflected Velocity: {reflected}");

            rb.linearVelocity = reflected;
        }

        if (collision.collider.CompareTag("Player"))
        {
            transform.position = spawnPosition;
            rb.linearVelocity = Vector2.zero;

            StartCoroutine(LaunchWithDelay(0.4f));
            FindFirstObjectByType<GameManager>().SoftReset();
        }
        audioSource.Play();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(rb != null ? rb.linearVelocity.normalized : initialDirection.normalized) * 1.5f);
    }
#endif
}
