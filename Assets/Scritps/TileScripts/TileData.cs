using System.Diagnostics;
using System.Security;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;
    public TileState tileState;
    public enum TileState
    {
        NormalTile = 0,
        EarthTile = 1,
        FireTile = 2,
        WaterTile = 3,
        GoalTile = 4,
        WallTile = 5

    }
}
