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

    private int currentIndex = 0;
    private PlayerWeaponInstance currentWeapon;

    private float nextFireTime = 0f;
    private bool isReloading = false;
    private bool isShooting = false;

    private PlayerInputActions input;

    // Events
    public event Action OnShoot;
    public event Action OnReload;
    public event Action<int, int> OnAmmoChanged;

    private void Awake()
    {
        input = new PlayerInputActions();

        input.Player.Shoot.performed += _ => isShooting = true;
        input.Player.Shoot.canceled += _ => isShooting = false;

        input.Player.Reload.performed += _ => TryReload();

        input.Player.ChangeWeapon.performed += ctx => HandleScrollInput(ctx.ReadValue<float>());
    }

    private void OnEnable()
    {
        PauseManager.Instance.OnPauseChanged += OnPauseChanged;
        input.Enable();
    }

    private void OnDisable()
    {
        PauseManager.Instance.OnPauseChanged -= OnPauseChanged;
        input.Disable();
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
        input.Player.Disable();
    }

    public void OnResume()
    {
        input.Player.Enable();
    }
    #endregion

    private void Start()
    {

        damageMultiplierStat = PlayerStatsManager.Instance.GetStatByName("DamageMultiplier");
        reloadSpeedStat = PlayerStatsManager.Instance.GetStatByName("ReloadSpeedMultiplier");
        fireRateMultiplierStat = PlayerStatsManager.Instance.GetStatByName("FireRateMultiplier");

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
        if (isReloading)
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

        NotifyAmmoChanged();
    }

    private void Shoot()
    {
        if (currentWeapon == null || currentWeapon.currentAmmo <= 0)
        {
            return;
        }

        float fireRateMult = (fireRateMultiplierStat != null && fireRateMultiplierStat.Value != 0) ? fireRateMultiplierStat.Value : 1f;
        float fireRate = currentWeapon.fireRate / fireRateMult;
        nextFireTime = Time.time + fireRate;

        currentWeapon.DecreaseAmmo(1);
        NotifyAmmoChanged();


        _recoilController.ApplyRecoil(currentWeapon.weaponData);


        if (playerCamera != null)
        {
            float spreadAngle = (_recoilController != null) ? _recoilController.CurrentSpread : 0f;
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
                    float dmgMult = (damageMultiplierStat != null) ? damageMultiplierStat.Value : 1f;
                    damageable.TakeDamage(Mathf.RoundToInt(currentWeapon.damage * dmgMult));
                }
            }
        }

        OnShoot?.Invoke();
    }

    private void TryReload()
    {
        if (currentWeapon == null || isReloading)
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

        float reloadMult = (reloadSpeedStat != null && reloadSpeedStat.Value != 0) ? reloadSpeedStat.Value : 1f;
        float reloadDuration = currentWeapon.reloadTime / reloadMult;
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
}