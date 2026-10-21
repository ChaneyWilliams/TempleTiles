using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    bool nextTurn = true;
    public float timeBetweenTurns = 1.0f;
    public GameObject continueButton;
    void Start()
    {
        if (LivesCounter.instance.GetLives() <= 0 || LivesCounter.instance.currentLevelIndex == 0)
        {
            continueButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
    }
    void Update()
    {
        /*if (nextTurn)
        {
            StartCoroutine(TitleScreenAnimation());
        }*/
    }
    public void StartGame()
    {
        // Reset lives and level index when starting a new game
        LivesCounter.instance.SetLives(3);
        LivesCounter.instance.SetCurrentLevelIndex(0);

        UnityEngine.SceneManagement.SceneManager.LoadScene(1); // Change 1 to your first level's build index
    }
    public void LoadGame()
    {
        // Load the saved level index
        int savedLevelIndex = LivesCounter.instance.currentLevelIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(savedLevelIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator TitleScreenAnimation()
    {
        nextTurn = false;
        GameManager.instance.ChangeGameState(GameManager.GameState.LevelTurn);
        yield return new WaitForSeconds(timeBetweenTurns);
        nextTurn = true;
        
    }
}
