using UnityEngine;

public class Winning : MonoBehaviour
{
    public GameObject winningScreen;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            winningScreen.SetActive(true);
        }
    }

    public void MainMenu()
    {
        GameManager.instance.LoadLevel(Player.instance.transform.position, 0); // Load the main menu scene
    }
}
