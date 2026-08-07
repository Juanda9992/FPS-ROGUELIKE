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


    [Header("PlayerStatsManager Integration")]
    [SerializeField] private string damageStatName = "GrenadeDamageMultiplier";
    [SerializeField] private string radiusStatName = "GrenadeRadiusMultiplier";
    [SerializeField] private string cooldownStatName = "GrenadeCooldownMultiplier";

    private PlayerInputActions input;
    private float lastThrowTime = -999f;

    // Events for UI / Systems
    public event Action OnGrenadeThrown;
    public event Action<float, float> OnCooldownUpdated; // (remainingTime, totalCooldown)

    public float CurrentCooldown => baseCooldown / cooldownMultiplierStat.Value;
    public bool IsOnCooldown => Time.time < lastThrowTime + CurrentCooldown;
    public float RemainingCooldown => Mathf.Max(0f, (lastThrowTime + CurrentCooldown) - Time.time);

    private Stat damageMultiplierStat;
    private Stat radiusMultiplierStat;
    private Stat cooldownMultiplierStat;

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

    private void Start()
    {
        GetStats();
    }

    private void Update()
    {
        if (IsOnCooldown)
        {
            OnCooldownUpdated?.Invoke(RemainingCooldown, CurrentCooldown);
        }
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
        grenadeInstance.Initialize(damageMultiplierStat.Value, radiusMultiplierStat.Value);

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
