using UnityEngine;
using System;

public class PlayerHealthController : MonoBehaviour, IDamageable
{
    [Header("Health Stats")]
    [SerializeField] private Stat maxHealthStat;
    [SerializeField] private Stat healthRegenStat;
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

    private void Start()
    {
        maxHealthStat = PlayerStatsManager.Instance.GetStatByName("Health");
        healthRegenStat = PlayerStatsManager.Instance.GetStatByName("HealthRegen");
        invulnerabilityTimeStat = PlayerStatsManager.Instance.GetStatByName("InvulnerabilityTime");
        shieldStat = PlayerStatsManager.Instance.GetStatByName("Shield");
        shieldRegenStat = PlayerStatsManager.Instance.GetStatByName("ShieldRegen");

        health = Mathf.RoundToInt(maxHealthStat.Value);
        currentShield = Mathf.RoundToInt(shieldStat.Value);


        OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
        OnShieldChanged?.Invoke(Mathf.RoundToInt(currentShield), Mathf.RoundToInt(shieldStat.Value));
    }

    private void Update()
    {
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

        if (shieldStat.Value > 0)
        {
            int shieldDamage = Mathf.Min(damage, Mathf.RoundToInt(currentShield));
            currentShield -= shieldDamage;
            OnShieldChanged?.Invoke(Mathf.RoundToInt(currentShield), Mathf.RoundToInt(shieldStat.Value));
            damage -= shieldDamage;

            if (damage <= 0)
            {
                OnTakeDamageEvent?.Invoke(shieldDamage);
                OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
                return;
            }
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

    public int MaxHealth => maxHealthStat != null ? Mathf.RoundToInt(maxHealthStat.Value) : 100;
    public int CurrentShield => Mathf.RoundToInt(currentShield);
    public int MaxShield => shieldStat != null ? Mathf.RoundToInt(shieldStat.Value) : 0;

    public void OnHealthRestored(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, Mathf.RoundToInt(maxHealthStat.Value));

        OnHealthRestoredEvent?.Invoke(amount);
        OnHealthChanged?.Invoke(Mathf.RoundToInt(health), Mathf.RoundToInt(maxHealthStat.Value));
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