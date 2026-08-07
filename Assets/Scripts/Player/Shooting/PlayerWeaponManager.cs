using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapons")]
    public Weapon[] weapons;
    [SerializeField] private PlayerWeaponInstance[] weaponInstances;
    [SerializeField] private GameObject[] weaponPrefabs;

    [Header("References")]
    public Camera playerCamera;
    public LayerMask hitMask;

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
    public event Action<int> OnAmmoChanged;

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
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

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
        if (weapons == null || weapons.Length == 0) return;

        currentIndex++;
        if (currentIndex >= weapons.Length)
        {
            currentIndex = 0;
        }

        EquipWeapon(currentIndex);
    }

    private void PreviousWeapon()
    {
        if (weapons == null || weapons.Length == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = weapons.Length - 1;
        }

        EquipWeapon(currentIndex);
    }

    private void EquipWeapon(int index)
    {
        if (weaponInstances == null || index < 0 || index >= weaponInstances.Length) return;

        isReloading = false;
        nextFireTime = 0f;
        StopAllCoroutines();

        currentWeapon = weaponInstances[index];

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
        if (currentWeapon != null && currentWeapon.weaponData != null)
        {
            Debug.Log($"Equipped weapon: {currentWeapon.weaponData.weaponName}");
        }
    }

    private void Shoot()
    {
        if (currentWeapon == null) return;

        if (currentWeapon.currentAmmo <= 0)
        {
            Debug.Log("Sin munición");
            return;
        }

        float fireRateMult = (fireRateMultiplierStat != null && fireRateMultiplierStat.Value != 0) ? fireRateMultiplierStat.Value : 1f;
        float fireRate = currentWeapon.fireRate / fireRateMult;
        nextFireTime = Time.time + fireRate;

        currentWeapon.currentAmmo--;
        NotifyAmmoChanged();

        if (playerCamera != null)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 50f, hitMask))
            {
                if (hit.collider.GetComponent<IDamageable>() is IDamageable damageable)
                {
                    float dmgMult = (damageMultiplierStat != null) ? damageMultiplierStat.Value : 1f;
                    damageable.TakeDamage(Mathf.RoundToInt(currentWeapon.damage * dmgMult));
                    Debug.Log(hit.collider.name);
                }
            }
        }

        OnShoot?.Invoke();
    }

    private void TryReload()
    {
        if (currentWeapon == null || isReloading) return;

        if (currentWeapon.currentAmmo < currentWeapon.weaponData.ammo)
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

        currentWeapon.currentAmmo = currentWeapon.weaponData.ammo;
        NotifyAmmoChanged();

        isReloading = false;
    }

    private void NotifyAmmoChanged()
    {
        if (currentWeapon != null)
        {
            OnAmmoChanged?.Invoke(currentWeapon.currentAmmo);
        }
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