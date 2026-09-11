using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
public class GameManager : MonoBehaviour
{
    [Header("TurnSystem")]
    public static GameManager instance;
    public GameState currentGameState;
    [SerializeField] private float timeBetweenTurns = 0.25f;

    [Header("TileManagement")]
    [SerializeField] private List<TileBase> allTiles;
    [SerializeField] private List<TileData> tileDatas;
    public Tilemap map;

    private Dictionary<TileBase, TileData> dataFromTile;
    private List<Vector3Int> specialTiles = new List<Vector3Int>();

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
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

        if (map == null)
            map = GameObject.FindWithTag("Tilemap")?.GetComponent<Tilemap>();
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
            case TileData.TileState.NormalTile:
            default:
                break;
        }
    }

    public void ChangeEnvironment()
    {
        specialTiles.Clear();

        BoundsInt bounds = map.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileData tile = GetTileFromMap(pos);
            if (tile == null || tile.tileState == TileData.TileState.NormalTile || tile.tileState == TileData.TileState.WallTile)
                continue;

            specialTiles.Add(pos);
        }

        Dictionary<Vector3Int, TileBase> allChanges = new Dictionary<Vector3Int, TileBase>();

        foreach (Vector3Int tile in specialTiles) //loop through all special tiles
        {
            //make a dict that has all the nighbors with the right tile type
            //(if fire tile get earth neighbors, if earth tile get water neighbors, etc..)
            Dictionary<Vector3Int, TileBase> changes = CheckAllNeighbors(tile); 

            foreach (KeyValuePair<Vector3Int, TileBase> kvp in changes)
            {
                allChanges[kvp.Key] = kvp.Value; // pull all those tiles into a dict outside the loop
            }
        }
        foreach (KeyValuePair<Vector3Int, TileBase> kvp in allChanges)
        {
            map.SetTile(kvp.Key, kvp.Value); // loop through outside dict to place new tiles without chaining new tiles (new fire tiles wont burn earth tiles etc)
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
                    break;

                case (TileData.TileState.GrassTile, TileData.TileState.WaterTile):
                    changes[nextPos] = allTiles[2];
                    break;

                case (TileData.TileState.WaterTile, TileData.TileState.FireTile):
                    changes[nextPos] = allTiles[3];
                    break;
                case (TileData.TileState.FireTile, TileData.TileState.NormalTile):
                    changes[nextPos] = allTiles[1];
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

}
