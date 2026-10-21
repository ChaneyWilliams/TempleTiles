using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesCounter : MonoBehaviour
{
    public static LivesCounter instance;

    public int lives = 3;
    public int currentLevelIndex = 0;
    public float globalVolume = 1.0f;

    private const string LivesKey = "SavedLives";
    private const string LevelKey = "SavedLevelIndex";
    private const string GlobalVolumeKey = "GlobalVolume";

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
        globalVolume = PlayerPrefs.GetFloat(GlobalVolumeKey, 1.0f);
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
        if (lives == 0)
        {
            SceneManager.LoadScene(0); 
        }
        SaveProgress();
    }

    public int GetLives()
    {
        return lives;
    }

    public void SetGlobalVolume(float newVolume)
    {
        globalVolume = Mathf.Clamp01(newVolume);
        PlayerPrefs.SetFloat(GlobalVolumeKey, globalVolume);
        PlayerPrefs.Save();
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(LivesKey, lives);
        PlayerPrefs.SetInt(LevelKey, SceneManager.GetActiveScene().buildIndex);
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