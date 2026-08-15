using UnityEngine;

public class AmmoOrb : OrbBase
{
    [SerializeField] private bool isGrenadeReload = false;

    public bool IsGrenadeReload => isGrenadeReload;

    private void Awake()
    {
        orbType = OrbType.Ammo;
    }

    protected override bool CanBePickedUp(GameObject player)
    {
        if (!consumeOnlyIfNeeded)
        {
            return true;
        }

        if (isGrenadeReload)
        {
            if (player.TryGetComponent<PlayerGrenadeController>(out var grenadeController))
            {
                return !grenadeController.IsAmmoFull;
            }
        }
        else
        {
            if (player.TryGetComponent<PlayerWeaponManager>(out var weaponManager))
            {
                var weaponInstance = weaponManager.GetCurrentWeaponInstance();
                if (weaponInstance != null)
                {
                    return weaponInstance.currentReserveAmmo < weaponInstance.maxAmmo;
                }
            }
        }

        return true;
    }
    protected override void ApplyEffect(GameObject player)
    {
        if (isGrenadeReload)
        {
            PlayerGrenadeController grenadeController = player.GetComponent<PlayerGrenadeController>();
            if (grenadeController != null)
            {
                grenadeController.AddGrenade(ValueAmount);
            }
        }
        else
        {
            PlayerWeaponManager weaponManager = player.GetComponent<PlayerWeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.AddAmmoToCurrentWeapon(ValueAmount);
            }
        }
    }
}
