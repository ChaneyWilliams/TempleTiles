using UnityEngine;

public class Winning : MonoBehaviour
{
    public GameObject winningScreen;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            winningScreen.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; // Resume the game before loading the main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); // Load the main menu scene
    }
}
