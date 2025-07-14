using UnityEngine;
using UnityEngine.SceneManagement;

public class Spikes : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            //die
            FindFirstObjectByType<GameManager>().SoftReset();
        }
    }
}
