#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class EdgeTileManager : MonoBehaviour
{
    public static EdgeTileManager Instance { get; private set; }

    public GameObject edgeTilePrefab;
    public int gridWidth = 16;
    public int gridHeight = 16;
    public float tileSize = 1f;

    private readonly List<EdgeTile> cornerTiles = new();
    private List<EdgeTile> allTiles = new();

    private AudioSource so;

    public string nextSceneName;

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
        if (transform.childCount == 0)
        {
            PlaceEdgeTiles();
        }

        cornerTiles.Clear();
        allTiles.Clear();

        foreach (Transform child in transform)
        {
            EdgeTile tile = child.GetComponent<EdgeTile>();
            if (tile == null) continue;

            allTiles.Add(tile);
            tile.activated = false;

            var sprite = tile.GetComponent<SpriteRenderer>();
            if (sprite.color == Color.red)
            {
                cornerTiles.Add(tile);
            }
            else
            {
                sprite.color = Color.white;
            }
        }
    }

    public void ResetAllTiles()
    {
        allTiles = FindObjectsByType<EdgeTile>(FindObjectsSortMode.None).ToList();
        foreach (var tile in allTiles)
        {
            tile.activated = false;

            if (cornerTiles.Contains(tile))
            {
                //tile.GetComponent<SpriteRenderer>().color = Color.red; // Corner stays red
            }
            else
            {
                tile.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        Debug.Log("Reset!");
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
        //Debug.Log("Checking Win Condition...");

        foreach (var tile in allTiles)
        {
            if (cornerTiles.Contains(tile))
                continue;  

            if (!tile.activated)
                return;  
        }

        FindFirstObjectByType<PlayerController>().isHittable = false;

        Debug.Log("All tiles activated! Starting transition...");
        so = GameObject.FindGameObjectWithTag("CompleteAudio").GetComponent<AudioSource>();
        so.Play();
        FindObjectOfType<LevelTransitionManager>().StartTransition(nextSceneName);
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
