using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
    {
        Idle = 0,
        Chase = 1,
        Fight = 2
    }
    public State currentState;
    float distanceToPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = State.Idle;
        
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(Player.instance.transform.position, transform.position);
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;

            case State.Chase:
                ChaseState();
                break;

            case State.Fight:
                FightState();
                break;
        }
    }


    public void ChangeState(State newState)
    {
        if (newState == currentState)
            return;

        Debug.Log("Changing from " + currentState + " to " + newState);
        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }

    void EnterState(State state)
    {
        switch (state)
        {
            case State.Idle:
                break;

            case State.Chase:
                break;

            case State.Fight:
                break;
        }
    }

    void ExitState(State state)
    {
        switch (state)
        {
            case State.Idle:
                break;
                
            case State.Chase:
                break;

            case State.Fight:
                break;
        }
    }


    void IdleState()
    {
        if(distanceToPlayer < 1.0f)
            ChangeState(State.Fight);
        if(distanceToPlayer < 5.0f)
            ChangeState(State.Chase);
    }

    void ChaseState()
    {
        if(distanceToPlayer > 5.0f)
            ChangeState(State.Idle);
        if(distanceToPlayer < 1.0f)
            ChangeState(State.Fight);
    }

    void FightState()
    {
        if(distanceToPlayer > 1.0f)
            ChangeState(State.Chase);
    }
}
