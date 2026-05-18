using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public float invincibilityDuration = 1.5f;
    private bool isInvincible;

    public event Action<int> OnHealthChanged;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible) return;

        currentHealth -= damageAmount;
        OnHealthChanged?.Invoke(currentHealth);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayerHit();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        
        // Flashing effect
        float timer = 0;
        while (timer < invincibilityDuration)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        if (spriteRenderer != null) spriteRenderer.enabled = true;
        isInvincible = false;
    }

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
        gameObject.SetActive(false);
    }
}
