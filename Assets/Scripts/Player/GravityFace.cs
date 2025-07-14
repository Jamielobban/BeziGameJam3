using UnityEngine;

public class GravityFace : MonoBehaviour
{
    public Transform worldRoot; // Assign the rotating world root (e.g., Parent)

    void Start()
    {
        worldRoot = FindFirstObjectByType<WorldRotator>().transform;
    }

    void LateUpdate()
    {
        // Match the rotation of the world
        transform.rotation = worldRoot.rotation;
    }
}
