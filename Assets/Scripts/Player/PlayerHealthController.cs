using UnityEngine;
using System;

public class PlayerHealthController : MonoBehaviour, IDamageable
{
    [Header("Health Stats")]
    [SerializeField] private Stat maxHealthStat;
    [SerializeField] private Stat healthRegenStat;
    [SerializeField] private Stat lifeStealStat;
    [SerializeField] private Stat armorStat;
    [SerializeField] private float health = 100;

    [SerializeField] private Stat invulnerabilityTimeStat;
    [Header("Shield Stats")]
    [SerializeField] private Stat shieldStat;
    [SerializeField] private Stat shieldRegenStat;
    [SerializeField] private float currentShield = 0;

    public int Health
    {
        get => Mathf.RoundToInt(health);
        set => health = value;
    }
    [SerializeField] private bool canBeHit = true;

    // Eventos
    public event Action<int> OnTakeDamageEvent;
    public event Action<int> OnHealthRestoredEvent;
    public event Action<int, int> OnHealthChanged; // (current, max)
    public event Action<int, int> OnShieldChanged; // (current, max)

    private bool _isGameStarted;

    private void Start()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
            GameEventsManager.Instance.OnEnemyTakeDamage += HandleEnemyTakeDamage;
            if (GameEventsManager.Instance.IsGameStarted)
            {
                HandleGameStarted();
            }
        }
        else
        {
            HandleGameStarted();
        }
    }

    private void OnDestroy()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
            GameEventsManager.Instance.OnEnemyTakeDamage -= HandleEnemyTakeDamage;
        }
    }

    private void HandleGameStarted()
    {
        _isGameStarted = true;

        maxHealthStat = PlayerStatsManager.Instance.GetStatByName("Health");
        healthRegenStat = PlayerStatsManager.Instance.GetStatByName("HealthRegen");
        lifeStealStat = PlayerStatsManager.Instance.GetStatByName("LifeSteal");
        armorStat = PlayerStatsManager.Instance.GetStatByName("Armor");
        invulnerabilityTimeStat = PlayerStatsManager.Instance.GetStatByName("InvulnerabilityTime");
        shieldStat = PlayerStatsManager.Instance.GetStatByName("Shield");
        shieldRegenStat = PlayerStatsManager.Instance.GetStatByName("ShieldRegen");

        if (maxHealthStat != null)
        {
            health = Mathf.RoundToInt(maxHealthStat.Value);
            OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
        }

        if (shieldStat != null)
        {
            currentShield = Mathf.RoundToInt(shieldStat.Value);
            OnShieldChanged?.Invoke(Mathf.RoundToInt(currentShield), Mathf.RoundToInt(shieldStat.Value));
        }
    }

    private void Update()
    {
        if (!_isGameStarted)
        {
            return;
        }

        HandleHealthRegen();
        HandleShieldRegen();
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

    private void HandleShieldRegen()
    {
        if (currentShield < Mathf.RoundToInt(shieldStat.Value))
        {
            currentShield += shieldRegenStat.Value * Time.deltaTime;
            currentShield = Mathf.Clamp(currentShield, 0, Mathf.RoundToInt(shieldStat.Value));
            OnShieldChanged?.Invoke(Mathf.RoundToInt(currentShield), Mathf.RoundToInt(shieldStat.Value));
        }
    }
    public void TakeDamage(int damage)
    {
        if (!canBeHit)
        {
            return;
        }

        int finalDamage = damage;
        if (armorStat.Value > 0f)
        {
            float reductionMultiplier = Mathf.Clamp01(1f - (armorStat.Value / 100f));
            finalDamage = Mathf.RoundToInt(damage * reductionMultiplier);
        }

        if (shieldStat.Value > 0)
        {
            int shieldDamage = Mathf.Min(finalDamage, Mathf.RoundToInt(currentShield));
            currentShield -= shieldDamage;
            OnShieldChanged?.Invoke(Mathf.RoundToInt(currentShield), Mathf.RoundToInt(shieldStat.Value));
            finalDamage -= shieldDamage;

            if (finalDamage <= 0)
            {
                OnTakeDamageEvent?.Invoke(shieldDamage);
                OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
                if (GameEventsManager.Instance != null)
                {
                    GameEventsManager.Instance.TriggerPlayerTakeDamage(shieldDamage);
                }
                return;
            }
        }
        health -= finalDamage;
        health = Mathf.Clamp(health, 0, Mathf.RoundToInt(maxHealthStat.Value));

        OnTakeDamageEvent?.Invoke(finalDamage);
        OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.TriggerPlayerTakeDamage(finalDamage);
        }

        if (health <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvulnerabilityCoroutine());
    }

    public int MaxHealth => maxHealthStat != null ? Mathf.RoundToInt(maxHealthStat.Value) : 100;
    public int CurrentShield => Mathf.RoundToInt(currentShield);
    public int MaxShield => shieldStat != null ? Mathf.RoundToInt(shieldStat.Value) : 0;

    private void HandleEnemyTakeDamage(int damage)
    {
        if (lifeStealStat == null || lifeStealStat.Value <= 0f || damage <= 0 || health <= 0)
        {
            return;
        }

        float healAmount = damage * (lifeStealStat.Value / 100f);
        RestoreHealth(healAmount);
    }

    public void RestoreHealth(float amount)
    {
        if (amount <= 0f || health <= 0)
        {
            return;
        }

        health += amount;
        health = Mathf.Clamp(health, 0, Mathf.RoundToInt(maxHealthStat.Value));

        OnHealthRestoredEvent?.Invoke(Mathf.RoundToInt(amount));
        OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
    }

    public void OnHealthRestored(int amount)
    {
        RestoreHealth(amount);
    }

    public void RestoreShield(int amount)
    {
        currentShield += amount;
        int maxShield = shieldStat != null ? Mathf.RoundToInt(shieldStat.Value) : Mathf.RoundToInt(currentShield);
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);

        OnShieldChanged?.Invoke(Mathf.RoundToInt(currentShield), maxShield);
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
        GameEventsManager.Instance.TriggerPlayerDeath();
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