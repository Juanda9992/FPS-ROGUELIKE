using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrenadeController : MonoBehaviour
{
    [Header("Grenade Settings")]
    [SerializeField] private GrenadeBase grenadePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float upwardForce = 2f;

    [Header("Base Stats (Parametrizables)")]
    [SerializeField] private int baseDamage = 75;
    [SerializeField] private float baseRadius = 6f;
    [SerializeField] private float baseCooldown = 5f;
    [SerializeField] private float baseFuseTime = 3f;

    [Header("PlayerStatsManager Integration")]
    [SerializeField] private string damageStatName = "GrenadeDamageMultiplier";
    [SerializeField] private string radiusStatName = "GrenadeRadiusMultiplier";
    [SerializeField] private string cooldownStatName = "GrenadeCooldownMultiplier";

    private PlayerInputActions input;
    private float lastThrowTime = -999f;

    // Events for UI / Systems
    public event Action OnGrenadeThrown;
    public event Action<float, float> OnCooldownUpdated; // (remainingTime, totalCooldown)

    // Current dynamic stats
    public float CurrentDamageMultiplier { get; set; }
    public float CurrentRadiusMultiplier { get; set; }
    public float CurrentCooldownMultiplier { get; set; }
    public float CurrentFuseTimeMultiplier { get; set; }

    public float CurrentCooldown => CurrentCooldownMultiplier;
    public bool IsOnCooldown => Time.time < lastThrowTime + CurrentCooldown;
    public float RemainingCooldown => Mathf.Max(0f, (lastThrowTime + CurrentCooldown) - Time.time);

    private void Awake()
    {
        input = new PlayerInputActions();
        input.Player.Grenade.performed += OnGrenadeInputPerformed;

        // Initialize default property values (multipliers default to 1f)
        CurrentDamageMultiplier = 1f;
        CurrentRadiusMultiplier = 1f;
        CurrentCooldownMultiplier = baseCooldown;
        CurrentFuseTimeMultiplier = baseFuseTime;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        RefreshStats();
    }

    private void Update()
    {
        if (IsOnCooldown)
        {
            OnCooldownUpdated?.Invoke(RemainingCooldown, CurrentCooldownMultiplier);
        }
    }

    /// <summary>
    /// Fetches stats from PlayerStatsManager if present, otherwise keeps default parameterized stats.
    /// </summary>
    public void RefreshStats()
    {
        Stat damageStat = PlayerStatsManager.Instance.GetStatByName(damageStatName);
        CurrentDamageMultiplier = damageStat.Value;

        Stat radiusStat = PlayerStatsManager.Instance.GetStatByName(radiusStatName);
        CurrentRadiusMultiplier = radiusStat.Value;

        Stat cooldownStat = PlayerStatsManager.Instance.GetStatByName(cooldownStatName);
        CurrentCooldownMultiplier = Mathf.Max(0.1f, cooldownStat.Value);
    }

    /// <summary>
    /// Programmatic API to update custom stats at runtime (e.g. for upgrades or item pickups).
    /// </summary>
    public void SetCustomStats(float damageMultiplier, float radiusMultiplier, float cooldown)
    {
        CurrentDamageMultiplier = damageMultiplier;
        CurrentRadiusMultiplier = radiusMultiplier;
        CurrentCooldownMultiplier = cooldown;
    }

    private void OnGrenadeInputPerformed(InputAction.CallbackContext context)
    {
        TryThrowGrenade();
    }

    public bool TryThrowGrenade()
    {
        if (IsOnCooldown)
        {
            Debug.Log($"[PlayerGrenadeController] Grenade on cooldown! Remaining: {RemainingCooldown:F1}s");
            return false;
        }
        ThrowGrenade();
        return true;
    }

    private void ThrowGrenade()
    {
        lastThrowTime = Time.time;
        Vector3 spawnPos = throwPoint.position;
        Quaternion spawnRot = throwPoint.rotation;
        GrenadeBase grenadeInstance = Instantiate(grenadePrefab, spawnPos, spawnRot);

        // Pass parameterized stats to the grenade instance
        grenadeInstance.Initialize(CurrentDamageMultiplier, CurrentRadiusMultiplier, CurrentFuseTimeMultiplier);

        // Apply impulse force
        Vector3 throwDirection = throwPoint.forward;
        Vector3 finalForce = (throwDirection * throwForce) + (Vector3.up * upwardForce);

        if (grenadeInstance.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(finalForce, ForceMode.Impulse);
        }

        OnGrenadeThrown?.Invoke();
    }
}
