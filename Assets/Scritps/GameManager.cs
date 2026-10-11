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
    [SerializeField] private List<TileBase> UITiles;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem waterParticles;
    [SerializeField] private ParticleSystem grassParticles;
    [SerializeField] private TileBase fireVisualTile;
    [SerializeField] private TileBase grassVisualTile;
    [SerializeField] private TileBase waterVisualTile;

    public Tilemap map;
    private Tilemap previewMap;
    private Tilemap fireMap;
    private Tilemap grassMap;
    private Tilemap waterMap;

    private Dictionary<TileBase, TileData> dataFromTile;
    private List<Vector3Int> specialTiles = new List<Vector3Int>();
    private ParticleSystem particlesInstance;
    private Vector3 moveOnesPlace = new Vector3(8.5f, -6.5f, 0.0f);
    private Vector3 movesTensPlace = new Vector3(7.5f, -6.5f, 0.0f);

    private Vector3 livesOnesPlace = new Vector3(-3.5f, -6.5f, 0.0f);
    private Vector3 livesTensPlace = new Vector3(-4.5f, -6.5f, 0.0f);


    private readonly List<Vector3Int> gameOfLifeDirections = new List<Vector3Int>
    {
        new Vector3Int(-1, -1, 0),
        new Vector3Int(-1,  0, 0),
        new Vector3Int(-1,  1, 0),
        new Vector3Int( 0, -1, 0),
        new Vector3Int( 0,  1, 0),
        new Vector3Int( 1, -1, 0),
        new Vector3Int( 1,  0, 0),
        new Vector3Int( 1,  1, 0)
    };


    public enum GameState
    {
        PlayerTurn = 0,
        LevelTurn
    }

    public int moveCounter = 0;

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
        if (fireMap == null)
            fireMap = GameObject.FindWithTag("FireMap")?.GetComponent<Tilemap>();
        if (grassMap == null)
            grassMap = GameObject.FindWithTag("GrassMap")?.GetComponent<Tilemap>();
        if (waterMap == null)
            waterMap = GameObject.FindWithTag("WaterMap")?.GetComponent<Tilemap>();
        dataFromTile = new Dictionary<TileBase, TileData>();

        foreach (TileData tileData in tileDatas)
        {
            foreach (TileBase tile in tileData.tiles)
            {
                if (!dataFromTile.ContainsKey(tile))
                    dataFromTile.Add(tile, tileData);
            }
        }
        SyncEntireSpecialMaps();
        SetLivesCounterTiles();
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
                if (CheckMoveCounterTiles())
                {
                    SynchMoveCounterTiles();
                    moveCounter = (moveCounter + 1 > 99) ? moveCounter : moveCounter + 1;
                    int ones = moveCounter % 10;
                    int tens = (moveCounter / 10) % 10;
                    map.SetTile(Vector3Int.FloorToInt(moveOnesPlace), UITiles[ones]);
                    map.SetTile(Vector3Int.FloorToInt(movesTensPlace), UITiles[tens]);
                }
                if (CheckLivesCounterTiles())
                {
                    SynchLivesCounterTiles();
                    int ones = LivesCounter.instance.GetLives() % 10;
                    int tens = (LivesCounter.instance.GetLives() / 10) % 10;
                    map.SetTile(Vector3Int.FloorToInt(livesOnesPlace), UITiles[ones]);
                    map.SetTile(Vector3Int.FloorToInt(livesTensPlace), UITiles[tens]);
                }
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

    public TileBase TileAtPos(Vector3 pos)
    {
        Vector3Int gridPos = map.WorldToCell(pos);
        return map.GetTile(gridPos);
    }

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

        // PHASE 1: Process environmental interactions first.
        Dictionary<Vector3Int, TileBase> environmentChanges =
            new Dictionary<Vector3Int, TileBase>();

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileData tile = GetTileFromMap(pos);

            if (tile == null ||
                tile.tileState == TileData.TileState.UITile)
                continue;

            if (tile.tileState == TileData.TileState.FireTile)
            {
                GameObject tileObject = map.GetInstantiatedObject(pos);

                if (tileObject != null)
                {
                    FireTileTracker tracker =
                        tileObject.GetComponent<FireTileTracker>();

                    if (tracker != null)
                        tracker.BurnOut();
                }
            }

            if (tile.tileState == TileData.TileState.FireTile ||
                tile.tileState == TileData.TileState.GrassTile ||
                tile.tileState == TileData.TileState.WaterTile)
            {
                specialTiles.Add(pos);
            }
        }

        // Calculate fire, water, and grass interactions
        // using the current board.
        foreach (Vector3Int pos in specialTiles)
        {
            Dictionary<Vector3Int, TileBase> changes =
                CheckAllNeighbors(pos);

            foreach (KeyValuePair<Vector3Int, TileBase> change in changes)
            {
                environmentChanges[change.Key] = change.Value;
            }
        }

        // Apply environmental changes BEFORE Game of Life.
        foreach (KeyValuePair<Vector3Int, TileBase> change
                 in environmentChanges)
        {
            map.SetTile(change.Key, change.Value);

            TileData newTile = GetTileFromMap(change.Key);

            if (newTile != null &&
                newTile.tileState == TileData.TileState.GrassTile)
            {
                SetGrassTTL(change.Key, 3);
            }

            if (newTile != null &&
                newTile.tileState == TileData.TileState.FireTile)
            {
                GameObject tileObject =
                    map.GetInstantiatedObject(change.Key);

                if (tileObject != null)
                {
                    FireTileTracker tracker =
                        tileObject.GetComponent<FireTileTracker>();

                    if (tracker != null)
                        tracker.SetFireTTL(3);
                }
            }
        }

        // PHASE 2: Conway's Game of Life with grass TTL.
        Dictionary<Vector3Int, TileBase> lifeChanges =
            new Dictionary<Vector3Int, TileBase>();

        // Track TTL updates separately so they are processed
        // before any tiles are replaced.
        List<Vector3Int> grassToAge = new List<Vector3Int>();
        List<Vector3Int> grassToDie = new List<Vector3Int>();
        List<Vector3Int> newGrass = new List<Vector3Int>();

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileData tile = GetTileFromMap(pos);

            if (tile == null ||
                tile.tileState == TileData.TileState.UITile)
                continue;

            int grassCount = CountGrassNeighbors(pos);

            switch (tile.tileState)
            {
                case TileData.TileState.NormalTile:
                    // Birth: exactly 3 grass neighbors.
                    if (grassCount == 3)
                    {
                        lifeChanges[pos] = allTiles[2];
                        newGrass.Add(pos);
                        SpawnParticles(grassParticles, pos);
                    }
                    break;

                case TileData.TileState.GrassTile:
                    // Survival: 2 or 3 neighbors.
                    // Otherwise, lose 1 TTL.
                    if (grassCount < 2 || grassCount > 3)
                    {
                        grassToAge.Add(pos);
                    }
                    break;
            }
        }

        // Age grass that failed the survival rules.
        foreach (Vector3Int pos in grassToAge)
        {
            GameObject tileObject = map.GetInstantiatedObject(pos);

            if (tileObject == null)
                continue;

            GrassLightTracker tracker =
                tileObject.GetComponent<GrassLightTracker>();

            if (tracker == null)
                continue;

            tracker.DecrementTTL();

            if (tracker.TTL <= 0)
            {
                lifeChanges[pos] = allTiles[0];
                grassToDie.Add(pos);
            }
        }

        // Apply all calculated Game of Life changes.
        foreach (KeyValuePair<Vector3Int, TileBase> change in lifeChanges)
        {
            map.SetTile(change.Key, change.Value);
        }

        // Initialize TTL for newly spawned grass.
        foreach (Vector3Int pos in newGrass)
        {
            SetGrassTTL(pos, 3);
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
                case (TileData.TileState.WaterTile, TileData.TileState.NormalTile):
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
        foreach (Vector3Int direction in gameOfLifeDirections)
        {
            neighbors.Add(start + direction);
        }
        return neighbors;
    }

    int CountGrassNeighbors(Vector3Int position)
    {
        int grassCount = 0;

        foreach (Vector3Int neighborPos in GetNeighbors(position))
        {
            TileData neighborTile = GetTileFromMap(neighborPos);

            if (neighborTile != null &&
                neighborTile.tileState == TileData.TileState.GrassTile)
            {
                grassCount++;
            }
        }

        return grassCount;
    }

    bool ShouldGrassSurvive(Vector3Int position)
    {
        int grassCount = CountGrassNeighbors(position);

        return grassCount == 2 || grassCount == 3;
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


    private void OnEnable()
    {
        Tilemap.tilemapTileChanged += OnTilemapTileChanged;
    }

    private void OnDisable()
    {
        Tilemap.tilemapTileChanged -= OnTilemapTileChanged;
    }

    private void OnTilemapTileChanged(
        Tilemap changedMap,
        Tilemap.SyncTile[] changedTiles)
    {
        // Only synchronize changes made to the main map.
        if (changedMap != map)
            return;

        foreach (Tilemap.SyncTile syncTile in changedTiles)
        {
            Vector3Int pos = syncTile.position;
            TileBase tile = syncTile.tile;

            SyncTileToSpecialMaps(pos, tile);
        }
    }

    private void SyncTileToSpecialMaps(Vector3Int pos, TileBase tile)
    {
        // Clear this position from every special map.
        fireMap?.SetTile(pos, null);
        grassMap?.SetTile(pos, null);
        waterMap?.SetTile(pos, null);

        if (tile == null)
            return;

        if (!dataFromTile.TryGetValue(tile, out TileData tileData))
            return;

        switch (tileData.tileState)
        {
            case TileData.TileState.FireTile:
                fireMap?.SetTile(pos, fireVisualTile);
                break;

            case TileData.TileState.GrassTile:
                grassMap?.SetTile(pos, grassVisualTile);
                break;

            case TileData.TileState.WaterTile:
                waterMap?.SetTile(pos, waterVisualTile);
                break;


        }
    }


    private void SyncEntireSpecialMaps()
    {
        if (map == null)
            return;

        if (fireMap != null)
            fireMap.ClearAllTiles();

        if (grassMap != null)
            grassMap.ClearAllTiles();

        if (waterMap != null)
            waterMap.ClearAllTiles();

        BoundsInt bounds = map.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = map.GetTile(pos);

            if (tile == null)
                continue;

            SyncTileToSpecialMaps(pos, tile);
        }
    }

    bool CheckMoveCounterTiles()
    {
        TileData onesTile = GetTileFromMap(moveOnesPlace);
        TileData tensTile = GetTileFromMap(movesTensPlace);

        if (onesTile == null || tensTile == null || onesTile.tileState != TileData.TileState.UITile || tensTile.tileState != TileData.TileState.UITile)
        {
            return false;
        }

        return true;
    }

    void SynchMoveCounterTiles()
    {
        TileBase onesTile = TileAtPos(moveOnesPlace);
        TileBase tensTile = TileAtPos(movesTensPlace);

        int total = UITiles.IndexOf(tensTile) * 10 + UITiles.IndexOf(onesTile);
        moveCounter = total;
    }

    bool CheckLivesCounterTiles()
    {
        TileData onesTile = GetTileFromMap(livesOnesPlace);
        TileData tensTile = GetTileFromMap(livesTensPlace);

        if (onesTile == null || tensTile == null || onesTile.tileState != TileData.TileState.UITile || tensTile.tileState != TileData.TileState.UITile)
        {
            return false;
        }
        return true;
    }

    void SynchLivesCounterTiles()
    {

        TileBase onesTile = TileAtPos(livesOnesPlace);
        TileBase tensTile = TileAtPos(livesTensPlace);

        int total = UITiles.IndexOf(tensTile) * 10 + UITiles.IndexOf(onesTile);
        LivesCounter.instance.SetLives(total);
    }
    void SetLivesCounterTiles()
    {
        int ones = LivesCounter.instance.GetLives() % 10;
        int tens = (LivesCounter.instance.GetLives() / 10) % 10;
        map.SetTile(Vector3Int.FloorToInt(livesOnesPlace), UITiles[ones]);
        map.SetTile(Vector3Int.FloorToInt(livesTensPlace), UITiles[tens]);
    }

    private void SetGrassTTL(Vector3Int pos, int ttl)
    {
        GameObject tileObject = map.GetInstantiatedObject(pos);

        if (tileObject == null)
            return;

        GrassLightTracker tracker =
            tileObject.GetComponent<GrassLightTracker>();

        if (tracker != null)
            tracker.SetGrassTTL(ttl);
    }

}
