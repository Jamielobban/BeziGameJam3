using UnityEngine;
using DG.Tweening;

public class MovingSpike : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveDuration = 2f;

    private Tween moveTween;

    public float delay = 0.4f;

    void Start()
    {
        ResetSpike();
    }

    void ResetSpike()
    {
        // Kill old tween
        moveTween?.Kill();

        // Force absolute world position reset
        transform.position = pointA.position;

        // Tween using absolute world positions
        moveTween = transform.DOMove(pointB.position, moveDuration)
            .SetEase(Ease.Linear)
            .SetDelay(delay)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true); // optional: makes it continue if game is paused
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            ResetSpike();
            FindFirstObjectByType<GameManager>().SoftReset();
        }
    }
}
