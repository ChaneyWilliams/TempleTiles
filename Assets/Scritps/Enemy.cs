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
    public float fightDist = 2.0f;
    public float chaseDist = 5.0f;
    float distanceToPlayer;
    Vector3 target;
    Vector3 nextPos;
    readonly Vector3[] directions =
    {
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left
    };
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
        if (distanceToPlayer < fightDist)
            ChangeState(State.Fight);
        if (distanceToPlayer < chaseDist)
            ChangeState(State.Chase);
    }

    void ChaseState()
    {
        if (distanceToPlayer > chaseDist)
            ChangeState(State.Idle);
        if (distanceToPlayer < fightDist)
            ChangeState(State.Fight);
        target = Player.instance.transform.position;
    }

    void FightState()
    {
        if (distanceToPlayer > 1.0f)
            ChangeState(State.Chase);
    }

    Vector3 FindPath(Vector3 goal)
    {
        Vector3 final = transform.position;

        foreach (Vector3 direction in directions)
        {
            Vector3 newPos = transform.position + direction;

            if (Vector3.Distance(newPos, goal) < Vector3.Distance(final, goal))
            {
                TileData td = GameManager.instance.GetTileFromMap(newPos);
                if(td == null || td.tileState != TileData.TileState.NormalTile)
                    continue;
                final = newPos;
            }
        }

        return final;
    }

    public void Move()
    {
        if (currentState == State.Chase)
        {
            nextPos = FindPath(target);
            TweenHelper.Move(transform, nextPos);
        }
    }
}
