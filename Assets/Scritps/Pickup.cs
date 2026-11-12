using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LivesCounter.instance.AddLife();
            Destroy(gameObject);
        }
    }
}
