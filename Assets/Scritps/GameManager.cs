using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [Header("TurnSystem")]
    public static GameManager instance;
    public GameState currentGameState;
    [SerializeField] private float timeBetweenTurns = 0.1f;

    [Header("TileManagement")]
    [SerializeField] private List<TileBase> allTiles;
    [SerializeField] private List<TileData> tileDatas;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem waterParticles;
    [SerializeField] private ParticleSystem grassParticles;
    public Tilemap map;
    public Tilemap previewMap;

    private Dictionary<TileBase, TileData> dataFromTile;
    private List<Vector3Int> specialTiles = new List<Vector3Int>();
    private ParticleSystem particlesInstance;

    private readonly List<Vector3Int> directions = new List<Vector3Int>
    {
        Vector3Int.left,
        Vector3Int.right,
        Vector3Int.down,
        Vector3Int.up
    };

    public enum GameState
    {
        PlayerTurn = 0,
        LevelTurn
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(gameObject); }

    }

    void Start()
    {
        if (map == null)
            map = GameObject.FindWithTag("Tilemap")?.GetComponent<Tilemap>();
        if (previewMap == null)
            previewMap = GameObject.FindWithTag("PreviewMap")?.GetComponent<Tilemap>();
        dataFromTile = new Dictionary<TileBase, TileData>();

        foreach (TileData tileData in tileDatas)
        {
            foreach (TileBase tile in tileData.tiles)
            {
                if (!dataFromTile.ContainsKey(tile))
                    dataFromTile.Add(tile, tileData);
            }
        }
    }

    public void ChangeGameState(GameState newGameState)
    {
        StopAllCoroutines();
        Debug.Log(newGameState);
        StartCoroutine(ChangeGameStateRoutine(newGameState));
    }

    private IEnumerator ChangeGameStateRoutine(GameState newGameState)
    {
        currentGameState = newGameState;

        switch (currentGameState)
        {
            case GameState.PlayerTurn:
                yield break;

            case GameState.LevelTurn:
                yield return new WaitForSeconds(timeBetweenTurns);

                ChangeEnvironment();

                yield return new WaitForSeconds(timeBetweenTurns);
                ChangeGameState(GameState.PlayerTurn);
                break;
        }
    }

    public TileBase GetTileBase(int tileIndex)
    {
        return allTiles[tileIndex];
    }

    public TileData GetTileFromMap(Vector3 position)
    {
        Vector3Int gridPos = map.WorldToCell(position);

        TileBase tile = map.GetTile(gridPos);

        if (tile == null) return null;
        return dataFromTile[tile];
    }


    //GET THE PREFAB THATS STORED IN THE TILEBASE
    public void TileChoices(TileData tileInfo, GameObject entered)
    {
        switch (tileInfo.tileState)
        {
            case TileData.TileState.FireTile:
                tileInfo.FireTile(entered);
                break;

            case TileData.TileState.GrassTile:
                tileInfo.GrassTile(entered);
                break;
            case TileData.TileState.WaterTile:
                tileInfo.WaterTile(entered);
                break;
            case TileData.TileState.GoalTile:
                tileInfo.GoalTile(entered);
                break;
            case TileData.TileState.NormalTile:
            default:
                break;
        }
    }

    public void ChangeEnvironment()
    {
        specialTiles.Clear();

        BoundsInt bounds = map.cellBounds;

        Dictionary<Vector3Int, TileBase> allChanges =
            new Dictionary<Vector3Int, TileBase>();

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileData tile = GetTileFromMap(pos);

            if (tile == null)
                continue;

            // Normal tile becomes grass if surrounded
            // by 3 or more grass tiles.
            if (tile.tileState == TileData.TileState.NormalTile)
            {
                if (HasEnoughGrassNeighbors(pos))
                {
                    allChanges[pos] = allTiles[2];
                }

                continue;
            }

            if (tile.tileState == TileData.TileState.WallTile)
                continue;

            if (tile.tileState == TileData.TileState.FireTile)
            {
                GameObject tileObject = map.GetInstantiatedObject(pos);

                if (tileObject != null)
                {
                    FireTileTracker tracker =
                        tileObject.GetComponent<FireTileTracker>();

                    if (tracker != null)
                    {
                        tracker.BurnOut();
                    }
                }
            }

            specialTiles.Add(pos);
        }

        // Check all special tiles for environmental changes
        foreach (Vector3Int tile in specialTiles)
        {
            Dictionary<Vector3Int, TileBase> changes =
                CheckAllNeighbors(tile);

            foreach (KeyValuePair<Vector3Int, TileBase> kvp in changes)
            {
                allChanges[kvp.Key] = kvp.Value;
            }
        }

        // Apply all changes at the end
        foreach (KeyValuePair<Vector3Int, TileBase> kvp in allChanges)
        {
            map.SetTile(kvp.Key, kvp.Value);

            TileData newTile = GetTileFromMap(kvp.Key);

            if (newTile != null &&
                newTile.tileState == TileData.TileState.FireTile)
            {
                GameObject tileObject =
                    map.GetInstantiatedObject(kvp.Key);

                if (tileObject != null)
                {
                    FireTileTracker tracker =
                        tileObject.GetComponent<FireTileTracker>();

                    if (tracker != null)
                    {
                        tracker.SetFireTTL(3);
                    }
                }
            }
        }
    }



    Dictionary<Vector3Int, TileBase> CheckAllNeighbors(Vector3Int position)
    {

        Dictionary<Vector3Int, TileBase> changes = new Dictionary<Vector3Int, TileBase>();
        TileData currentTile = GetTileFromMap(position);
        if (currentTile == null)
            return changes;

        foreach (Vector3Int nextPos in GetNeighbors(position))
        {
            //chunky switch boi 
            TileData neighborTile = GetTileFromMap(nextPos);
            if (neighborTile == null)
                continue;
            switch ((currentTile.tileState, neighborTile.tileState))
            {
                case (TileData.TileState.FireTile, TileData.TileState.GrassTile):
                    changes[nextPos] = allTiles[1];
                    SpawnParticles(fireParticles, nextPos);
                    break;

                case (TileData.TileState.GrassTile, TileData.TileState.WaterTile):
                    changes[nextPos] = allTiles[2];
                    SpawnParticles(grassParticles, nextPos);
                    break;

                case (TileData.TileState.WaterTile, TileData.TileState.FireTile):
                    changes[nextPos] = allTiles[3];
                    SpawnParticles(waterParticles, nextPos);
                    break;
                case(TileData.TileState.WaterTile, TileData.TileState.NormalTile):
                    changes[nextPos] = allTiles[3];
                    SpawnParticles(waterParticles, nextPos);
                    break;

            }

        }

        return changes;
    }



    List<Vector3Int> GetNeighbors(Vector3Int start)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        foreach (Vector3Int direction in directions)
        {
            neighbors.Add(start + direction);
        }
        return neighbors;
    }

    bool HasEnoughGrassNeighbors(Vector3Int position)
    {
        int grassCount = 0;

        foreach (Vector3Int neighborPos in GetNeighbors(position))
        {
            TileData neighborTile = GetTileFromMap(neighborPos);

            if (neighborTile != null &&
                neighborTile.tileState == TileData.TileState.GrassTile)
            {
                grassCount++;

                if (grassCount >= 2)
                    return true;
            }
        }

        return false;
    }



    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetTileColor(Vector3 position, Color color)
    {
        Vector3Int gridPos = map.WorldToCell(position);

        map.SetColor(gridPos, color);
    }

    public void SetPreviewTile(Vector3 position, Color color, TileBase currentTile)
    {
        Vector3Int cellPos = map.WorldToCell(position);

        previewMap.SetTile(cellPos, currentTile);
        previewMap.SetTileFlags(cellPos, TileFlags.None);
        previewMap.SetColor(cellPos, color);
    }

    public void ClearPreviewMap()
    {
        previewMap.ClearAllTiles();
    }

    void SpawnParticles(ParticleSystem particleSystem, Vector3 pos)
    {
        particlesInstance = Instantiate(particleSystem, pos, Quaternion.identity);
    }


}
