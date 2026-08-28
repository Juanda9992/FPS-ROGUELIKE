using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponManager : MonoBehaviour, IPausable
{
    [Header("Weapons")]
    public Weapon[] weapons;
    [SerializeField] private PlayerWeaponInstance[] weaponInstances;
    [SerializeField] private GameObject[] weaponPrefabs;

    [Header("References")]
    public Camera playerCamera;
    public LayerMask hitMask;
    [SerializeField] private PlayerRecoilController _recoilController;

    [Header("Stats")]
    [SerializeField] private Stat damageMultiplierStat;
    public Stat reloadSpeedStat;
    [SerializeField] private Stat fireRateMultiplierStat;
    [SerializeField] private Stat _critChanceStat;
    [SerializeField] private Stat _criticalDamageStat;

    private int currentIndex = 0;
    private PlayerWeaponInstance currentWeapon;

    private float nextFireTime = 0f;
    private bool isReloading = false;
    private bool isShooting = false;

    private PlayerInputActions _input;

    // Events
    public event Action OnShoot;
    public event Action OnReload;
    public event Action OnWeaponChanged;
    public event Action<int, int> OnAmmoChanged;

    private void Awake()
    {
        _input = new PlayerInputActions();

        _input.Player.Shoot.performed += _ => isShooting = true;
        _input.Player.Shoot.canceled += _ => isShooting = false;

        _input.Player.Reload.performed += _ => TryReload();

        _input.Player.ChangeWeapon.performed += ctx => HandleScrollInput(ctx.ReadValue<float>());
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
        isShooting = false;
        isReloading = false;
        if (_input != null)
        {
            _input.Player.Disable();
        }
    }

    public void OnResume()
    {
        if (_input != null)
        {
            _input.Player.Enable();
        }
    }
    #endregion

    private bool _isGameStarted;

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

    private void HandleGameStarted()
    {
        _isGameStarted = true;

        damageMultiplierStat = PlayerStatsManager.Instance.GetStatByName("DamageMultiplier");
        reloadSpeedStat = PlayerStatsManager.Instance.GetStatByName("ReloadSpeedMultiplier");
        fireRateMultiplierStat = PlayerStatsManager.Instance.GetStatByName("FireRateMultiplier");
        _critChanceStat = PlayerStatsManager.Instance.GetStatByName("CritChance") ?? PlayerStatsManager.Instance.GetStatByName("CriticalChance") ?? PlayerStatsManager.Instance.GetStatByName("Crit Chance");
        _criticalDamageStat = PlayerStatsManager.Instance.GetStatByName("CriticalDamage") ?? PlayerStatsManager.Instance.GetStatByName("CritDamage") ?? PlayerStatsManager.Instance.GetStatByName("Critical Damage");

        if (weapons != null && weapons.Length > 0)
        {
            weaponInstances = new PlayerWeaponInstance[weapons.Length];
            for (int i = 0; i < weapons.Length; i++)
            {
                weaponInstances[i] = new PlayerWeaponInstance(weapons[i]);
            }

            EquipWeapon(0);
        }
    }

    private void Update()
    {
        if (!_isGameStarted || isReloading)
        {
            return;
        }

        if (isShooting && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void HandleScrollInput(float scroll)
    {
        if (!_isGameStarted)
        {
            return;
        }

        if (scroll > 0.01f)
        {
            NextWeapon();
        }
        else if (scroll < -0.01f)
        {
            PreviousWeapon();
        }
    }

    private void NextWeapon()
    {
        currentIndex++;
        if (currentIndex >= weapons.Length)
        {
            currentIndex = 0;
        }

        EquipWeapon(currentIndex);
    }

    private void PreviousWeapon()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = weapons.Length - 1;
        }

        EquipWeapon(currentIndex);
    }

    private void EquipWeapon(int index)
    {
        isReloading = false;
        nextFireTime = 0f;
        StopAllCoroutines();

        currentWeapon = weaponInstances[index];

        if (_recoilController != null && currentWeapon != null)
        {
            _recoilController.ResetSpread(currentWeapon.weaponData);
        }

        if (weaponPrefabs != null)
        {
            for (int i = 0; i < weaponPrefabs.Length; i++)
            {
                if (weaponPrefabs[i] != null)
                {
                    weaponPrefabs[i].SetActive(i == index);
                }
            }
        }

        OnWeaponChanged?.Invoke();
        NotifyAmmoChanged();
    }

    private void Shoot()
    {
        if (currentWeapon == null || currentWeapon.currentAmmo <= 0)
        {
            return;
        }

        float fireRateMult = fireRateMultiplierStat.Value;
        float fireRate = currentWeapon.fireRate / fireRateMult;
        nextFireTime = Time.time + fireRate;

        currentWeapon.DecreaseAmmo(1);
        NotifyAmmoChanged();
        _recoilController.ApplyRecoil(currentWeapon.weaponData);

        float spreadAngle = _recoilController.CurrentSpread;
        Vector3 shootDirection = playerCamera.transform.forward;

        if (spreadAngle > 0.001f)
        {
            float spreadAngleRad = spreadAngle * Mathf.Deg2Rad;
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * Mathf.Tan(spreadAngleRad);
            shootDirection = (playerCamera.transform.forward + playerCamera.transform.right * randomCircle.x + playerCamera.transform.up * randomCircle.y).normalized;
        }

        Ray ray = new Ray(playerCamera.transform.position, shootDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, hitMask))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                float dmgMult = damageMultiplierStat.Value;
                float finalDamage = currentWeapon.damage * dmgMult;

                if (UnityEngine.Random.Range(0f, 100f) < _critChanceStat.Value)
                {
                    finalDamage *= _criticalDamageStat.Value;
                }

                damageable.TakeDamage(Mathf.RoundToInt(finalDamage));
            }
        }

        OnShoot?.Invoke();
        GameEventsManager.Instance.TriggerPlayerShot();
    }

    private void TryReload()
    {
        if (!_isGameStarted || currentWeapon == null || isReloading)
        {
            return;
        }

        if (currentWeapon.CanReload())
        {
            StopAllCoroutines();
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        OnReload?.Invoke();
        GameEventsManager.Instance.TriggerPlayerReload();
        float reloadDuration = currentWeapon.reloadTime / reloadSpeedStat.Value;
        yield return new WaitForSeconds(reloadDuration);

        currentWeapon.Reload();
        NotifyAmmoChanged();

        isReloading = false;
    }

    private void NotifyAmmoChanged()
    {
        if (currentWeapon != null)
        {
            OnAmmoChanged?.Invoke(currentWeapon.currentAmmo, currentWeapon.currentReserveAmmo);
        }
    }

    public void AddAmmoToCurrentWeapon(int amount)
    {
        if (currentWeapon == null)
        {
            return;
        }

        currentWeapon.IncreaseAmmo(amount);
        NotifyAmmoChanged();
    }

    public void AddAmmoToWeapon(int weaponIndex, int amount)
    {
        if (weaponInstances != null && weaponIndex >= 0 && weaponIndex < weaponInstances.Length)
        {
            weaponInstances[weaponIndex].IncreaseAmmo(amount);
            if (weaponInstances[weaponIndex] == currentWeapon)
            {
                NotifyAmmoChanged();
            }
        }
    }

    public void AddAmmoToAllWeapons(int amount)
    {
        if (weaponInstances == null)
        {
            return;
        }

        foreach (var instance in weaponInstances)
        {
            instance.IncreaseAmmo(amount);
        }
        NotifyAmmoChanged();
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon?.weaponData;
    }

    public PlayerWeaponInstance GetCurrentWeaponInstance()
    {
        return currentWeapon;
    }

    #region Context Menu Tests
    [ContextMenu("Test Next Weapon")]
    private void TestNextWeapon()
    {
        NextWeapon();
    }

    [ContextMenu("Test Previous Weapon")]
    private void TestPreviousWeapon()
    {
        PreviousWeapon();
    }

    [ContextMenu("Test Reload")]
    private void TestReload()
    {
        TryReload();
    }

    [ContextMenu("Test Add 50 Ammo")]
    private void TestAddAmmo()
    {
        AddAmmoToCurrentWeapon(50);
    }
    #endregion
}