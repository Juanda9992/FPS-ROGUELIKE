using System;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour, IDamageable, IVulnerable
{
    public event Action OnDeath;

    [Header("Health Settings")]
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    [Header("References")]
    [SerializeField] private DamageFeedback damageFeedback;
    [SerializeField] private OrbGenerator orbGenerator;
    [SerializeField] private EnemyHealthControllerUI _healthUI;

    [Header("Vulnerability")]
    [SerializeField] private float _vulnerabilityPercentage = 1f;

    private void Start()
    {
        if (_healthUI != null)
        {
            _healthUI.UpdateHealth(health, maxHealth);
        }
    }

    public int Health
    {
        get => health;
        set => health = value;
    }

    public float Percentage
    {
        get => _vulnerabilityPercentage;
        set => _vulnerabilityPercentage = value;
    }

    public void ApplyVulnerability(float percentage, float duration)
    {
        _vulnerabilityPercentage = percentage;
        StartCoroutine(UndoVulnerabilityCoroutine(duration));
    }

    private System.Collections.IEnumerator UndoVulnerabilityCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        UndoVulnerability();
    }

    public void UndoVulnerability()
    {
        _vulnerabilityPercentage = 1f;
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.RoundToInt(damage * _vulnerabilityPercentage);
        health -= finalDamage;

        _healthUI.UpdateHealth(health, maxHealth);

        damageFeedback.PlayDamageFlash();

        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.TriggerEnemyTakeDamage(finalDamage);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.TriggerEnemyKilled(gameObject);
        }

        orbGenerator.SpawnOrbs();

        Destroy(gameObject);
    }

    [ContextMenu("Take Damage")]
    private void TakeDamageContextMenu()
    {
        TakeDamage(10);
    }
}