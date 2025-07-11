using UnityEngine;

public class EdgeTileManager : MonoBehaviour
{
    public static EdgeTileManager Instance { get; private set; }

    public GameObject edgeTilePrefab;
    public int gridWidth = 16;
    public int gridHeight = 16;
    public float tileSize = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlaceEdgeTiles();
    }

    public void PlaceEdgeTiles()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                bool isEdge = (x == 0 || x == gridWidth - 1 || y == 0 || y == gridHeight - 1);
                bool isCorner = (x == 0 || x == gridWidth - 1) && (y == 0 || y == gridHeight - 1);

                if (isEdge)
                {
                    Vector2 position = new Vector2(
                        (x - gridWidth / 2f + 0.5f) * tileSize,
                        (y - gridHeight / 2f + 0.5f) * tileSize
                    );

                    GameObject tile = Instantiate(edgeTilePrefab, position, Quaternion.identity, transform);

                    // Set corner color (or tag, or sprite)
                    if (isCorner)
                    {
                        tile.GetComponent<SpriteRenderer>().color = Color.red; // Corner
                        tile.GetComponent<EdgeTile>().Activate();
                    }
                    else
                    {
                        tile.GetComponent<SpriteRenderer>().color = Color.white; // Edge
                        tile.layer = LayerMask.NameToLayer("Ground");
                    }
                }
            }
        }
    }

    [System.Obsolete]
    public void CheckWinCondition()
    {
        Debug.Log("dONE");
        EdgeTile[] tiles = FindObjectsOfType<EdgeTile>();
        foreach (var tile in tiles)
        {
            if (!tile.activated)
                return; // Still tiles left
        }

        Debug.Log("All tiles activated! You win!");
        // You can trigger effects or next level here
    }
}
