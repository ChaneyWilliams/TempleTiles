using UnityEngine;

public class GrassLightTracker : MonoBehaviour
{
     public int TTL = 3;

    public void SetGrassTTL(int newTTL)
    {
        TTL = newTTL;
    }

    public void DecrementTTL()
    {
        TTL--;
    }
}
