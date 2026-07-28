using UnityEngine;
using System;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    // Eventos
    public event Action<int> OnTakeDamageEvent;
    public event Action<int> OnHealthRestoredEvent;
    public event Action<int, int> OnHealthChanged; // (current, max)

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void OnTakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnTakeDamageEvent?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void OnHealthRestored(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthRestoredEvent?.Invoke(amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Jugador ha muerto");
    }

    [ContextMenu("Take 20 Damage")]
    private void TakeDamageContextMenu()
    {
        OnTakeDamage(20);
    }

    [ContextMenu("Restore 20 Health")]
    private void RestoreHealthContextMenu()
    {
        OnHealthRestored(20);
    }
}