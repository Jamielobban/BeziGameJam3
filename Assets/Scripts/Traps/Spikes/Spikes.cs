using UnityEngine;
using UnityEngine.SceneManagement;

public class Spikes : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player != null && player.isHittable)
        {
            player.isHittable = false;
            FindFirstObjectByType<GameManager>().SoftReset();
        }
    }
}
