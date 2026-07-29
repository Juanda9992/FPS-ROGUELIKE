using System;
using UnityEngine;

public class FPSWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public float fireRate = 0.2f;
    public float reloadTime = 1.5f;
    public float range = 100f;

    [Header("References")]
    public Camera playerCamera;
    public LayerMask hitMask;

    private float nextFireTime = 0f;
    private bool isReloading = false;

    // Eventos
    public event Action OnShoot;
    public event Action OnReload;
    public event Action<int> OnAmmoChanged;

    private Weapon currentWeapon;

    void Start()
    {
        currentAmmo = maxAmmo;
        NotifyAmmoChanged();
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
            if (currentAmmo < maxAmmo)
            {
                StartCoroutine(Reload());
            }
        }
    }

    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        maxAmmo = weapon.ammo;
        currentAmmo = maxAmmo;
        fireRate = weapon.fireRate;
        reloadTime = weapon.reloadTime;

        NotifyAmmoChanged();
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Sin munición");
            return;
        }

        nextFireTime = Time.time + fireRate;

        currentAmmo--;
        NotifyAmmoChanged();

        // Raycast
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, hitMask))
        {
            if(hit.collider.GetComponent<IDamageable>() is IDamageable damageable)
            {
                damageable.TakeDamage((int)currentWeapon.damage);
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

        Debug.Log("Recargando...");
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        NotifyAmmoChanged();

        isReloading = false;
    }
    void NotifyAmmoChanged()
    {
        OnAmmoChanged?.Invoke(currentAmmo);
    }
}