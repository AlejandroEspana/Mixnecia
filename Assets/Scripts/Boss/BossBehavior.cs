using System.Collections;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    protected BossStateMachine stateMachine;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    [Tooltip("Límite máximo a la izquierda (Min X)")]
    public float minMovementX = -2.74f;
    [Tooltip("Límite máximo a la derecha (Max X)")]
    public float maxMovementX = 7f;
    protected Vector3 targetPosition;

    [Header("Attack Settings")]
    public string bulletTag = "EnemyBullet";
    public float attackDuration = 3f;
    protected int patternIndex = 0;

    protected virtual void Awake()
    {
        stateMachine = GetComponent<BossStateMachine>();
        SetNewTargetPosition();
    }

    public virtual void HandleMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }

    protected void SetNewTargetPosition()
    {
        float center = (minMovementX + maxMovementX) / 2f;
        float randomX;

        // Force the boss to cross the center to the opposite side
        if (transform.position.x < center)
        {
            randomX = Random.Range(center + 0.5f, maxMovementX);
        }
        else
        {
            randomX = Random.Range(minMovementX, center - 0.5f);
        }

        targetPosition = new Vector3(randomX, transform.position.y, transform.position.z);
    }

    public void StartAttack()
    {
        StartCoroutine(AttackRoutine());
    }

    public virtual void HandleAttack()
    {
        // Continuous attack logic if needed, usually handled by coroutine
    }

    protected virtual IEnumerator AttackRoutine()
    {
        // Default attack: Basic spread
        float timer = 0f;
        while (timer < attackDuration)
        {
            ShootSpread();
            yield return new WaitForSeconds(0.5f);
            timer += 0.5f;
        }

        if (stateMachine != null) stateMachine.TransitionToState(BossState.Moving);
    }

    protected void ShootSpread()
    {
        if (ObjectPooler.Instance == null) return;

        int bulletCount = 5;
        float angleStep = 15f;
        float startAngle = -((bulletCount / 2) * angleStep);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + (i * angleStep);
            Quaternion rotation = Quaternion.Euler(0, 0, angle) * transform.rotation;
            ObjectPooler.Instance.SpawnFromPool(bulletTag, transform.position, rotation);
        }
    }

    // Helper for subclasses
    protected GameObject SpawnBullet(float angleOffset)
    {
        if (ObjectPooler.Instance == null) return null;
        Quaternion rotation = Quaternion.Euler(0, 0, angleOffset + 180f) * transform.rotation;
        return ObjectPooler.Instance.SpawnFromPool(bulletTag, transform.position, rotation);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the movement limits in the Unity Editor
        Gizmos.color = Color.yellow;
        Vector3 leftLimit = new Vector3(minMovementX, transform.position.y, transform.position.z);
        Vector3 rightLimit = new Vector3(maxMovementX, transform.position.y, transform.position.z);
        
        // Draw a line representing the movement path
        Gizmos.DrawLine(leftLimit, rightLimit);
        
        // Draw small spheres at the boundaries
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(leftLimit, 0.2f);
        Gizmos.DrawWireSphere(rightLimit, 0.2f);

        // Draw the current target position if the game is running
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPosition, 0.3f);
        }
    }
}
