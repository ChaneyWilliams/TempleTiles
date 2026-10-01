using UnityEngine;

public class FireTileTracker : MonoBehaviour
{
    public int TTL = 255;

    public void BurnOut()
    {
        TTL -=1;
        if(TTL <= 0)
        {
            GameManager.instance.map.SetTile(Vector3Int.FloorToInt(transform.position), GameManager.instance.GetTileBase(0));
        }
    }

    public void SetFireTTL(int newTTL)
    {
        TTL = newTTL;
    }
}
