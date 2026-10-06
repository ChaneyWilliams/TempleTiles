using UnityEngine;

public class LivesCounter : MonoBehaviour
{
    public static LivesCounter instance;
    public int lives = 3;

    void Awake()
    {
        if( instance == null || instance != this)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLives(int newLives)
    {
        lives = (newLives > 99 || newLives < 0) ? lives: newLives;
    }

    public void SubLives()
    {
        lives--;
    }

    public int GetLives()
    {
        return lives;
    }
}
