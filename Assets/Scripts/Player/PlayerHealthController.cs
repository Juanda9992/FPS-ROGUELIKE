using UnityEngine;
using System;

public class PlayerHealthController : MonoBehaviour, IDamageable
{
    [SerializeField] private Stat maxHealthStat;
    [SerializeField] private Stat healthRegenStat;
    [SerializeField] private Stat invulnerabilityTimeStat;
    [SerializeField] private float health = 100;

    public int Health
    {
        get => Mathf.RoundToInt(health);
        set => health = value;
    }

    [Header("Invulnerability")]
    [SerializeField] private float timeBetweenHits = 1f;
    [SerializeField] private bool canBeHit = true;

    // Eventos
    public event Action<int> OnTakeDamageEvent;
    public event Action<int> OnHealthRestoredEvent;
    public event Action<int, int> OnHealthChanged; // (current, max)

    private void Start()
    {
        maxHealthStat = PlayerStatsManager.Instance.GetStatByName("Health");
        healthRegenStat = PlayerStatsManager.Instance.GetStatByName("HealthRegen");
        invulnerabilityTimeStat = PlayerStatsManager.Instance.GetStatByName("InvulnerabilityTime");

        health = Mathf.RoundToInt(maxHealthStat.Value);
        OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
    }

    private void Update()
    {
        HandleHealthRegen();
    }

    private void HandleHealthRegen()
    {
        if (health < Mathf.RoundToInt(maxHealthStat.Value))
        {
            health += healthRegenStat.Value * Time.deltaTime;
            health = Mathf.Clamp(health, 0, Mathf.RoundToInt(maxHealthStat.Value));
            OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
        }
    }
    public void TakeDamage(int damage)
    {
        if (!canBeHit)
        {
            return;
        }
        health -= damage;
        health = Mathf.Clamp(health, 0, Mathf.RoundToInt(maxHealthStat.Value));

        OnTakeDamageEvent?.Invoke(damage);
        OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));

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
        health = Mathf.Clamp(health, 0, Mathf.RoundToInt(maxHealthStat.Value));

        OnHealthRestoredEvent?.Invoke(amount);
        OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
    }
    private System.Collections.IEnumerator InvulnerabilityCoroutine()
    {
        canBeHit = false;
        yield return new WaitForSeconds(invulnerabilityTimeStat.Value);
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