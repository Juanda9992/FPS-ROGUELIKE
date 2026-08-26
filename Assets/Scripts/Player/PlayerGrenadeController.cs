using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrenadeController : MonoBehaviour, IPausable
{
    [Header("Grenade Settings")]
    [SerializeField] private GrenadeBase _grenadePrefab;
    [SerializeField] private Transform _throwPoint;
    [SerializeField] private float _throwForce = 15f;
    [SerializeField] private float _upwardForce = 2f;
    [SerializeField] private float _baseCooldown = 5f;

    [Header("Ammo Settings")]
    [SerializeField] private int _maxAmmo = 3;
    [SerializeField] private int _currentAmmo;

    [Header("PlayerStatsManager Integration")]
    [SerializeField] private string _damageStatName = "GrenadeDamageMultiplier";
    [SerializeField] private string _radiusStatName = "GrenadeRadiusMultiplier";
    [SerializeField] private string _cooldownStatName = "GrenadeCooldownMultiplier";

    private PlayerInputActions _input;
    private float _lastThrowTime = -999f;

    // Events for UI / Systems
    public event Action<int> OnGrenadeThrown;
    public event Action<float, float> OnCooldownChanged; // (remainingTime, totalCooldown)

    public int CurrentAmmo => _currentAmmo;
    public int MaxAmmo => _maxAmmo;
    public bool IsAmmoFull => _currentAmmo >= _maxAmmo;

    public float CurrentCooldown => _baseCooldown / (_cooldownMultiplierStat != null ? _cooldownMultiplierStat.Value : 1f);
    public bool IsOnCooldown => Time.time < _lastThrowTime + CurrentCooldown;
    public float RemainingCooldown => Mathf.Max(0f, (_lastThrowTime + CurrentCooldown) - Time.time);

    private Stat _damageMultiplierStat;
    private Stat _radiusMultiplierStat;
    private Stat _cooldownMultiplierStat;
    private bool _wasOnCooldown;
    private bool _isGameStarted;

    private void Awake()
    {
        _input = new PlayerInputActions();
        _input.Player.Grenade.performed += OnGrenadeInputPerformed;
    }

    private void OnEnable()
    {
        PauseManager.Instance.OnPauseChanged += OnPauseChanged;

        _input.Enable();
    }

    private void OnDisable()
    {
        PauseManager.Instance.OnPauseChanged -= OnPauseChanged;
        _input.Disable();
    }

    private void Start()
    {
        GameEventsManager.Instance.OnGameStarted += HandleGameStarted;
        if (GameEventsManager.Instance.IsGameStarted)
        {
            HandleGameStarted();
        }
    }

    private void OnDestroy()
    {
        GameEventsManager.Instance.OnGameStarted -= HandleGameStarted;
        _input.Dispose();
    }

    #region Pause And Resume Methods
    private void OnPauseChanged(bool isPaused)
    {
        if (isPaused)
        {
            OnPause();
        }
        else
        {
            OnResume();
        }
    }

    public void OnPause()
    {
        _input.Player.Disable();
    }

    public void OnResume()
    {
        _input.Player.Enable();
    }
    #endregion

    private void HandleGameStarted()
    {
        _isGameStarted = true;
        _currentAmmo = _maxAmmo;
        OnGrenadeThrown?.Invoke(_currentAmmo);
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
            _wasOnCooldown = true;
            OnCooldownChanged?.Invoke(RemainingCooldown, CurrentCooldown);
        }
        else if (_wasOnCooldown)
        {
            _wasOnCooldown = false;
            OnCooldownChanged?.Invoke(0f, CurrentCooldown);
        }
    }

    public void AddGrenade(int amount)
    {
        _currentAmmo = Mathf.Min(_maxAmmo, _currentAmmo + amount);
        OnGrenadeThrown?.Invoke(_currentAmmo);
    }

    public void GetStats()
    {
        _damageMultiplierStat = PlayerStatsManager.Instance.GetStatByName(_damageStatName);
        _radiusMultiplierStat = PlayerStatsManager.Instance.GetStatByName(_radiusStatName);
        _cooldownMultiplierStat = PlayerStatsManager.Instance.GetStatByName(_cooldownStatName);
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

        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused)
        {
            return false;
        }

        if (IsOnCooldown)
        {
            Debug.Log($"[PlayerGrenadeController] Grenade on cooldown! Remaining: {RemainingCooldown:F1}s");
            return false;
        }

        if (_currentAmmo <= 0)
        {
            Debug.Log($"[PlayerGrenadeController] No grenades left!");
            return false;
        }

        ThrowGrenade();
        return true;
    }

    private void ThrowGrenade()
    {
        _lastThrowTime = Time.time;
        Vector3 spawnPos = _throwPoint.position;
        Quaternion spawnRot = _throwPoint.rotation;
        GrenadeBase grenadeInstance = Instantiate(_grenadePrefab, spawnPos, spawnRot);

        float dmgMult = _damageMultiplierStat != null ? _damageMultiplierStat.Value : 1f;
        float radMult = _radiusMultiplierStat != null ? _radiusMultiplierStat.Value : 1f;

        // Pass parameterized stats to the grenade instance
        grenadeInstance.Initialize(dmgMult, radMult);

        // Apply impulse force
        Vector3 throwDirection = _throwPoint.forward;
        Vector3 finalForce = (throwDirection * _throwForce) + (Vector3.up * _upwardForce);

        if (grenadeInstance.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(finalForce, ForceMode.Impulse);
        }

        _currentAmmo--;
        OnGrenadeThrown?.Invoke(_currentAmmo);
    }
}
