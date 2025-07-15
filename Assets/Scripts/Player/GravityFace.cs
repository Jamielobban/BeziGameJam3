using UnityEngine;

public class GravityFace : MonoBehaviour
{
    public Transform worldRoot; 

    void Start()
    {
        worldRoot = FindFirstObjectByType<WorldRotator>().transform;
    }

    void LateUpdate()
    {

        transform.rotation = worldRoot.rotation;
    }
}
