using UnityEngine;

public enum BossState
{
    Idle,
    Moving,
    Attacking,
    Dead
}

public class BossStateMachine : MonoBehaviour
{
    public BossState currentState { get; private set; }

    private BossBehavior bossBehavior;
    private float moveTimer = 0f;
    private float timeToMove = 2f;

    private void Awake()
    {
        bossBehavior = GetComponent<BossBehavior>();
    }

    private void Start()
    {
        TransitionToState(BossState.Idle);
    }

    private void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                // Idle logic (e.g., waiting for player to approach)
                TransitionToState(BossState.Moving);
                break;
            case BossState.Moving:
                bossBehavior.HandleMovement();
                
                moveTimer += Time.deltaTime;
                if (moveTimer >= timeToMove)
                {
                    TransitionToState(BossState.Attacking);
                }
                break;
            case BossState.Attacking:
                bossBehavior.HandleAttack();
                // The behavior itself will return state to moving after attack is done
                break;
            case BossState.Dead:
                // Do nothing
                break;
        }
    }

    public void TransitionToState(BossState newState)
    {
        currentState = newState;
        
        if (newState == BossState.Moving)
        {
            moveTimer = 0f;
            timeToMove = Random.Range(1.0f, 2.5f); // Moves for 1 to 2.5 seconds before attacking
        }
        else if (newState == BossState.Attacking)
        {
            bossBehavior.StartAttack();
        }
    }
}
