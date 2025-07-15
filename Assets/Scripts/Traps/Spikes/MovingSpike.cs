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
        moveTween?.Kill();

        transform.localPosition = pointA.localPosition;

        moveTween = transform.DOLocalMove(pointB.localPosition, moveDuration)
            .SetEase(Ease.Linear)
            .SetDelay(delay)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
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
