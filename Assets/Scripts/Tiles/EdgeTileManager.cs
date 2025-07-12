#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

public class EdgeTileManager : MonoBehaviour
{
    public static EdgeTileManager Instance { get; private set; }

    public GameObject edgeTilePrefab;
    public int gridWidth = 16;
    public int gridHeight = 16;
    public float tileSize = 1f;

    private readonly List<EdgeTile> cornerTiles = new();
    private readonly List<EdgeTile> allTiles = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [System.Obsolete]
    private void Start()
    {
        PlaceEdgeTiles();

        // Reset tile states after placement
        foreach (var tile in allTiles)
        {
            tile.activated = false;

            if (cornerTiles.Contains(tile))
            {
                tile.GetComponent<SpriteRenderer>().color = Color.red;  // Keep corners red
            }
            else
            {
                tile.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    public void PlaceEdgeTiles()
    {
        cornerTiles.Clear();
        allTiles.Clear();

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

                    GameObject tileObj = Instantiate(edgeTilePrefab, position, Quaternion.identity, transform);
                    EdgeTile tile = tileObj.GetComponent<EdgeTile>();
                    allTiles.Add(tile);

                    if (isCorner)
                    {
                        cornerTiles.Add(tile);
                        tile.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                    else
                    {
                        tile.GetComponent<SpriteRenderer>().color = Color.white;
                        tileObj.layer = LayerMask.NameToLayer("Ground");
                    }
                }
            }
        }
    }

    public void ClearTiles()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(transform.GetChild(i).gameObject);
            else
#endif
                Destroy(transform.GetChild(i).gameObject);
        }

        cornerTiles.Clear();
        allTiles.Clear();
    }

    [System.Obsolete]
    public void CheckWinCondition()
    {
        Debug.Log("Checking Win Condition...");

        foreach (var tile in allTiles)
        {
            if (cornerTiles.Contains(tile))
                continue;  // Skip corners

            if (!tile.activated)
                return;  // Still tiles left to activate
        }

        Debug.Log("All tiles activated! Starting transition...");
        FindObjectOfType<LevelTransitionManager>().StartTransition(() =>
        {
            Debug.Log("Level swapped!");
        });
    }

   #if UNITY_EDITOR
    [ContextMenu("Preview Edge Tiles In Editor")]
    private void EditorPreviewTiles()
    {
        ClearTiles();
        PlaceEdgeTiles();
    }

    [ContextMenu("Delete All Edge Tiles In Editor")]
    private void EditorDeleteTiles()
    {
        ClearTiles();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (transform.childCount == 0)
            {
                PlaceEdgeTiles();
            }
        }
    }
    #endif
}
