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
        GrassTile = 1,
        FireTile = 2,
        WaterTile = 3,
        GoalTile = 4,
        WallTile = 5,
        UITile = 6

    }

    public void FireTile(GameObject go)
    {
        if (go.CompareTag("Player"))
        {
            Player.instance.moveTargetPos = go.transform.position + Vector3.up * 2;

        }
    }

    public void GrassTile(GameObject go)
    {
        if (go.CompareTag("Player"))
        {
            Player.instance.moveTargetPos = go.transform.position + Vector3.left * 2;
        }
    }

    public void WaterTile(GameObject go)
    {
        if (go.CompareTag("Player"))
        {
            Player.instance.moveTargetPos = go.transform.position + Vector3.right * 2;
        }
    }

    public void GoalTile(GameObject go)
    {
        if (go.CompareTag("Player"))
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }

}
