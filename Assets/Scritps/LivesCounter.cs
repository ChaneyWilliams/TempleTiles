using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesCounter : MonoBehaviour
{
    public static LivesCounter instance;

    public int lives = 3;
    public int currentLevelIndex = 0;

    private const string LivesKey = "SavedLives";
    private const string LevelKey = "SavedLevelIndex";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved values, using defaults for a new game.
        lives = PlayerPrefs.GetInt(LivesKey, 3);
        currentLevelIndex = PlayerPrefs.GetInt(LevelKey, 0);
    }

    public void SetLives(int newLives)
    {
        if (newLives > 99 || newLives < 0)
            return;

        lives = newLives;
        SaveProgress();
    }

    public void SubLives()
    {
        lives = Mathf.Max(0, lives - 1);
        SaveProgress();
    }

    public int GetLives()
    {
        return lives;
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(LivesKey, lives);
        PlayerPrefs.SetInt(LevelKey, SceneManager.GetActiveScene().buildIndex);
        Debug.Log("AccelerationEvent: " + lives + " lives saved at level index: " + currentLevelIndex);
        PlayerPrefs.Save();
    }

    public void SetCurrentLevelIndex(int newIndex)
    {
        currentLevelIndex = newIndex;
        SaveProgress();
    }

    void OnApplicationQuit()
    {
        SaveProgress();
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
            SaveProgress();
    }
}