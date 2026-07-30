using System;
using UnityEngine;

public class FPSWeapon : MonoBehaviour
{
    [SerializeField] private Stat damageMultiplierStat;
    [SerializeField] private Stat reloadSpeedStat;
    [Header("References")]
    public Camera playerCamera;
    public LayerMask hitMask;

    private float nextFireTime = 0f;
    private bool isReloading = false;

    // Eventos
    public event Action OnShoot;
    public event Action OnReload;
    public event Action<int> OnAmmoChanged;

    private PlayerWeaponInstance currentWeapon;

    private void Start()
    {
        damageMultiplierStat = PlayerStatsManager.Instance.GetStatByName("DamageMultiplier");
        reloadSpeedStat = PlayerStatsManager.Instance.GetStatByName("ReloadSpeedMultiplier");
    }
    void Update()
    {
        if (isReloading)
        {
            return;
        }

        // Disparo
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
        }

        // Recarga
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentWeapon.currentAmmo < currentWeapon.weaponData.ammo)
            {
                StopCoroutine(Reload());
                StartCoroutine(Reload());
            }
        }
    }

    public void SetWeapon(PlayerWeaponInstance weapon)
    {
        isReloading = false;
        nextFireTime = 0f;
        StopCoroutine(Reload());
        currentWeapon = weapon;
        NotifyAmmoChanged();
    }

    void Shoot()
    {
        if (currentWeapon.currentAmmo <= 0)
        {
            Debug.Log("Sin munición");
            return;
        }

        nextFireTime = Time.time + currentWeapon.fireRate;

        currentWeapon.currentAmmo--;
        NotifyAmmoChanged();

        // Raycast
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50, hitMask))
        {
            if(hit.collider.GetComponent<IDamageable>() is IDamageable damageable)
            {
                damageable.TakeDamage((Mathf.RoundToInt(currentWeapon.damage * damageMultiplierStat.Value)));
                Debug.Log(hit.collider.name);
            }
        }

        // Evento
        OnShoot?.Invoke();
    }

    private System.Collections.IEnumerator Reload()
    {
        isReloading = true;

        OnReload?.Invoke();

        float reloadTime = currentWeapon.reloadTime / reloadSpeedStat.Value;
        Debug.Log($"Recargando... Tiempo de recarga: {reloadTime} segundos");
        yield return new WaitForSeconds(currentWeapon.reloadTime / reloadSpeedStat.Value);

        currentWeapon.currentAmmo = currentWeapon.weaponData.ammo;
        NotifyAmmoChanged();

        isReloading = false;
    }
    void NotifyAmmoChanged()
    {
        OnAmmoChanged?.Invoke(currentWeapon.currentAmmo);
    }

    public PlayerWeaponInstance GetCurrentWeapon()
    {
        return currentWeapon;
    }
}