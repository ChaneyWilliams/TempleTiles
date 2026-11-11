using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public float timeBetweenTurns = 1.0f;
    public GameObject continueButton;
    bool startGame = false;
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
        if (!startGame)
        {
            startGame = true;
            PlayClickSound();
        }
        // Reset lives and level index when starting a new game
        LivesCounter.instance.SetLives(3);
        LivesCounter.instance.SetCurrentLevelIndex(0);

        GameManager.instance.LoadLevel(GameManager.instance.transform.position, 1); // Change 1 to your first level's build index
    }
    public void LoadGame()
    {
        if (!startGame)
        {
            startGame = true;
            PlayClickSound();
        }
        // Load the saved level index
        int savedLevelIndex = LivesCounter.instance.currentLevelIndex;
        GameManager.instance.LoadLevel(GameManager.instance.transform.position, savedLevelIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
        PlayClickSound();
    }

    public void PlayHoverSound()
    {
        LivesCounter.instance.PlayRandomClip("HighlightButton");
    }

    public void PlayClickSound()
    {
        LivesCounter.instance.PlayRandomClip("ClickButton");
    }
}
