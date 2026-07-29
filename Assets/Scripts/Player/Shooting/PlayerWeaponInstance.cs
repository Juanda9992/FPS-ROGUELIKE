[System.Serializable]
public class PlayerWeaponInstance
{
    public Weapon weaponData;
    public float damage;
    public int currentAmmo;
    public float reloadTime;
    public float fireRate;

    public PlayerWeaponInstance(Weapon weapon)
    {
        weaponData = weapon;

        // Copiamos los valores base
        damage = weapon.damage;
        currentAmmo = weapon.ammo;
        reloadTime = weapon.reloadTime;
        fireRate = weapon.fireRate;
    }
}