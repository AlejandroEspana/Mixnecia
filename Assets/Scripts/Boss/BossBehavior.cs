using System.Collections;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    protected BossStateMachine stateMachine;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    [Tooltip("Límite máximo a la izquierda (Min X)")]
    public float minMovementX = -3f;
    [Tooltip("Límite máximo a la derecha (Max X)")]
    public float maxMovementX = 3f;
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
        Quaternion rotation = Quaternion.Euler(0, 0, angleOffset) * transform.rotation;
        return ObjectPooler.Instance.SpawnFromPool(bulletTag, transform.position, rotation);
    }
}
