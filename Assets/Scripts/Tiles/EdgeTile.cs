using UnityEngine;

public class EdgeTile : MonoBehaviour
{
    public bool activated = false;

    public void Activate()
    {
        if (activated) return;

        activated = true;
        GetComponent<SpriteRenderer>().color = Color.green; // Visual feedback
        EdgeTileManager.Instance.CheckWinCondition();
    }
}
