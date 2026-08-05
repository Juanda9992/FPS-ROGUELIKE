using UnityEngine;
using TMPro;
public class WeaponUI : MonoBehaviour
{
    [SerializeField] FPSWeapon weapon;

    [SerializeField] private TextMeshProUGUI ammoText;
    void OnEnable()
    {
        weapon.OnShoot += HandleShoot;
        weapon.OnReload += HandleReload;
        weapon.OnAmmoChanged += UpdateAmmo;
    }

    void OnDisable()
    {
        weapon.OnShoot -= HandleShoot;
        weapon.OnReload -= HandleReload;
        weapon.OnAmmoChanged -= UpdateAmmo;
    }

    void HandleShoot()
    {
    }

    void HandleReload()
    {
        ammoText.text = "Reloading...";
    }

    void UpdateAmmo(int ammo)
    {
        ammoText.text = $"{ammo}/{weapon.GetCurrentWeapon().weaponData.ammo}";

        if(weapon.GetCurrentWeapon().currentAmmo <= 0)
        {
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.color = Color.white;
        }
    }
}