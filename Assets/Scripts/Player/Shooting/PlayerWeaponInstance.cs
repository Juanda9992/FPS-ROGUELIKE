using UnityEngine;

[System.Serializable]
public class PlayerWeaponInstance
{
    public Weapon weaponData;
    public float damage;
    public int currentAmmo;
    public int currentReserveAmmo;
    public int chargerAmmo;
    public int maxAmmo;
    public float reloadTime;
    public float fireRate;

    public PlayerWeaponInstance(Weapon weapon)
    {
        weaponData = weapon;

        // Copiamos los valores base
        damage = weapon.damage;
        chargerAmmo = weapon.chargerAmmo;
        maxAmmo = weapon.maxAmmo;
        currentAmmo = weapon.chargerAmmo;
        currentReserveAmmo = weapon.maxAmmo;
        reloadTime = weapon.reloadTime;
        fireRate = weapon.fireRate;
    }

    public bool CanReload()
    {
        return currentAmmo < chargerAmmo && currentReserveAmmo > 0;
    }

    public void DecreaseAmmo(int amount = 1)
    {
        currentAmmo = Mathf.Max(0, currentAmmo - amount);
    }

    public void IncreaseAmmo(int amount)
    {
        currentReserveAmmo = Mathf.Min(maxAmmo, currentReserveAmmo + amount);
    }

    public void Reload()
    {
        int needed = chargerAmmo - currentAmmo;
        int ammoToTransfer = Mathf.Min(needed, currentReserveAmmo);
        currentAmmo += ammoToTransfer;
        currentReserveAmmo -= ammoToTransfer;
    }
}