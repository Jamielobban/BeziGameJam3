using UnityEngine;

public class GravityFace : MonoBehaviour
{
    void Update()
    {
        Vector2 gravity = Physics2D.gravity.normalized;
        float angle = Mathf.Atan2(gravity.y, gravity.x) * Mathf.Rad2Deg + 90f; // +90 to point "down"
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
