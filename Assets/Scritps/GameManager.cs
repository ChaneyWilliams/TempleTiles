using UnityEngine;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState currentGameState;
    public float timeBetweenTurns = 0.25f;
    public enum GameState
    {
        PlayerTurn = 0,
        LevelTurn
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    public void ChangeGameState(GameState newGameState)
    {
        StopAllCoroutines();
        Debug.Log(newGameState);
        StartCoroutine(ChangeGameStateRoutine(newGameState));
    }

    private IEnumerator ChangeGameStateRoutine(GameState newGameState)
    {
        currentGameState = newGameState;

        switch (currentGameState)
        {
            case GameState.PlayerTurn:
                yield break;
            
            case GameState.LevelTurn:
                yield return new WaitForSeconds(timeBetweenTurns);
                ChangeGameState(GameState.PlayerTurn);
                break;
        }
    }
}
