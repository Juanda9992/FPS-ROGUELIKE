using System;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour, IDamageable, IVulnerable
{
    public event Action OnDeath;

    [Header("Health Settings")]
    [SerializeField] private int _health = 100;
    [SerializeField] private int _maxHealth = 100;

    [Header("References")]
    [SerializeField] private DamageFeedback _damageFeedback;
    [SerializeField] private OrbGenerator _orbGenerator;
    [SerializeField] private EnemyHealthControllerUI _healthUI;

    [Header("Vulnerability")]
    [SerializeField] private float _vulnerabilityPercentage = 1f;

    private void Start()
    {
        if (_healthUI != null)
        {
            _healthUI.UpdateHealth(_health, _maxHealth);
        }
    }

    public int Health
    {
        get => _health;
        set => _health = value;
    }

    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }

    public float Percentage
    {
        get => _vulnerabilityPercentage;
        set => _vulnerabilityPercentage = value;
    }

    public void InitializeHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
        _health = maxHealth;

        if (_healthUI != null)
        {
            _healthUI.UpdateHealth(_health, _maxHealth);
        }
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
        _health -= finalDamage;

        if (_healthUI != null)
        {
            _healthUI.UpdateHealth(_health, _maxHealth);
        }

        if (_damageFeedback != null)
        {
            _damageFeedback.PlayDamageFlash();
        }

        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.TriggerEnemyTakeDamage(finalDamage);
        }

        if (_health <= 0)
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

        if (_orbGenerator != null)
        {
            _orbGenerator.SpawnOrbs();
        }

        Destroy(gameObject);
    }

    [ContextMenu("Take Damage")]
    private void TakeDamageContextMenu()
    {
        TakeDamage(10);
    }
}