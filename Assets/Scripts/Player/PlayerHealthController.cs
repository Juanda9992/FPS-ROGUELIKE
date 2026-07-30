using UnityEngine;
using System;

public class PlayerHealthController : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int health = 100;

    public int Health
    {
        get => health;
        set => health = value;
    }

    [Header("Invulnerability")]
    [SerializeField] private float timeBetweenHits = 1f;
    [SerializeField] private bool canBeHit = true;

    // Eventos
    public event Action<int> OnTakeDamageEvent;
    public event Action<int> OnHealthRestoredEvent;
    public event Action<int, int> OnHealthChanged; // (current, max)

    private void Awake()
    {
        health = maxHealth;
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (!canBeHit)
        {
            return;
        }
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        OnTakeDamageEvent?.Invoke(damage);
        OnHealthChanged?.Invoke(health, maxHealth);

        if (health <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvulnerabilityCoroutine());
    }

    public void OnHealthRestored(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        OnHealthRestoredEvent?.Invoke(amount);
        OnHealthChanged?.Invoke(health, maxHealth);
    }
    private System.Collections.IEnumerator InvulnerabilityCoroutine()
    {
        canBeHit = false;
        yield return new WaitForSeconds(timeBetweenHits);
        canBeHit = true;
    }

    private void Die()
    {
        Debug.Log("Jugador ha muerto");
    }

    [ContextMenu("Take 20 Damage")]
    private void TakeDamageContextMenu()
    {
        TakeDamage(20);
    }

    [ContextMenu("Restore 20 Health")]
    private void RestoreHealthContextMenu()
    {
        OnHealthRestored(20);
    }
}