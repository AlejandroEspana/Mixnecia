using System;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    public int scoreValue = 10000;
    public int levelIndex = 1; // Set this in inspector depending on scene/level

    public event Action<int, int> OnHealthChanged; // current, max
    public event Action OnBossDefeated;

    private BossStateMachine stateMachine;

    private void Start()
    {
        currentHealth = maxHealth;
        stateMachine = GetComponent<BossStateMachine>();
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (stateMachine != null && stateMachine.currentState == BossState.Dead) return;

        currentHealth -= amount;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (stateMachine != null) stateMachine.TransitionToState(BossState.Dead);
        ClearAllEnemyBullets();
        
        BossNarrative narrative = GetComponent<BossNarrative>();
        if (narrative != null)
        {
            DialogueSequence outro = narrative.GetOutroDialogue();
            if (outro != null && outro.lines != null && outro.lines.Count > 0)
            {
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.PlaySequence(outro, () => {
                        FinishDeath();
                    });
                    return;
                }
            }
        }
        
        FinishDeath();
    }

    private void FinishDeath()
    {
        OnBossDefeated?.Invoke();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerLevelComplete(levelIndex - 1, scoreValue);
        }
        gameObject.SetActive(false); // Or trigger death animation
    }

    private void ClearAllEnemyBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (var b in bullets)
        {
            if (b != null) b.SetActive(false);
        }
    }
}
