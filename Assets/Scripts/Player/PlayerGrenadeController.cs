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
    [SerializeField] private float baseCooldown = 5f;

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 3;
    [SerializeField] private int currentAmmo;

    [Header("PlayerStatsManager Integration")]
    [SerializeField] private string damageStatName = "GrenadeDamageMultiplier";
    [SerializeField] private string radiusStatName = "GrenadeRadiusMultiplier";
    [SerializeField] private string cooldownStatName = "GrenadeCooldownMultiplier";

    private PlayerInputActions input;
    private float lastThrowTime = -999f;

    // Events for UI / Systems
    public event Action<int> OnGrenadeThrown;
    public event Action<float, float> OnCooldownChanged; // (remainingTime, totalCooldown)

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsAmmoFull => currentAmmo >= maxAmmo;

    public float CurrentCooldown => baseCooldown / cooldownMultiplierStat.Value;
    public bool IsOnCooldown => Time.time < lastThrowTime + CurrentCooldown;
    public float RemainingCooldown => Mathf.Max(0f, (lastThrowTime + CurrentCooldown) - Time.time);

    private Stat damageMultiplierStat;
    private Stat radiusMultiplierStat;
    private Stat cooldownMultiplierStat;
    private bool wasOnCooldown;

    private void Awake()
    {
        input = new PlayerInputActions();
        input.Player.Grenade.performed += OnGrenadeInputPerformed;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private bool _isGameStarted;

    private void Start()
    {
        if (GameEventsManager.Instance != null)
        {
            GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
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
        }
    }

    private void HandleGameStarted()
    {
        _isGameStarted = true;
        currentAmmo = maxAmmo;
        OnGrenadeThrown?.Invoke(currentAmmo);
        GetStats();
    }

    private void Update()
    {
        if (!_isGameStarted)
        {
            return;
        }

        if (IsOnCooldown)
        {
            wasOnCooldown = true;
            OnCooldownChanged?.Invoke(RemainingCooldown, CurrentCooldown);
        }
        else if (wasOnCooldown)
        {
            wasOnCooldown = false;
            OnCooldownChanged?.Invoke(0f, CurrentCooldown);
        }
    }

    public void AddGrenade(int amount)
    {
        currentAmmo = Mathf.Min(maxAmmo, currentAmmo + amount);
        OnGrenadeThrown?.Invoke(currentAmmo);
    }

    public void GetStats()
    {
        damageMultiplierStat = PlayerStatsManager.Instance.GetStatByName(damageStatName);
        radiusMultiplierStat = PlayerStatsManager.Instance.GetStatByName(radiusStatName);
        cooldownMultiplierStat = PlayerStatsManager.Instance.GetStatByName(cooldownStatName);
    }
    private void OnGrenadeInputPerformed(InputAction.CallbackContext context)
    {
        TryThrowGrenade();
    }

    public bool TryThrowGrenade()
    {
        if (!_isGameStarted)
        {
            return false;
        }
        if (IsOnCooldown)
        {
            Debug.Log($"[PlayerGrenadeController] Grenade on cooldown! Remaining: {RemainingCooldown:F1}s");
            return false;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log($"[PlayerGrenadeController] No grenades left!");
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
        grenadeInstance.Initialize(damageMultiplierStat.Value, radiusMultiplierStat.Value);

        // Apply impulse force
        Vector3 throwDirection = throwPoint.forward;
        Vector3 finalForce = (throwDirection * throwForce) + (Vector3.up * upwardForce);

        if (grenadeInstance.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(finalForce, ForceMode.Impulse);
        }

        currentAmmo--;
        OnGrenadeThrown?.Invoke(currentAmmo);
    }
}
