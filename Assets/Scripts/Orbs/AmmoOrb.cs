using UnityEngine;

public class AmmoOrb : OrbBase
{
    [SerializeField] private bool isGrenadeReload = false;
    private void Awake()
    {
        orbType = OrbType.Ammo;
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
